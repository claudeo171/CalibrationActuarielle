using Stochastique.Distributions.Continous;
using Stochastique.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stochastique.Distributions.Discrete;
using Expr = MathNet.Symbolics.SymbolicExpression;
using Stochastique.Enums;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public class CopuleAMH: CopuleArchimedienne
    {
        [MessagePack.IgnoreMember]
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
            AddParameter(new CopuleParameter(CopuleParameterName.thetaAMH, CopuleHelper.RechercheDichotomique(0, 0.99999,(a)=>FonctionTau(tau,a))));
            base.Initialize(value, typeCalibration);
            Distribution = new GeometricDistribution(1 - GetParameter(CopuleParameterName.thetaAMH).Value);
        }

        private void communConstructeurs(double theta)
        {
            if (theta < 0 || theta > 1)
            {
                throw new Exception("Theta doit être compris entre 0 et 1");
            }

            AddParameter(new CopuleParameter(CopuleParameterName.thetaAMH, theta));
            Distribution = new GeometricDistribution(1 - theta);
        }

        protected override double Generateur(double t)
        {
            return Math.Log((1 - Theta * (1 - t)) / t);
        }

        protected override double InverseGenerateur(double t)
        {
            return (1 - Theta) / (Math.Exp(t) - Theta);
        }

        protected override Expr InverseGenerator()
        {
            var theta = Expr.Variable("thetaAMH");
            var t = Expr.Variable("t");
            return (1- theta)/(Expr.E.Pow(t)-theta);
        }

        private int B(int n,int k)
        {
            if(k==1)
            {
                return 1;
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
