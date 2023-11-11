using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    public class NegativeBinomialDistribution : DiscreteDistribution
    {
        public override TypeDistribution Type => TypeDistribution.NegativeBinomial;

        private double P => GetParameter(ParametreName.p).Value;
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
    }
}
