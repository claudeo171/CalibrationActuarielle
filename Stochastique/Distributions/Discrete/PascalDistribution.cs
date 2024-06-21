using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    [MessagePackObject]
    public class PascalDistribution : DiscreteDistribution
    {
        [MessagePack.IgnoreMember]
        public override TypeDistribution Type => TypeDistribution.Pascal;

        [MessagePack.IgnoreMember]
        public double P => GetParameter(ParametreName.p).Value;

        [MessagePack.IgnoreMember]
        public double R => GetParameter(ParametreName.r).Value;

        public override double ExpextedValue()
        {
            return R / P;
        }

        public override double Variance()
        {
            return R * (1 - P) / (P * P);
        }

        public override double Skewness()
        {
            return (2 - P) / Math.Sqrt(R * (1 - P));
        }

        public override double Kurtosis()
        {
            return (6 * (1 - P) + P * P) / (R * (1 - P));
        }

        protected override double PDFInt(int k)
        {
            if (k < R)
            {
                return 0;
            }
            else
            {
                return Math.Exp(SpecialFunctions.FactorialLn(k - 1) - SpecialFunctions.FactorialLn((int)(R - 1)) - SpecialFunctions.FactorialLn((int)(k - R)) + R * Math.Log(P) + (k - R) * Math.Log(1 - P));
            }
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameters(CalibrateWithMoment(value));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(Variance()));
        }
        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            result.Add(new Parameter(ParametreName.p, Math.Max(0, ev / (ev + variance))));
            var p = result[0].Value;
            result.Add(new Parameter(ParametreName.r, Math.Max(1, ev * p)));
            return result;
        }
    }
}
