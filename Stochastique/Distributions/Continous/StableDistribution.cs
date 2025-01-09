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
        public double Beta => GetParameter(ParametreName.b).Value;
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

            if (Alpha == 1)
            {
                temp = 1 + C3 * U;
                x = temp * Math.Tan(U) - C3 * Math.Log(E * Math.Cos(U) / temp);
            }
            else
            {
                temp = Beta * (U + C2);
                x = C1 * Math.Sin(temp) * Math.Pow(Math.Cos(U), -C4) * Math.Pow(Math.Cos(U - temp) / E, C4 - 1);
            }

            return Mu + Sigma * x;
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
