using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    public class BernouliDistribution : DiscreteDistribution
    {
        private double P => GetParameter(ParametreName.p).Value;
        public override TypeDistribution Type => TypeDistribution.Bernouli;
        protected override double MaxValue => 1;

        public override double ExpextedValue()
        {
            return P;
        }

        protected override double PDFInt(int x)
        {
            if (x == 0)
            {
                return 1 - P;
            }
            else
            {
                return P;
            }
        }

        public override double Variance()
        {
            return P * (1 - P);
        }
    }
}
