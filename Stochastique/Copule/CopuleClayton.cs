using Stochastique.Distributions.Continous;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    public class CopuleClayton:CopuleArchimedienne
    {
        private double Theta => GetParameter(CopuleParameterName.thetaClayton).Value;

        public CopuleClayton(int dimension, double theta)
        {
            communConstructeurs(theta);
            CheckDimension(dimension);
        }

        public CopuleClayton(double theta)
        {
            communConstructeurs(theta);
            Dimension = 2;
        }

        public CopuleClayton()
        {
        }

        private void communConstructeurs(double theta)
        {
            if (theta < -1 || theta == 0)
            {
                throw new Exception("Theta doit être supérieur ou égal à -1 et non nul");
            }

            AddParameter(new CopuleParameter(CopuleParameterName.thetaClayton,theta));
            distribution = new GammaDistribution(1 / theta, theta);
        }

        protected override double Generateur(double t)
        {
            return (Math.Pow(t, -Theta) - 1) / Theta;
        }

        protected override double InverseGenerateur(double t)
        {
            return Math.Pow(Theta * t + 1, -1 / Theta);
        }

        public override double DensityCopula(IEnumerable<double> u)
        {
            return (1+Theta)*Math.Pow(Math.Exp( u.Sum(a=>Math.Log(a))),-1-Theta)*Math.Pow(-1 + u.Sum(a => Math.Pow(a, -Theta)), -2 - 1 / Theta);
        }

        public override double CDFCopula(List<double> u)
        {
           return Math.Pow(Math.Max(0,u.Sum(a=>Math.Pow(a,-Theta))-1),-1/Theta);
        }

        protected override double InverseGenerateurDerivate(double t, int ordre)
        {
            return -Math.Pow(Theta, ordre - 1) * Math.Pow(1 + Theta * t, -1 / Theta - ordre)*CopuleHelper.NegativeProd(ordre-1,1/Theta);
        }
        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaClayton, 2 * tau / (1 - tau)));
            base.Initialize(value, typeCalibration);
        }
    }
}
