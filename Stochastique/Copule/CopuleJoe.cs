using Stochastique.Distributions.Discrete;
using Expr = MathNet.Symbolics.SymbolicExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Symbolics;
using Stochastique.Enums;
using Stochastique.SpecialFunction;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public partial class CopuleJoe : CopuleArchimedienne
    {
        public CopuleJoe() : base(2)
        {
            Type = Enums.TypeCopule.Joe;
        }
        public CopuleJoe(double theta) : base(2)
        {
            Dimension = 2;
            AddParameter(new CopuleParameter(CopuleParameterName.thetaJoe, theta));
            Distribution=new JoeDistribution(theta);
        }
        [MessagePack.IgnoreMember]
        private double Theta => GetParameter(CopuleParameterName.thetaJoe).Value;
        protected override double Generateur(double t)
        {
            return -Math.Log(1 - Math.Pow(1.0 - t, Theta));
        }
        protected override double InverseGenerateur(double t)
        {
            return 1 - Math.Pow((1 - Math.Exp(-t)), 1.0 / Theta);
        }

        protected override Expr InverseGenerator(SymbolicExpression param,List<SymbolicExpression> copuleParameter)
        {
            
            return 1- (1-(-param).Exp()).Pow(1 / copuleParameter[0]);
        }

        protected override Expr Generator(SymbolicExpression param, List<SymbolicExpression> copuleParameter)
        {
            return -(1 - (1 - param).Pow(copuleParameter[0])).Ln();
        }

        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaJoe, CopuleHelper.RechercheDichotomique(1.00001, 50, (a) => FonctionTau(tau, a))));
            base.Initialize(value, typeCalibration);
            Distribution = new JoeDistribution(1 - GetParameter(CopuleParameterName.thetaJoe).Value);
        }

        private double FonctionTau(double tau, double theta)
        {
            return tau - (1 + 4 / theta *  Debye.gsl_sf_debye_1_e(theta));
        }
        
        public override double DensityCopula(IEnumerable<double> u)
        {
            SymbolicExpression  val= SymbolicExpression.Variable("u");
            SymbolicExpression theta = SymbolicExpression.Variable("theta");
            var generator= Generator(val, new List<Expr> { theta });
            var generatorDerivative= generator.Differentiate(val);
            var generatorXDerivative = generatorDerivative;
            for (int i = 0; i < Dimension - 1; i++)
            {
                generatorXDerivative= generatorXDerivative.Differentiate(val);
            }
            var rst = 1.0;
            foreach(var valeur in u)
            {
                rst *= generatorDerivative.Evaluate(new Dictionary<string, FloatingPoint> { { "u",valeur }, { "theta", Theta } }).RealValue;
            }
            rst *= generatorXDerivative.Evaluate(new Dictionary<string, FloatingPoint> { { "u", CDFCopula(u.ToList()) }, { "theta", Theta } }).RealValue;
            rst /= Math.Pow(generatorDerivative.Evaluate(new Dictionary<string, FloatingPoint> { { "u", CDFCopula(u.ToList()) }, { "theta", Theta } }).RealValue,Dimension);
            return rst;
        }
    }
}
