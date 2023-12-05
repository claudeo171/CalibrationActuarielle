using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    [MessagePackObject]
    public class StudentDistribution : Distribution
    {
        public StudentDistribution() { }
        [Key(6)]
        public override TypeDistribution Type => TypeDistribution.Student;

        [Key(7)]
        public double n => GetParameter(ParametreName.n).Value;

        public StudentDistribution(int n)
        {
            AddParameter(new Parameter(ParametreName.n, n));
        }

        public override double CDF(double x)
        {
            //TODO int à la place du du double???
            return 0.5 + x * SpecialFunctions.Gamma((n + 1) / 2) * SpecialFunctions.GeneralizedHypergeometric(new double[] { 0.5, (n + 1) / 2 }, new double[] { 1.5 }, (int)(-x * x / n));
        }


        public override double PDF(double x)
        {
            return SpecialFunctions.Gamma((n + 1) / 2) / (SpecialFunctions.Gamma(n / 2) * Math.Sqrt(Math.PI * n)) * Math.Pow(1 + x * x / n, -(n + 1) / 2);
        }

        public override double ExpextedValue()
        {
            if (n > 1)
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
            if (n > 2)
            {
                return n / (n - 2);
            }
            else
            {
                return double.NaN;
            }
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var variance = value.Variance();
            AddParameter(new Parameter(ParametreName.n,Math.Max(3, 2* variance/(variance-1))));
            base.Initialize(value, typeCalibration);    
        }
        public override double Skewness()
        {
            if (n > 3)
            {
                return 0;
            }
            else
            {
                return double.NaN;
            }
        }

        public override double Kurtosis()
        {
            if (n > 4)
            {
                return 6/(n-4);
            }
            else
            {
                return double.NaN;
            }
        }
    }
}
