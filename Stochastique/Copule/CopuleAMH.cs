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
using MathNet.Symbolics;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public partial class CopuleAMH: CopuleArchimedienne
    {
        [MessagePack.IgnoreMember]
        private double Theta => GetParameter(CopuleParameterName.thetaAMH).Value;

        public CopuleAMH(int dimension, double theta):base(dimension)
        {
            communConstructeurs(theta);
            CheckDimension(dimension);
        }

        public CopuleAMH(double theta):base(2)
        {
            communConstructeurs(theta);
            Dimension = 2;
        }

        public CopuleAMH() : base(2)
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

        protected override Expr InverseGenerator(SymbolicExpression param, List<SymbolicExpression> copuleParam)
        {
            return (1- copuleParam[0]) /(Expr.E.Pow(param) - copuleParam[0]);
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

        protected override Expr Generator(Expr param, List<Expr> copuleParameter)
        {
            return (1 - copuleParameter[0] * (1 - param) / param).Log();
        }
    }
}
