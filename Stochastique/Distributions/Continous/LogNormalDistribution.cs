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
    public class LogNormalDistribution : NormalDistribution
    {
        [Key(6)]
        public override TypeDistribution Type => TypeDistribution.LogNormal;

        [Key(7)]
        public double mu => GetParameter(ParametreName.mu).Value;

        [Key(8)]
        public double sigma => GetParameter(ParametreName.sigma).Value;

        public override double CDF(double x)
        {
            return 0.5 + 0.5 * SpecialFunctions.Erf((Math.Log(x) - mu) / (sigma * Constants.Sqrt2));
        }

        public override double ExpextedValue()
        {
            return Math.Exp(mu + sigma / 2);
        }

        public override double PDF(double x)
        {
            return 1 / (x * sigma * Constants.Sqrt2Pi) * Math.Exp(-Math.Pow(Math.Log(x) - mu, 2) / (2 * sigma * sigma));
        }

        public override double Variance()
        {
            return (Math.Exp(sigma * sigma) - 1) * Math.Exp(2 * mu + sigma * sigma);
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double mu = 0;
            double sigma = 0;
            mu = value.Sum(a => Math.Log(a)) / value.Count();
            sigma = Math.Sqrt(value.Sum(a => Math.Log(a) * Math.Log(a)) / value.Count() - mu * mu);
            AddParameter(new Parameter(ParametreName.mu, mu));
            AddParameter(new Parameter(ParametreName.sigma, sigma));
            switch (typeCalibration)
            {
                case TypeCalibration.MaximumLikelyhood:
                    Optim(value, typeCalibration);
                    break;
                case TypeCalibration.LeastSquare:
                    Optim(value, typeCalibration);
                    break;
            }
            IntervaleForDisplay = new Intervale(0,Math.Exp( mu + 5 * sigma));

        }
        public override double Simulate(Random r)
        {
            return Math.Exp( base.Simulate(r));
        }
        public override double[] Simulate(Random r, int n)
        {
            return base.Simulate(r, n).Select(a=>Math.Exp(a)).ToArray();
        }
    }
}
