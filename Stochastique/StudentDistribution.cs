using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public class StudentDistribution : Distribution
    {
        public override TypeDistribution Type => TypeDistribution.Student;

        public double n => GetParameter(ParametreName.n).Value;

        public StudentDistribution(int n)
        {
            AddParameter(new Parameter(ParametreName.n, n));
        }

        public override double CDF(double x)
        {
            //TODO int à la place du du double???
            return 0.5 + x * SpecialFunctions.Gamma((n + 1) / 2) * SpecialFunctions.GeneralizedHypergeometric(new double[] { 0.5, (n + 1) / 2 }, new double[] { 1.5 },(int)( -x * x / n));
        }


        public override double PDF(double x)
        {
            return SpecialFunctions.Gamma((n + 1) / 2) / (SpecialFunctions.Gamma(n / 2) * Math.Sqrt(Math.PI * n)) * Math.Pow(1+x*x/n,-(n+1)/2);
        }

        public override double ExpextedValue()
        {
            if(n>1)
            {
                return 0;
            }
            else
            {
                return double.NaN;
            }
        }

        public override double Variance()
        {
            if(n<=2)
            {
                return n / (n - 2);
            }    
            else
            {
                return double.NaN;
            }
        }
    }
}
