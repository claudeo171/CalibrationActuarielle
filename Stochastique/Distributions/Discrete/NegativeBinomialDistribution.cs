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
    public class NegativeBinomialDistribution : DiscreteDistribution
    {
        [Key(11)]
        public override TypeDistribution Type => TypeDistribution.NegativeBinomial;

        [Key(12)]
        private double P => GetParameter(ParametreName.p).Value;

        [Key(13)]
        private double R => GetParameter(ParametreName.r).Value;

        public override double ExpextedValue()
        {
            return R * (1 - P) / P;
        }

        public override double Variance()
        {
            return R * (1 - P) / (P * P);
        }

        protected override double PDFInt(int k)
        {
            return Math.Exp(SpecialFunctions.FactorialLn((int)(k + R - 1)) - SpecialFunctions.FactorialLn(k) - SpecialFunctions.FactorialLn((int)(R - 1)) + R * Math.Log(P) + k * Math.Log(1 - P));
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            AddParameter(new Parameter(ParametreName.p, Math.Min(1, Math.Max(0, ev / variance))));
            AddParameter(new Parameter(ParametreName.r, Math.Max(1, P == 1 ? 0 : ev * P / (1 - P))));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(variance));
        }
    }
}
