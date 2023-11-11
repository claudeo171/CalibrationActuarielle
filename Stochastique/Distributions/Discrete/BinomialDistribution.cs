using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    public class BinomialDistribution : DiscreteDistribution
    {
        private double P => GetParameter(ParametreName.p).Value;
        private double N => GetParameter(ParametreName.n).Value;
        public override TypeDistribution Type => TypeDistribution.Binomial;

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
    }
}
