using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class WeibullDistribution : Distribution
    {
        public double Lambda => GetParameter(ParametreName.lambda).Value;
        public double K => GetParameter(ParametreName.k).Value;
        public override TypeDistribution Type => TypeDistribution.Weibull;

        public override double CDF(double x)
        {
            return 1- Math.Exp(-Math.Pow((x/Lambda),K));
        }

        public override double ExpextedValue()
        {
            return Lambda * SpecialFunctions.Gamma(1 + 1 / K);
        }

        public override double PDF(double x)
        {
            if(x<0)
            {
                return 0;
            }
            else
            {
                return (K/Lambda)*Math.Exp(Math.Log(x/Lambda)*(K-1)-Math.Pow(x/Lambda,K));
            }
        }

        public override double Variance()
        {
            return Lambda * Lambda * SpecialFunctions.Gamma(1 + 2 / K) - Math.Pow(ExpextedValue(), 2);
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            var mm = variance / ev / ev;
            AddParameter(new Parameter(ParametreName.k, OptimHelper.OptimHelper.GetOptimalParametre(ev, new Parameter(ParametreName.k,0).MinValue, new Parameter(ParametreName.k, 0).MaxValue, (a) =>
            {
                return Math.Pow(mm - (SpecialFunctions.Gamma(1 + 2 / a) - SpecialFunctions.Gamma(1 + 1 / a) * SpecialFunctions.Gamma(1 + 1 / a)) / (SpecialFunctions.Gamma(1 + 1 / a) * SpecialFunctions.Gamma(1 + 1 / a)), 2);
            })));
            AddParameter(new Parameter(ParametreName.lambda, Math.Pow( ev/(SpecialFunctions.Gamma(1/K)+1), K)));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(variance));
        }

    }
}
