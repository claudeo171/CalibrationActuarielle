using MathNet.Numerics;
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
    public class Khi2Distribution : Distribution
    {
        [Key(6)]
        public override TypeDistribution Type => TypeDistribution.Khi2;
        public Khi2Distribution()
        {
        }

        [Key(7)]
        public double K => GetParameter(ParametreName.k).Value;

        public Khi2Distribution(int k) : base()
        {
            AddParameter(new Parameter(ParametreName.k, k));
        }
        public override double CDF(double x)
        {
            if(x<0)
            {
                return 0;
            }
            return SpecialFunctions.GammaLowerIncomplete(GetParameter(ParametreName.k).Value / 2, x) / SpecialFunctions.Gamma(GetParameter(ParametreName.k).Value / 2);
        }

        public override double PDF(double x)
        {
            return Math.Pow(0.5, K / 2) / SpecialFunctions.Gamma(K / 2) * Math.Pow(x, K / 2 - 1) * Math.Exp(-x / 2);
        }

        public override double ExpextedValue()
        {
            return GetParameter(ParametreName.k).Value;
        }

        public override double Variance()
        {
            return ExpextedValue() * 2;
        }
        public override double Skewness()
        {
            return Math.Sqrt(8.0/K);
        }

        public override double Kurtosis()
        {
            return 12.0/K;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double k = 0;
            k = value.Sum() / value.Count();
            
            if(value.Any(a=>a<0))
            {
                AddParameter(new Parameter(ParametreName.k, 1));
            }
            else
            {
                AddParameter(new Parameter(ParametreName.k, k));
                base.Initialize(value, typeCalibration);
            }
            
            IntervaleForDisplay = new Intervale(Math.Max(0, k - 10 * k), k + 10 * k);
        }
    }
}
