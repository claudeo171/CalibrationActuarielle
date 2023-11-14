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
    /// <summary>
    /// Represents Hypergeometrical distribution
    /// https://fr.wikipedia.org/wiki/Loi_hyperg%C3%A9om%C3%A9trique
    /// </summary>
    public class HyperGeometricalDistribution : DiscreteDistribution
    {
        /// <summary>
        /// p parameter value
        /// </summary>
        public double NP => GetParameter(ParametreName.Np).Value;
        /// <summary>
        /// n parameter value
        /// </summary>
        public double n => GetParameter(ParametreName.n).Value;
        /// <summary>
        /// N parameter value
        /// </summary>
        public double N => GetParameter(ParametreName.N).Value;

        public override TypeDistribution Type => TypeDistribution.Hypergeometrical;

        public override double ExpextedValue()
        {
            return n * NP / N;
        }

        public override double Variance()
        {
            return n * NP * (N - NP) / N / N * (N - n) / (N - 1);
        }

        protected override double PDFInt(int k)
        {
            return Math.Exp(
                SpecialFunctions.FactorialLn((int)(NP ))
                + SpecialFunctions.FactorialLn((int)(N-NP))
                + SpecialFunctions.FactorialLn((int)n)
                + SpecialFunctions.FactorialLn((int)(N - n))
                - (SpecialFunctions.FactorialLn((int)(NP) - k)
                + SpecialFunctions.FactorialLn(k)
                + SpecialFunctions.FactorialLn((int)(n - k))
                + SpecialFunctions.FactorialLn((int)((N - NP) - n + k))
                + SpecialFunctions.FactorialLn((int)N)
                ));
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            //We take the hypothesis of NP = 0,5 * N
            AddParameter(new Parameter(ParametreName.n,(int)(ev*2)));
            AddParameter(new Parameter(ParametreName.N, (-2*ev+2* variance / ev )/(2*variance/ev-1)));
            AddParameter(new Parameter(ParametreName.Np, N*0/5));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(variance));
        }
    }
}
