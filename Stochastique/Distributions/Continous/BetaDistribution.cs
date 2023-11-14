using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class BetaDistribution : Distribution
    {
        public override TypeDistribution Type => TypeDistribution.Beta;

        public double A => GetParameter(ParametreName.a).Value;
        public double B => GetParameter(ParametreName.b).Value;

        public override double CDF(double x)
        {
            return SpecialFunctions.BetaIncomplete(A, B, x) / SpecialFunctions.Beta(A, B);
        }

        public override double ExpextedValue()
        {
            return A / (A + B);
        }

        public override double PDF(double x)
        {
            return (x>1 || x<0)? 0 : 1 / SpecialFunctions.Beta(A, B) * Math.Pow(x,A-1) * Math.Pow(1-x, B - 1);
        }

        public override double Variance()
        {
            return A*B / ((A + B)* (A + B)+ (A + B + 1));
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double k = 0;
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            alglib.complex[] rst = new alglib.complex[3];
            var repot = new alglib.polynomialsolver.polynomialsolverreport();
            alglib.xparams xparams = new alglib.xparams(1);
            alglib.polynomialsolver.polynomialsolve(new double[4] { 0, ev * (ev - 1), variance * (4 * ev * ev * ev - 8 * ev * ev + 5 * ev + ev - 1), variance * (8 * ev * ev * ev - 12 * ev * ev + 6 * ev - 1) }, 3,ref rst, repot, xparams);
            
            AddParameter(new Parameter(ParametreName.k, k));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(Math.Max(0, k - 10 * k), k + 10 * k);
            base.Initialize(value, typeCalibration);
        }
    }
}
