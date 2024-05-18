using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    [MessagePackObject]
    public class WeibullDistribution : Distribution
    {
        [Key(6)]
        public double Lambda => GetParameter(ParametreName.lambda).Value;

        [Key(7)]
        public double K => GetParameter(ParametreName.k).Value;

        [Key(8)]
        public override TypeDistribution Type => TypeDistribution.Weibull;
        [IgnoreMember]
        public override double InconditionnalMinimumPossibleValue => 0;

        public override double CDF(double x)
        {
            if (x <= 0)
            {
                return 0;
            }
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

        public override double Skewness()
        {
            var mu = ExpextedValue();
            var sigma = Math.Sqrt(Variance());
            return (Lambda*Lambda*Lambda*SpecialFunctions.Gamma(1+3/K)-3*mu*Variance()-mu*mu*mu)/sigma*sigma*sigma;
        }

        public override double Kurtosis()
        {
            var mu = ExpextedValue();
            var sigma2 = Variance();
            return (Lambda * Lambda * Lambda * Lambda* SpecialFunctions.Gamma(1 + 4 / K) - 4 * mu * sigma2*Math.Sqrt(sigma2)*Skewness()-4*sigma2*sigma2 -6*mu*mu*sigma2- mu * mu * mu *mu) / sigma2* sigma2;
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

        public override double[] Simulate(Random r, int nbSimulations)
        {
            double[] result = new double[nbSimulations];
            for (int i = 0; i < nbSimulations; i++)
                result[i] = K * Math.Pow(-Math.Log(r.NextDouble()), Lambda);
            return result;
        }

        public override double Simulate(Random r)
        {
            return base.Simulate(r, 1)[0];
        }

    }
}
