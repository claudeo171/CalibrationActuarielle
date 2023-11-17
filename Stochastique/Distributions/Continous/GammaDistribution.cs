using MathNet.Numerics;
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
    public class GammaDistribution : Distribution
    {
        [Key(6)]
        public double K => GetParameter(ParametreName.k).Value;

        [Key(7)]
        public double Theta => GetParameter(ParametreName.theta).Value;

        [Key(8)]
        public override TypeDistribution Type => TypeDistribution.Gamma;

        public override double CDF(double x)
        {
            return SpecialFunctions.GammaLowerIncomplete(K,x/Theta) / SpecialFunctions.Gamma(K);
        }

        public override double ExpextedValue()
        {
            return K * Theta;
        }

        public override double PDF(double x)
        {
            return Math.Exp(Math.Log(x)* (K-1) -(x/Theta)-Math.Log(Theta)*K -SpecialFunctions.GammaLn(K));
        }

        public override double Variance()
        {
            return K * Theta * Theta;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            AddParameter(new Parameter(ParametreName.theta, variance/ev));
            AddParameter(new Parameter(ParametreName.k, ev*ev / variance));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(variance));
        }
    }
}
