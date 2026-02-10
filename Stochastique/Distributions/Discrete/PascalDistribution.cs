using MathNet.Numerics;
using MathNet.Numerics.Distributions;
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
    public partial class PascalDistribution : DiscreteDistribution
    {
        public override TypeDistribution Type => TypeDistribution.Pascal;

        [MemoryPack.MemoryPackIgnore]
        public double P => GetParameter(ParametreName.p).Value;

        [MemoryPack.MemoryPackIgnore]
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
                return Math.Exp(
                    SpecialFunctions.FactorialLn(k - 1)
                    - SpecialFunctions.GammaLn(R)
                    - SpecialFunctions.GammaLn(k - R+1) 
                    + R * Math.Log(P) 
                    + (k - R) * Math.Log(1 - P));
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
        public override double Simulate(Random r)
        {
            var lambda = Gamma.Sample(r, R, P / (1 - P));
            var c = Math.Exp(-lambda);
            var p1 = 1.0;
            var k = 0;
            do
            {
                k = k + 1;
                p1 = p1 * r.NextDouble();
            }
            while (p1 >= c);
            return ((int)R) + k - 1;
        }
    }
}
