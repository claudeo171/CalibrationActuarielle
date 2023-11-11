using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    internal class UniformDistribution : Distribution
    {
        public double A => GetParameter(ParametreName.a).Value;
        public double B => GetParameter(ParametreName.b).Value;
        public override TypeDistribution Type => TypeDistribution.Uniform;

        public override double CDF(double x)
        {
            if (x < A)
            {
                return 0;
            }
            else if (x > B)
            {
                return 1;
            }
            else
            {
                return (x - A) / (B - A);
            }
        }

        public override double ExpextedValue()
        {
            return (B + A) / 2;
        }

        public override double PDF(double x)
        {
            if (x < A)
            {
                return 0;
            }
            else if (x > B)
            {
                return 0;
            }
            else
            {
                return 1 / (B - A);
            }
        }

        public override double Variance()
        {
            return (B - A) * (B - A) / 12;
        }
    }
}
