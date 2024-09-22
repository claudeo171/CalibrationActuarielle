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
    public class VecteurGaussien : VecteurAleatoire
    {
        private DenseVector vecteurEsperance;
        private DenseVector vecteurEcartType;
        private DenseMatrix matriceCorrelation;
        private DenseMatrix triangleInfCholesky;

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

            this.vecteurEsperance = new DenseVector(DenseVectorStorage<double>.OfVector(vecteurEsperance.Storage));
            this.vecteurEcartType = new DenseVector(DenseVectorStorage<double>.OfVector(vecteurEcartType.Storage));
            this.matriceCorrelation = new DenseMatrix(DenseColumnMajorMatrixStorage<double>.OfMatrix(matriceCorrelation.Storage));
            calculerCholesky();
        }

        public VecteurGaussien(DenseMatrix matriceCorrelation)
        {
            Dimension = Math.Min(matriceCorrelation.RowCount, matriceCorrelation.ColumnCount);
            vecteurEsperance = new DenseVector(Dimension);
            vecteurEcartType = new DenseVector(Dimension);
            this.matriceCorrelation = new DenseMatrix(Dimension);

            for (int i = 0; i < Dimension; i++)
            {
                checkLigneMatrice(i, matriceCorrelation);
                vecteurEcartType.At(i, 1.0);
            }
            this.matriceCorrelation = new DenseMatrix(DenseColumnMajorMatrixStorage<double>.OfMatrix(matriceCorrelation.Storage));
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
                triangleInfCholesky = (DenseMatrix)matriceCorrelation.Cholesky().Factor;
            }
            catch (Exception e)
            {
                if (Dimension == 2)
                {
                    triangleInfCholesky = new DenseMatrix(Dimension);

                    triangleInfCholesky.At(0, 0, matriceCorrelation[0, 1]);
                    triangleInfCholesky.At(0, 1, 0);
                    triangleInfCholesky.At(1, 0, 1);
                    triangleInfCholesky.At(1, 1, 0);
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

            matriceGaussiennes = (DenseMatrix)triangleInfCholesky.Multiply(matriceGaussiennes);
            List<List<double>> variables = new List<List<double>>();

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < nbSim; j++)
                {
                    matriceGaussiennes.At(i, j, vecteurEsperance.At(i) + vecteurEcartType.At(i) * matriceGaussiennes.At(i, j));
                }

                variables.Add(new List<double>(matriceGaussiennes.Row(i).ToArray()));
            }

            return variables;
        }
    }
}
