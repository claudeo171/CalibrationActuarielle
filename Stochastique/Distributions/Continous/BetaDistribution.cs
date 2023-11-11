using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class BetaDistribution : Distribution
    {
        public override TypeDistribution Type => TypeDistribution.Beta;

        public double A => GetParameter(ParametreName.a).Value;
        public double B => GetParameter(ParametreName.b).Value;

        public override double CDF(double x)
        {
            return SpecialFunctions.BetaIncomplete(A, B, x) / SpecialFunctions.Beta(A, B);
        }

        public override double ExpextedValue()
        {
            return A / (A + B);
        }

        public override double PDF(double x)
        {
            return (x>1 || x<0)? 0 : 1 / SpecialFunctions.Beta(A, B) * Math.Pow(x,A-1) * Math.Pow(1-x, B - 1);
        }

        public override double Variance()
        {
            return A*B / ((A + B)* (A + B)+ (A + B + 1));
        }
    }
}
