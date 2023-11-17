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
    public class GeometricDistribution : DiscreteDistribution
    {
        [Key(11)]
        private double P => GetParameter(ParametreName.p).Value;

        [Key(12)]
        public override TypeDistribution Type => TypeDistribution.Geometric;

        public override double ExpextedValue()
        {
            return 1 / P;
        }

        public override double Variance()
        {
            return (1 - P) / (P * P);
        }

        protected override double PDFInt(int k)
        {
            return P * Math.Pow(1 - P, k);
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);

            AddParameter(new Parameter(ParametreName.p, Math.Min(1, Math.Max(0, 1/ev))));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(Variance()));
        }
    }
}
