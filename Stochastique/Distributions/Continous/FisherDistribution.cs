using MathNet.Numerics;
using MathNet.Numerics.Statistics;
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
        public override TypeDistribution Type => TypeDistribution.Fisher;

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

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            AddParameter(new Parameter(ParametreName.d2,Math.Max(5, (int)(2*ev/ev-1))));
            AddParameter(new Parameter(ParametreName.d1,(int)(2*D2 * D2 * D2-4 * D2 * D2/(variance*(D2-2)*(D2-2)*(D2-4)-2*D2))));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * ev);
        }
    }
}
