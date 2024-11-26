using Stochastique.Distributions.Discrete;
using Expr = MathNet.Symbolics.SymbolicExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public class CopuleJoe : CopuleArchimedienne
    {
        public CopuleJoe() {
            Type = Enums.TypeCopule.Joe;
        }
        public CopuleJoe(double theta)
        {
            Dimension = 2;
            AddParameter(new CopuleParameter(CopuleParameterName.thetaJoe, theta));
            Distribution=new JoeDistribution(theta);
        }
        [MessagePack.IgnoreMember]
        private double Theta => GetParameter(CopuleParameterName.thetaJoe).Value;
        protected override double Generateur(double t)
        {
            return Math.Log(1 - Math.Pow(1 - t, Theta));
        }

        protected override double InverseGenerateur(double t)
        {
            return 1 - Math.Pow((1 - Math.Exp(-t)), 1 / Theta);
        }

        protected override Expr InverseGenerator()
        {
            var theta = Expr.Variable("thetaJoe");
            var t = Expr.Variable("t");
            return 1- (1-(-t).Exp()).Pow(1 / theta);
        }
    }
}
