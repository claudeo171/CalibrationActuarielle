using LiveChartsCore.Defaults;
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
    public class NormalDistribution : Distribution
    {
        [Key(6)]
        public override TypeDistribution Type => TypeDistribution.Normal;
        public NormalDistribution()
        {
        }

        public override double CDF(double x)
        {
            return 0.5 * (1 + SpecialFunctions.Erf((x - GetParameter(ParametreName.mu).Value) / (GetParameter(ParametreName.sigma).Value * Constants.Sqrt2)));
        }

        public override double InverseCDF(double x)
        {
            return SpecialFunctions.ErfInv(2 * x - 1) * GetParameter(ParametreName.sigma).Value * Constants.Sqrt2 + GetParameter(ParametreName.mu).Value;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double mu = 0;
            double sigma = 0;
            mu = value.Sum() / value.Count();
            sigma = Math.Sqrt(value.Sum(a => a * a) / value.Count() - mu * mu);
            AddParameter(new Parameter(ParametreName.mu, mu));
            AddParameter(new Parameter(ParametreName.sigma, sigma));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(mu - 5 * sigma, mu + 5 * sigma);

        }

        public override double PDF(double x)
        {
            return Math.Exp(-(x - GetParameter(ParametreName.mu).Value) * (x - GetParameter(ParametreName.mu).Value) / (2 * GetParameter(ParametreName.sigma).Value * GetParameter(ParametreName.sigma).Value)) / (Math.Sqrt(Math.PI * 2) * GetParameter(ParametreName.sigma).Value);
        }

        public override double ExpextedValue()
        {
            return GetParameter(ParametreName.mu).Value;
        }

        public override double Variance()
        {
            return GetParameter(ParametreName.sigma).Value * GetParameter(ParametreName.sigma).Value;
        }

        public override double Simulate(Random r)
        {
            var u1 = r.NextDouble();
            var u2 = r.NextDouble();
            return Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        }
        public override double[] Simulate(Random r, int n)
        {
            double[] result = new double[n];
            for (int i = 0; i < n/2 +1; i++)
            {
                var u1 = r.NextDouble();
                var u2 = r.NextDouble();
                result[2*i] = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
                if(2*i+1<n)
                {
                    result[2 * i + 1] = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);
                }
            }
            return result;
        }
    }
}
