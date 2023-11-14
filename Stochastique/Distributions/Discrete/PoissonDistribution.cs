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
    public class PoissonDistribution : DiscreteDistribution
    {
        public override TypeDistribution Type => TypeDistribution.Poisson;
        public double Lambda => GetParameter(ParametreName.lambda).Value;
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
