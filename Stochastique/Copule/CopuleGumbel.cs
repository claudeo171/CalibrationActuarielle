using Stochastique.Enums;
using Stochastique.SpecialFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public class CopuleGumbel : CopuleArchimedienne
    {
        [MessagePack.IgnoreMember]
        public double Theta => GetParameter(CopuleParameterName.thetaGumbel).Value;
        public CopuleGumbel()
        {
            Type = TypeCopule.Gumbel;
        }
        protected override double Generateur(double t)
        {
            return Math.Pow(-Math.Log(t),Theta);
        }

        protected override double InverseGenerateur(double t)
        {
            return Math.Exp(Math.Pow(-t,1/Theta));
        }

        protected override Expr InverseGenerator()
        {
            var theta = Expr.Variable("thetaGumbel");
            var t = Expr.Variable("t");
            return (-t).Pow(1/theta).Exp();
        }
        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaGumbel, 1/1-tau ));
            base.Initialize(value, typeCalibration);
            //Distribution = new GeometricDistribution(1 - GetParameter(CopuleParameterName.thetaAMH).Value);
        }

    }
}
