using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class CauchyDistribution : Distribution
    {
        public double A => GetParameter(ParametreName.a).Value;
        public double B => GetParameter(ParametreName.b).Value;
        public override TypeDistribution Type => TypeDistribution.Cauchy;

        public override double CDF(double x)
        {
            return 1 / Math.PI * Math.Atan((x-B)/A) +0.5;
        }

        public override double ExpextedValue()
        {
            return double.NaN;
        }

        public override double PDF(double x)
        {
            return A / (Math.PI * (A * A + (x - B) * (x - B)));
        }

        public override double Variance()
        {
            return double.NaN;
        }
    }
}
