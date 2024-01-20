using Stochastique.Distributions.Continous;
using Stochastique.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stochastique.Distributions.Discrete;

namespace Stochastique.Copule
{
    public class CopuleAMH: CopuleArchimedienne
    {
        private double Theta => GetParameter(CopuleParameterName.theta).Value;

        public CopuleAMH(int dimension, double theta)
        {
            communConstructeurs(theta);
            CheckDimension(dimension);
        }

        public CopuleAMH(double theta)
        {
            communConstructeurs(theta);
            Dimension = 2;
        }

        private void communConstructeurs(double theta)
        {
            if (theta < -1 || theta == 0)
            {
                throw new Exception("Theta doit être supérieur ou égal à -1 et non nul");
            }

            AddParameter(new CopuleParameter(CopuleParameterName.theta, theta));
            distribution = new GeometricDistribution(1 - theta);
        }

        protected override double Generateur(double t)
        {
            return Math.Log((1 - Theta * (1 - t)) / t);
        }

        protected override double InverseGenerateur(double t)
        {
            return (1 - Theta) / (Math.Exp(t) - Theta);
        }
        protected override double GenerateurDerivate(double t, int ordre)
        {
            return CopuleHelper.NegativeProd(ordre - 1)*(Math.Pow(t,-ordre)- Math.Pow(t+(Theta-1)/Theta, -ordre)) ;
        }
    }
}
