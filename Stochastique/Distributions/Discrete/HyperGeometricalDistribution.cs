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
    /// <summary>
    /// Represents Hypergeometrical distribution
    /// https://fr.wikipedia.org/wiki/Loi_hyperg%C3%A9om%C3%A9trique
    /// </summary>
    [MessagePackObject]
    public class HyperGeometricalDistribution : DiscreteDistribution
    {
        /// <summary>
        /// p parameter value
        /// </summary>
        [MessagePack.IgnoreMember]
        public double NP => GetParameter(ParametreName.Np).Value;

        /// <summary>
        /// n parameter value
        /// </summary>
        [MessagePack.IgnoreMember]
        public double n => GetParameter(ParametreName.n).Value;

        /// <summary>
        /// N parameter value
        /// </summary>
        [MessagePack.IgnoreMember]
        public double N => GetParameter(ParametreName.N).Value;
        [IgnoreMember]
        public double P => NP / N;

        [MessagePack.IgnoreMember]
        public override TypeDistribution Type => TypeDistribution.Hypergeometrical;

        public override double ExpextedValue()
        {
            return n * NP / N;
        }

        public override double Variance()
        {
            return n * NP * (N - NP) / N / N * (N - n) / (N - 1);
        }

        public override double Skewness()
        {
            return (N - 2 * n) * (1 - 2 * P) * Math.Sqrt(N - 1) / (Math.Sqrt(n * P * (1 - P) * (N - n)) * (N - 2));
        }

        public override double Kurtosis()
        {
            return (N - 1) * (N * N * 1 - 6 * P * (1 - P) + N * (1 - 6 * n) + 6 * n * n) / (n * P * (1 - P) * (N - 2) * (N - 3)) + 6 * N * N / ((N - 2) * (N - 3)) - 6;
        }

        protected override double PDFInt(int k)
        {
            if (k > NP || n > N || (N - NP) - n + k < 0)
            {
                return 0;
            }
            else
            {
                return Math.Exp(
                    SpecialFunctions.FactorialLn((int)(NP))
                    + SpecialFunctions.FactorialLn((int)(N - NP))
                    + SpecialFunctions.FactorialLn((int)n)
                    + SpecialFunctions.FactorialLn((int)(N - n))
                    - (SpecialFunctions.FactorialLn((int)(NP) - k)
                    + SpecialFunctions.FactorialLn(k)
                    + SpecialFunctions.FactorialLn((int)(n - k))
                    + SpecialFunctions.FactorialLn((int)((N - NP) - n + k))
                    + SpecialFunctions.FactorialLn((int)N)
                    ));
            }
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            //We take the hypothesis of NP = 0,5 * N
            AddParameter(new Parameter(ParametreName.n, Math.Max(1, (int)(ev * 2))));
            AddParameter(new Parameter(ParametreName.N, Math.Max(1, (int)(-2 * ev + 2 * variance / ev) / (2 * variance / ev - 1))));
            AddParameter(new Parameter(ParametreName.Np, (int)N * 0 / 5));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(variance));
        }
    }
}
