using MathNet.Numerics.Statistics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class UniformDistribution : Distribution
    {
        public override bool IsTrunkable => false;
        [MemoryPack.MemoryPackConstructor]
        public UniformDistribution()
        {

        }
        public UniformDistribution(double a,double b)
        {
            AddParameter(new Parameter(ParametreName.a, a));
            AddParameter(new Parameter(ParametreName.b, b));
        }

        [MemoryPack.MemoryPackOrder(6)]
        public double A => GetParameter(ParametreName.a).Value;

        [MemoryPack.MemoryPackOrder(7)]
        public double B => GetParameter(ParametreName.b).Value;

        [MemoryPack.MemoryPackOrder(8)]
        public override TypeDistribution Type => TypeDistribution.Uniform;

        public override double CDF(double x)
        {
            if (x < A)
            {
                return 0;
            }
            else if (x > B)
            {
                return 1;
            }
            else
            {
                return (x - A) / (B - A);
            }
        }

        public override double ExpextedValue()
        {
            return (B + A) / 2;
        }

        public override double PDF(double x)
        {
            if (x < A)
            {
                return 0;
            }
            else if (x > B)
            {
                return 0;
            }
            else
            {
                return 1 / (B - A);
            }
        }

        public override double Variance()
        {
            return (B - A) * (B - A) / 12;
        }
        public override double Skewness()
        {
            return 0;
        }

        public override double Kurtosis()
        {
            return -6/5.0;
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameters(CalibrateWithMoment(value));
            if (typeCalibration == TypeCalibration.MaximumLikelyhood)
            {
                GetParameter(ParametreName.a).SetValue(value.Min());
                GetParameter(ParametreName.b).SetValue(value.Max());
            }
            else
            {
                base.Initialize(value, typeCalibration);
            }
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(Variance()));
        }
        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            if (variance * 192 - 60 * ev * ev > 0)
            {
                result.Add(new Parameter(ParametreName.b, Math.Max(value.Max(), (-2 * ev + Math.Sqrt(variance * 192 - 60 * ev * ev)) / 8)));
                result.Add(new Parameter(ParametreName.a, Math.Min(value.Min(), 2 * ev - result[0].Value)));
            }
            else
            {
                result.Add(new Parameter(ParametreName.b, value.Max()));
                result.Add(new Parameter(ParametreName.a, value.Min()));
            }
            return result;
        }
        public override double Simulate(Random r)
        {
            return r.NextDouble() * (B - A) + A;
        }

    }
}
