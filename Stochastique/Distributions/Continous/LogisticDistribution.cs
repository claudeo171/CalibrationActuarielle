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
    public class LogisticDistribution : Distribution
    {
        public LogisticDistribution() { }
        public LogisticDistribution(double mu, double s)
        {
            AddParameter(new Parameter(ParametreName.mu, mu));
            AddParameter(new Parameter(ParametreName.s, s));
        }

        [IgnoreMember]
        public override TypeDistribution Type => TypeDistribution.Logistic;

        [IgnoreMember]
        public double Mu => GetParameter(ParametreName.mu).Value;

        [IgnoreMember]
        public double S => GetParameter(ParametreName.s).Value;
        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            var mean = values.Mean();
            var variance = values.Variance();
            var s = Math.Sqrt(3 * variance) / Math.PI;
            var mu = mean;
            return new List<Parameter> { new Parameter(ParametreName.mu, mu), new Parameter(ParametreName.s, s) };
        }

        public override double CDF(double x)
        {
            return 1 / (1 + Math.Exp(-(x - Mu) / S));
        }

        public override double ExpextedValue()
        {
            return Mu;
        }

        public override double Kurtosis()
        {
            return 1.2;
        }

        public override double PDF(double x)
        {
            var z = Math.Exp(-(x - Mu) / S);
            return z / (S * (1 + z) * (1 + z));
        }

        public override double Skewness()
        {
            return 0;
        }

        public override double Variance()
        {
            return S * S * Math.PI * Math.PI / 3;
        }
        public override double Simulate(Random r)
        {
            var u = r.NextDouble();
            return Mu+ S * Math.Log(u / (1 - u));
        }
    }
}
