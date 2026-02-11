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
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class CauchyDistribution : Distribution
    {
        public CauchyDistribution(double a, double b)
        {
            AddParameter(new Parameter(ParametreName.aCauchy, a));
            AddParameter(new Parameter(ParametreName.bCauchy, b));
        }
        [MemoryPack.MemoryPackConstructor]
        public CauchyDistribution() { }

        [MemoryPack.MemoryPackOrder(6)]
        public double A => GetParameter(ParametreName.aCauchy).Value;

        [MemoryPack.MemoryPackOrder(7)]
        public double B => GetParameter(ParametreName.bCauchy).Value;

        [MemoryPack.MemoryPackOrder(8)]
        public override TypeDistribution Type => TypeDistribution.Cauchy;

        public override double CDF(double x)
        {
            return 1 / Math.PI * Math.Atan((x - B) / A) + 0.5;
        }

        public override double ExpextedValue()
        {
            return double.NaN;
        }

        public override double PDF(double x)
        {
            return A / (Math.PI * (A * A + (x - B) * (x - B)));
        }

        public override double Variance()
        {
            return double.NaN;
        }
        public override double Skewness()
        {
            return double.NaN;
        }

        public override double Kurtosis()
        {
            return double.NaN;
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double k = 0;
            AddParameters(CalibrateWithMoment(value));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(Math.Max(0, B - 100 * A), B + 100 * A);
        }

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            var ev = Statistics.Mean(value);

            result.Add(new Parameter(ParametreName.aCauchy, 1));
            result.Add(new Parameter(ParametreName.bCauchy, ev));
            return result;
        }
        public override double[] Simulate(Random r, int nbSimulations)
        {
            var normal1 = new NormalDistribution(0, 1).Simulate(r, nbSimulations);
            var normal2 = new NormalDistribution(0, 1).Simulate(r, nbSimulations);
            return normal1.Select((a, i) => B + a.Divide(normal2[i], 0)).ToArray();
        }
        public override double Simulate(Random r)
        {
            return Simulate(r, 1)[0];
        }
    }
}
