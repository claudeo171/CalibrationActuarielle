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
    public class PascalDistribution : DiscreteDistribution
    {
        public override TypeDistribution Type => TypeDistribution.Pascal;
        public double P => GetParameter(ParametreName.p).Value;
        public double R => GetParameter(ParametreName.r).Value;

        public override double ExpextedValue()
        {
            return R / P;
        }

        public override double Variance()
        {
            return R * (1 - P) / (P * P);
        }

        protected override double PDFInt(int k)
        {
            return Math.Exp(SpecialFunctions.FactorialLn(k - 1) - SpecialFunctions.FactorialLn((int)(R - 1)) - SpecialFunctions.FactorialLn((int)(k - R)) + R * Math.Log(P) + (k - R) * Math.Log(1 - P));
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            AddParameter(new Parameter(ParametreName.p, Math.Max(0, ev/(ev+variance))));
            AddParameter(new Parameter(ParametreName.r, Math.Max(1, ev * P)));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(variance));
        }
    }
}
