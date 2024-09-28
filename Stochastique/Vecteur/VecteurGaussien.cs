using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using Stochastique.Distributions.Continous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Vecteur
{
    [MessagePack.MessagePackObject]
    public class VecteurGaussien : VecteurAleatoire
    {
        [MessagePack.Key(1)]
        public DenseVector VecteurEsperance { get; set; }
        [MessagePack.Key(2)]
        public DenseVector VecteurEcartType { get; set; }
        [MessagePack.Key(3)]
        public DenseMatrix MatriceCorrelation { get; set; }
        [MessagePack.Key(4)]
        public DenseMatrix TriangleInfCholesky { get; set; }

        public VecteurGaussien(DenseVector vecteurEsperance, DenseVector vecteurEcartType, DenseMatrix matriceCorrelation)
        {
            Dimension = Math.Min(vecteurEsperance.Count, Math.Min(vecteurEcartType.Count, Math.Min(matriceCorrelation.RowCount, matriceCorrelation.ColumnCount)));
            for (int i = 0; i < Dimension; i++)
            {
                if (vecteurEcartType.At(i) < 0)
                {
                    throw new Exception("Ecart-type négatif");
                }

                checkLigneMatrice(i, matriceCorrelation);
            }

            this.VecteurEsperance = new DenseVector(DenseVectorStorage<double>.OfVector(vecteurEsperance.Storage));
            this.VecteurEcartType = new DenseVector(DenseVectorStorage<double>.OfVector(vecteurEcartType.Storage));
            this.MatriceCorrelation = new DenseMatrix(DenseColumnMajorMatrixStorage<double>.OfMatrix(matriceCorrelation.Storage));
            calculerCholesky();
        }

        public VecteurGaussien(DenseMatrix matriceCorrelation)
        {
            Dimension = Math.Min(matriceCorrelation.RowCount, matriceCorrelation.ColumnCount);
            VecteurEsperance = new DenseVector(Dimension);
            VecteurEcartType = new DenseVector(Dimension);
            this.MatriceCorrelation = new DenseMatrix(Dimension);

            for (int i = 0; i < Dimension; i++)
            {
                checkLigneMatrice(i, matriceCorrelation);
                VecteurEcartType.At(i, 1.0);
            }
            this.MatriceCorrelation = new DenseMatrix(DenseColumnMajorMatrixStorage<double>.OfMatrix(matriceCorrelation.Storage));
            calculerCholesky();
        }

        protected void checkLigneMatrice(int i, DenseMatrix matriceCorrelation)
        {
            for (int j = 0; j <= i; j++)
            {
                if (i == j && matriceCorrelation.At(i, j) != 1 || matriceCorrelation.At(i, j) < -1 || matriceCorrelation.At(i, j) > 1 || matriceCorrelation.At(i, j) != matriceCorrelation.At(j, i))
                {
                    throw new Exception("La matrice de corrélation n'est pas une matrice de corrélation");
                }
            }
        }

        protected void calculerCholesky()
        {
            try
            {
                TriangleInfCholesky = (DenseMatrix)MatriceCorrelation.Cholesky().Factor;
            }
            catch (Exception e)
            {
                if (Dimension == 2)
                {
                    TriangleInfCholesky = new DenseMatrix(Dimension);

                    TriangleInfCholesky.At(0, 0, MatriceCorrelation[0, 1]);
                    TriangleInfCholesky.At(0, 1, 0);
                    TriangleInfCholesky.At(1, 0, 1);
                    TriangleInfCholesky.At(1, 1, 0);
                }

                Console.WriteLine(e.StackTrace);
            }
        }

        public override List<List<double>> Simuler(Random r,int nbSim)
        {
            NormalDistribution loiNormale = new NormalDistribution(0, 1);
            DenseMatrix matriceGaussiennes = new DenseMatrix(Dimension, nbSim);

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < nbSim; j++)
                {
                    matriceGaussiennes.At(i, j, loiNormale.Simulate(r));
                }
            }

            matriceGaussiennes = (DenseMatrix)TriangleInfCholesky.Multiply(matriceGaussiennes);
            List<List<double>> variables = new List<List<double>>();

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < nbSim; j++)
                {
                    matriceGaussiennes.At(i, j, VecteurEsperance.At(i) + VecteurEcartType.At(i) * matriceGaussiennes.At(i, j));
                }

                variables.Add(new List<double>(matriceGaussiennes.Row(i).ToArray()));
            }

            return variables;
        }
    }
}
