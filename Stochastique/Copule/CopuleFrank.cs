using Stochastique.Distributions.Discrete;
using Expr = MathNet.Symbolics.SymbolicExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stochastique.Enums;
using Stochastique.SpecialFunction;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public class CopuleFrank : CopuleArchimedienne
    {
        [MessagePack.IgnoreMember]
        public double Theta => GetParameter(CopuleParameterName.thetaFrank).Value;

        public CopuleFrank()
        {
            Type = TypeCopule.Frank;
        }
        public CopuleFrank(double theta)
        {
            AddParameter(new CopuleParameter(CopuleParameterName.thetaFrank, theta));
            Distribution = new LogarithmiqueDistribution(1-Math.Exp(-Theta));
        }
        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaFrank, CopuleHelper.RechercheDichotomique(0, 0.99999, (a) => FonctionTau(tau, a))));
            base.Initialize(value, typeCalibration);
            Distribution = new GeometricDistribution(1 - GetParameter(CopuleParameterName.thetaAMH).Value);
        }

        private double FonctionTau(double tau, double theta)
        {
            return tau - (1 - 4 / Theta *(1 - Debye.gsl_sf_debye_1_e(Theta)));
        }

        protected override double Generateur(double t)
        {
            return -Math.Log((Math.Exp(-Theta * t) - 1) / (Math.Exp(-Theta) - 1));
        }

        protected override double InverseGenerateur(double t)
        {
            return -1/Theta * Math.Log(1+Math.Exp(-t)*(Math.Exp(-Theta)-1));
        }

        protected override Expr InverseGenerator()
        {
            var theta = Expr.Variable("thetaFrank");
            var t = Expr.Variable("t");
            return -(1 / theta) * (-1 / t) *(1+(-t).Exp()*((-theta).Exp()-1)).Ln();
        }
    }
}
