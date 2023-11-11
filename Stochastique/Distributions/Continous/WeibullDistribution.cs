using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class WeibullDistribution : Distribution
    {
        public double Lambda => GetParameter(ParametreName.lambda).Value;
        public double K => GetParameter(ParametreName.k).Value;
        public override TypeDistribution Type => TypeDistribution.Weibull;

        public override double CDF(double x)
        {
            return 1- Math.Exp(-Math.Pow((x/Lambda),K));
        }

        public override double ExpextedValue()
        {
            return Lambda * SpecialFunctions.Gamma(1 + 1 / K);
        }

        public override double PDF(double x)
        {
            if(x<0)
            {
                return 0;
            }
            else
            {
                return (K/Lambda)*Math.Exp(Math.Log(x/Lambda)*(K-1)-Math.Pow(x/Lambda,K));
            }
        }

        public override double Variance()
        {
            return Lambda * Lambda * SpecialFunctions.Gamma(1 + 2 / K) - Math.Pow(ExpextedValue(), 2);
        }
    }
}
