using MathNet.Symbolics;
using Stochastique.Distributions.Continous;
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
    public partial class CopuleGumbel : CopuleArchimedienne
    {
        [MessagePack.IgnoreMember]
        public double Theta => GetParameter(CopuleParameterName.thetaGumbel).Value;
        public CopuleGumbel() : base(2)
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

        protected override Expr InverseGenerator(SymbolicExpression param, List<SymbolicExpression> copuleParameter)
        {
            return (-(param).Pow(1/ copuleParameter[0])).Exp();
        }
        protected override Expr Generator(SymbolicExpression param, List<SymbolicExpression> copuleParam)
        {
            return (-param.Ln()).Pow(copuleParam[0]);
        }
        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaGumbel, 1/1-tau ));
            base.Initialize(value, typeCalibration);
            Distribution = new StableDistribution(0, Math.Pow(Math.Cos(Math.PI / (2 * Theta)), Theta), 1 / Theta, 1);
        }

    }
}
