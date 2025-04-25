using Accord;
using NWaves.Utils;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class StableDistribution : Distribution
    {
        public override TypeDistribution Type => TypeDistribution.Stable;
        public double Mu => GetParameter(ParametreName.mu).Value;
        public double Sigma => GetParameter(ParametreName.sigma).Value;
        public double Alpha => GetParameter(ParametreName.a).Value;
        public double Beta => GetParameter(ParametreName.beta).Value;
        public double C1 { get; set; }
        public double C2 { get; set; }
        public double C3 { get; set; }
        public double C4 { get; set; }
        public StableDistribution(double mu, double sigma, double alpha, double beta) : base()
        {
            if (sigma < 0 || alpha <= 0 || alpha > 2 || Math.Abs(beta) > 1)
            {
                throw new Exception();
            }
            AddParameter(new Parameter(ParametreName.mu, mu));
            AddParameter(new Parameter(ParametreName.sigma, sigma));
            AddParameter(new Parameter(ParametreName.a, alpha));
            AddParameter(new Parameter(ParametreName.beta, beta));


            double temp1 = beta * Math.Tan(0.5 * Math.PI * alpha);
            double temp2 = 1 / alpha;
            C1 = Math.Pow(1 + temp1 * temp1, 0.5 * temp2);
            C2 = Math.Atan(temp1) * temp2;
            C3 = 2 * Alpha / Math.PI;
            C4 = temp2;
        }

        public override double Simulate(Random r)
        {
            double U = Math.PI * (r.NextDouble() - 0.5);
            double E = -Math.Log(r.NextDouble());

            double x = 0.0, temp = 0.0;

            if (Alpha != 1)
            {
                var theta = (1.0 / Alpha) * Math.Atan(Beta * Math.Tan(Math.PI / 2 * Alpha));
                var angle = Alpha * (U + theta);
                var part1 = Beta * Math.Tan(Math.PI/2 * Alpha);

                var factor = Math.Pow(1.0 + (part1 * part1), 1.0 / (2.0 * Alpha));
                var factor1 = Math.Sin(angle) / Math.Pow(Math.Cos(U), 1.0 / Alpha);
                var factor2 = Math.Pow(Math.Cos(U - angle) / E, (1 - Alpha) / Alpha);

                return Mu + Sigma * (factor * factor1 * factor2);
            }
            else
            {
                var part1 = Math.PI/2 + (Beta * U);
                var summand = part1 * Math.Tan(U);
                var subtrahend = Beta * Math.Log(Math.PI / 2 * E * Math.Cos(U) / part1);

                return Mu + Sigma * 2/Math.PI * (summand - subtrahend);
            }
        }

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            throw new NotImplementedException();
        }

        public override double CDF(double x)
        {
            throw new NotImplementedException();
        }

        public override double ExpextedValue()
        {
            throw new NotImplementedException();
        }

        public override double Kurtosis()
        {
            throw new NotImplementedException();
        }

        public override double PDF(double x)
        {
            throw new NotImplementedException();
        }

        public override double Skewness()
        {
            throw new NotImplementedException();
        }

        public override double Variance()
        {
            throw new NotImplementedException();
        }
    }
}
