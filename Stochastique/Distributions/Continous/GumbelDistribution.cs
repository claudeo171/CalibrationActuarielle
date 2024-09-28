using Accord.Math;
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
    public class GumbelDistribution : Distribution
    {
        public GumbelDistribution() { }
        public GumbelDistribution(double mu, double beta)
        {
            AddParameter(new Parameter(ParametreName.mu, mu));
            AddParameter(new Parameter(ParametreName.beta, beta));
        }

        [IgnoreMember]
        public double Mu => GetParameter(ParametreName.mu).Value;

        [IgnoreMember]
        public double Beta => GetParameter(ParametreName.beta).Value;

        [IgnoreMember]
        public override TypeDistribution Type => TypeDistribution.Gumbel;

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            var mean = values.Mean();
            var variance = values.Variance();
            var beta = Math.Sqrt(6 * variance) / Math.PI;
            var mu = mean - beta * Constants.EulerGamma;
            return new List<Parameter> { new Parameter(ParametreName.mu, mu), new Parameter(ParametreName.beta, beta) };
        }

        public override double CDF(double x)
        {
            return Math.Exp(-Math.Exp(-(x - Mu) / Beta));
        }

        public override double ExpextedValue()
        {
            return Mu + Beta* Constants.EulerGamma;
        }

        public override double Kurtosis()
        {
            return 2.4;
        }

        public override double PDF(double x)
        {
            var z = Math.Exp(-(x - Mu) / Beta);
            return Math.Exp(-z) * z / Beta;
        }

        public override double Skewness()
        {
            return 1.14;
        }

        public override double Variance()
        {
            return Math.PI * Math.PI * Beta * Beta / 6;
        }

    }
}
