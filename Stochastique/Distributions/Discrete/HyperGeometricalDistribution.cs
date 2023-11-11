using MathNet.Numerics;
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
        public double P => GetParameter(ParametreName.p).Value;
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
            return n * P;
        }

        public override double Variance()
        {
            return n * P * (1 - P) * (N - n) / (N - 1);
        }

        protected override double PDFInt(int k)
        {
            return Math.Exp(
                SpecialFunctions.FactorialLn((int)(P * N))
                + SpecialFunctions.FactorialLn((int)((1 - P) * N))
                + SpecialFunctions.FactorialLn((int)n)
                + SpecialFunctions.FactorialLn((int)(N - n))
                - (SpecialFunctions.FactorialLn((int)(P * N) - k)
                + SpecialFunctions.FactorialLn(k)
                + SpecialFunctions.FactorialLn((int)(n - k))
                + SpecialFunctions.FactorialLn((int)((1 - P) * n - n + k))
                + SpecialFunctions.FactorialLn((int)N)
                ));
        }
    }
}
