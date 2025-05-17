using Accord.Math;
using Accord.Math.Optimization;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class TukeyDistribution : Distribution
    {
        [MemoryPack.MemoryPackIgnore]
        public double Lambda => GetParameter(ParametreName.lambda).Value;
        public override TypeDistribution Type => TypeDistribution.Tukey;
        [MemoryPack.MemoryPackConstructor]
        public TukeyDistribution()
        {

        }
        public TukeyDistribution(double lambda)
        {
            AddParameter(new Parameter(ParametreName.lambda, lambda));
        }

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            return null;
        }

        public override double CDF(double x)
        {
            return BrentSearch.Find(InverseCDF, x, 0, 1, 1e-10);
        }

        public override double ExpextedValue()
        {
            return 0;
        }

        public override double Kurtosis()
        {
            return 0;
        }

        public override double PDF(double x)
        {
            return 1.0 / QuantileDensityFunction(CDF(x));
        }

        public override double Skewness()
        {
            return 0;
        }

        public override double Variance()
        {
            if (Lambda == 0)
                return (Math.PI * Math.PI) / 3.0;

            double a = 2.0 / (Lambda * Lambda);
            double b = 1 / (1 + 2 * Lambda);
            double c = Gamma.Function(Lambda + 1);
            double d = Gamma.Function(2 * Lambda + 2);
            return a * (b - (c * c) / d);
        }
        public override double InverseCDF(double x)
        {
            if (Lambda == 0)
                return Math.Log(x / (1 - x));

            double a = Math.Pow(x, Lambda);
            double b = Math.Pow(1 - x, Lambda);

            return (a - b) / Lambda;
        }
        public double QuantileDensityFunction(double p)
        {
            return Math.Pow(p, Lambda - 1) + Math.Pow(1 - p, Lambda - 1);
        }
    }
}
