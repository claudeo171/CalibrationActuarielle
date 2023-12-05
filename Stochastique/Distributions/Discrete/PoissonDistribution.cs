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
    public class PoissonDistribution : DiscreteDistribution
    {
        [Key(11)]
        public override TypeDistribution Type => TypeDistribution.Poisson;

        [Key(12)]
        public double Lambda => GetParameter(ParametreName.lambda).Value;

        [Key(13)]
        protected override double MaxValue => int.MaxValue;

        public override double ExpextedValue()
        {
            return Lambda;
        }

        protected override double PDFInt(int x)
        {
            return Math.Exp(-Lambda) * Math.Exp(Math.Log(Lambda) * x - SpecialFunctions.FactorialLn(x));
        }

        public override double Variance()
        {
            return Lambda;
        }
        public override double Skewness()
        {
            return 1/Math.Sqrt(Lambda);
        }

        public override double Kurtosis()
        {
            return 1 / Lambda;
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            AddParameter(new Parameter(ParametreName.lambda, (ev + variance)/2));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(variance));
        }
    }
}
