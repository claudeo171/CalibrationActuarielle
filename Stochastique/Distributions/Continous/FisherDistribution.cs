using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class FisherDistribution : Distribution
    {
        public double D1 => GetParameter(ParametreName.d1).Value;
        public double D2 => GetParameter(ParametreName.d2).Value;
        public override TypeDistribution Type => throw new NotImplementedException();

        public override double CDF(double x)
        {
            return SpecialFunctions.BetaRegularized(D1 / 2, D2 / 2, D1 * x / (D1 * x + D2));
        }

        public override double ExpextedValue()
        {
            return D2>2? D2 / (D2 - 2):double.NaN;
        }

        public override double PDF(double x)
        {
            return Math.Sqrt( Math.Pow(D1*x,D1)*Math.Pow(D2,D2)/Math.Pow(D1*x+D2,D1+D2))/(x*SpecialFunctions.Beta(D1/2,D2/2));
        }

        public override double Variance()
        {
            return D2 > 4 ? 2*D2*D2*(D1+D2-2) / (D1*(D2 - 2)* (D2 - 2)* (D2 - 4)) : double.NaN;
        }
    }
}
