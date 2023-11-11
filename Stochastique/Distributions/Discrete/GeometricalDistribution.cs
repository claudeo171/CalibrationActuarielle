using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    public class GeometricalDistribution : DiscreteDistribution
    {
        private double P => GetParameter(ParametreName.p).Value;
        public override TypeDistribution Type => TypeDistribution.Geometric;

        public override double ExpextedValue()
        {
            return 1 / P;
        }

        public override double Variance()
        {
            return (1 - P) / (P * P);
        }

        protected override double PDFInt(int k)
        {
            return P * Math.Pow(1 - P, k);
        }
    }
}
