using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    public class Khi2Distribution : Distribution
    {
        public override TypeDistribution Type => TypeDistribution.Khi2;
        public Khi2Distribution()
        {
        }
        public double k => GetParameter(ParametreName.k).Value;

        public Khi2Distribution(int k) : base()
        {
            AddParameter(new Parameter(ParametreName.k, k));
        }
        public override double CDF(double x)
        {
            return SpecialFunctions.GammaLowerIncomplete(GetParameter(ParametreName.k).Value / 2, x) / SpecialFunctions.Gamma(GetParameter(ParametreName.k).Value / 2);
        }

        public override double PDF(double x)
        {
            return Math.Pow(0.5, k / 2) / SpecialFunctions.Gamma(k / 2) * Math.Pow(x, k / 2 - 1) * Math.Exp(-x / 2);
        }

        public override double ExpextedValue()
        {
            return GetParameter(ParametreName.k).Value;
        }

        public override double Variance()
        {
            return ExpextedValue() * 2;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double k = 0;
            k = value.Sum() / value.Count();
            AddParameter(new Parameter(ParametreName.k, k));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(Math.Max(0, k - 10 * k), k + 10 * k);
            base.Initialize(value, typeCalibration);
        }
    }
}
