using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class BinomialDistribution : DiscreteDistribution
    {
        [MemoryPack.MemoryPackIgnore]
        private double P => GetParameter(ParametreName.p).Value;

        [MemoryPack.MemoryPackIgnore]
        private double N => GetParameter(ParametreName.n).Value;

        public override TypeDistribution Type => TypeDistribution.Binomial;

        protected override double MaxValue => N;

        public override double ExpextedValue()
        {
            return N * P;
        }

        protected override double PDFInt(int x)
        {
            if (N - x < 0)
            {
                return 0;
            }
            else
            {
                return Math.Exp(SpecialFunctions.FactorialLn((int)N) - SpecialFunctions.FactorialLn((int)(N - x)) - SpecialFunctions.FactorialLn(x) + x * Math.Log(P) + (N - x) * Math.Log(1 - P));
            }
        }

        public override double Variance()
        {
            return N * P * (1 - P);
        }

        public override double Skewness()
        {
            return (1-2*P)/Math.Sqrt(N*P*(1-P));
        }

        public override double Kurtosis()
        {
            return 1-6*P*(1-P)/(N*P*(1-P));
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
            if(variance>=ev)
            {
                variance = 0.999 * ev;
            }
            result.Add(new Parameter(ParametreName.p, Math.Min(1, Math.Max(0, (1 - variance / ev)))));
            result.Add(new Parameter(ParametreName.n, ev / result[0].Value));
            return result;
        }
    }
}
