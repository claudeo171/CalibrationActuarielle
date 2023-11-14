using MathNet.Numerics.Statistics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class ExponentialDistribution : Distribution
    {
        public override TypeDistribution Type => TypeDistribution.Exponential;

        public double Labda => GetParameter(ParametreName.lambda).Value;
        public override double CDF(double x)
        {
            return x < 0 ? 0 : 1 - Math.Exp(-Labda * x);
        }

        public override double ExpextedValue()
        {
            return 1 / Labda;
        }

        public override double PDF(double x)
        {

            return x < 0 ? 0 : Labda * Math.Exp(-Labda * x);
        }

        public override double Variance()
        {
            return 1 / (Labda * Labda);
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            AddParameter(new Parameter(ParametreName.lambda, (1/ev + Math.Sqrt( 1/variance))/2));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10*ev);
        }
    }
}
