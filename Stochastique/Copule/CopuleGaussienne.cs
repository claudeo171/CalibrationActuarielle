using MathNet.Numerics.LinearAlgebra.Double;
using MessagePack;
using Stochastique.Distributions.Continous;
using Stochastique.Enums;
using Stochastique.Vecteur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public partial class CopuleGaussienne : Copule, IMessagePackSerializationCallbackReceiver
    {
        [IgnoreMember]
        private DenseMatrix matriceCorrelations;
        [Key(5)]
        private double[][] matrice;
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
            AddParameter(new CopuleParameter(CopuleParameterName.rho, rho));
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

        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            Dimension = value.Count();
            if (Dimension==2)
            {
                var rho = Math.Sin(Math.PI / 2 * value.First().TauKendall(value.Last()));
                matriceCorrelations = new DenseMatrix(Dimension);
                matriceCorrelations.At(0, 0, 1);
                matriceCorrelations.At(0, 1, rho);
                matriceCorrelations.At(1, 0, rho);
                matriceCorrelations.At(1, 1, 1);
                AddParameter(new CopuleParameter(CopuleParameterName.rho, rho));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override double CDFCopula(List<double> u)
        {
            throw new NotImplementedException();
        }

        public override double DensityCopula(IEnumerable<double> u)
        {
            var distrib = new NormalDistribution(0,1);
            if(Dimension==2)
            {
                var rho = matriceCorrelations.At(0, 1);
                var a = distrib.InverseCDF(  u.First());
                var b = distrib.InverseCDF(u.Last());
                return 1 / Math.Sqrt(1 - rho * rho) * Math.Exp(-((a * a + b * b) * rho * rho - a * b * rho) / (2 * (1 - rho * rho)));
            }
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

        public void OnBeforeSerialize()
        {
            matrice = matriceCorrelations?.ToColumnArrays();
        }

        public void OnAfterDeserialize()
        {
            matriceCorrelations = new DenseMatrix( matrice.Length, matrice[0].Length);
            for(int i=0;i< matrice.Length;i++)
                for (int j = 0; j < matrice[i].Length; j++)
                    matriceCorrelations.At(i, j, matrice[i][j]);
        }
    }
}
