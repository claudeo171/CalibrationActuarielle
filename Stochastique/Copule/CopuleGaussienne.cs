using MathNet.Numerics.LinearAlgebra.Double;
using Stochastique.Vecteur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public class CopuleGaussienne : Copule
    {
        [MessagePack.Key(5)]
        private DenseMatrix matriceCorrelations;
        public CopuleGaussienne()
        {
            Type = Enums.TypeCopule.Gaussian;
        }
        public CopuleGaussienne(double rho)
        {
            if (rho < -1 || rho > 1)
            {
                throw new Exception();
            }

            Dimension = 2;
            matriceCorrelations = new DenseMatrix(Dimension);
            matriceCorrelations.At(0, 0, 1);
            matriceCorrelations.At(0, 1, rho);
            matriceCorrelations.At(1, 0, rho);
            matriceCorrelations.At(1, 1, 1);
        }

        public CopuleGaussienne(DenseMatrix matriceCorrelations)
        {
            Dimension = Math.Min(matriceCorrelations.RowCount, matriceCorrelations.ColumnCount);
            this.matriceCorrelations = new DenseMatrix(Dimension);
            for (int i = 0; i < Dimension; i++)
            {
                this.matriceCorrelations.At(i, i, 1);
                for (int j = 0; j < i; j++)
                {
                    if (matriceCorrelations.At(i, j) < 0 || matriceCorrelations.At(i, j) > 1)
                    {
                        throw new Exception("La matrice de corrélation n'est pas une matrice de corrélation");
                    }
                    this.matriceCorrelations.At(i, j, matriceCorrelations.At(i, j));
                    this.matriceCorrelations.At(j, i, this.matriceCorrelations.At(i, j));
                }
            }
        }

        public override double CDFCopula(List<double> u)
        {
            throw new NotImplementedException();
        }

        public override double DensityCopula(IEnumerable<double> u)
        {
            throw new NotImplementedException();
        }

        public override List<List<double>> SimulerCopule(Random r, int nbSim)
        {
            List<List<double>> gaussiennes = (new VecteurGaussien(matriceCorrelations)).Simuler(r, nbSim);
            List<List<double>> uniformes = new List<List<double>>();

            for (int i = 0; i < Dimension; i++)
            {
                uniformes.Add(gaussiennes[i].UniformeEmpirique());
            }

            return uniformes;
        }
    }
}
