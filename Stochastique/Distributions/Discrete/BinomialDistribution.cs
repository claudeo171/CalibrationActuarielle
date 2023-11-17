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
    public class BinomialDistribution : DiscreteDistribution
    {
        [Key(11)]
        private double P => GetParameter(ParametreName.p).Value;

        [Key(12)]
        private double N => GetParameter(ParametreName.n).Value;

        [Key(13)]
        public override TypeDistribution Type => TypeDistribution.Binomial;

        [Key(14)]
        protected override double MaxValue => N;

        public override double ExpextedValue()
        {
            return N * P;
        }

        protected override double PDFInt(int x)
        {
            return Math.Exp(SpecialFunctions.FactorialLn((int)N) - SpecialFunctions.FactorialLn((int)(N - x)) - SpecialFunctions.FactorialLn(x) + x * Math.Log(P) + (N - x) * Math.Log(1 - P));
        }

        public override double Variance()
        {
            return N * P * (1 - P);
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            AddParameter(new Parameter(ParametreName.p, Math.Min(1,Math.Max(0, (1-variance/ev)))));
            AddParameter(new Parameter(ParametreName.n, ev/P));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(variance));
        }
    }
}
