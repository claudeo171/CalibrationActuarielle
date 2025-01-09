using Stochastique.Distributions.Discrete;
using Expr = MathNet.Symbolics.SymbolicExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stochastique.Enums;
using Stochastique.SpecialFunction;
using MathNet.Symbolics;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public partial class CopuleFrank : CopuleArchimedienne
    {
        [MessagePack.IgnoreMember]
        public double Theta => GetParameter(CopuleParameterName.thetaFrank).Value;

        public CopuleFrank() : base(2)
        {
            Type = TypeCopule.Frank;
        }
        public CopuleFrank(double theta) : base(2)
        {
            AddParameter(new CopuleParameter(CopuleParameterName.thetaFrank, theta));
            Distribution = new LogarithmiqueDistribution(1-Math.Exp(-Theta));
        }
        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaFrank, CopuleHelper.RechercheDichotomique(0.00001, 30, (a) => FonctionTau(tau, a))));
            base.Initialize(value, typeCalibration);
            Distribution = new LogarithmiqueDistribution(1 - Math.Exp(-Theta));
        }

        private double FonctionTau(double tau, double theta)
        {
            return tau - (1 - 4 / theta * (1 - Debye.gsl_sf_debye_1_e(theta)));
        }

        protected override double Generateur(double t)
        {
            return -Math.Log((Math.Exp(-Theta * t) - 1) / (Math.Exp(-Theta) - 1));
        }

        protected override double InverseGenerateur(double t)
        {
            return -1/Theta * Math.Log(1+Math.Exp(-t)*(Math.Exp(-Theta)-1));
        }

        protected override Expr InverseGenerator(SymbolicExpression param,List<SymbolicExpression> copuleParam)
        {
            return -(1 / copuleParam[0]) * (1 + (-param).Exp() * ((-copuleParam[0]).Exp()-1)).Ln();
        }
        protected override Expr Generator(SymbolicExpression param, List<SymbolicExpression> copuleParam)
        {
            return -(((-copuleParam[0]*param).Exp()-1)/ ((-copuleParam[0] ).Exp() - 1)).Ln();
        }
    }
}
