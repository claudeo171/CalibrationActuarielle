using Stochastique.Distributions.Continous;
using Stochastique.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stochastique.Distributions.Discrete;
using Stochastique.Enums;

namespace Stochastique.Copule
{
    public class CopuleAMH: CopuleArchimedienne
    {
        private double Theta => GetParameter(CopuleParameterName.thetaAMH).Value;

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

        public CopuleAMH()
        {
            Type = TypeCopule.CopuleAMH;
        }
        private double FonctionTau(double tau,double theta)
        {
            return 1 + 3 * tau * theta * theta - 2 * ((1 - theta) * (1 - theta) * Math.Log(1 - theta) + theta);
        }
        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaAMH, CopuleHelper.RechercheDichotomique(-0.9999, 0.9999,(a)=>FonctionTau(tau,a))));
            base.Initialize(value, typeCalibration);
        }

        private void communConstructeurs(double theta)
        {
            if (theta < -1 || theta == 0)
            {
                throw new Exception("Theta doit être supérieur ou égal à -1 et non nul");
            }

            AddParameter(new CopuleParameter(CopuleParameterName.thetaAMH, theta));
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
        protected override double InverseGenerateurDerivate(double t, int ordre)
        {
            double rst = 0;
            for(int i=1;i<ordre;i++)
            {
                rst += Math.Exp(i * t) * Math.Pow(Math.Exp(t) - Theta, -ordre - 1) * B(ordre, i);
            }
            rst *= Theta - 1;
            return rst;
        }

        private int B(int n,int k)
        {
            if(k==1)
            {
                return 0;
            }
            if(k>n)
            {
                return 0;
            }
            else
            {
                return k * B(n - 1, k) - k * B(n - 1, k - 1);
            }
        }
    }
}
