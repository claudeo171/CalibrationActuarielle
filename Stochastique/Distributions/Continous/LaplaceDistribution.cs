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
    public class LaplaceDistribution : Distribution
    {
        public LaplaceDistribution()
        {

        }
        public LaplaceDistribution(int mu, int beta)
        {
            AddParameter(new Parameter(ParametreName.mu, mu));
            AddParameter(new Parameter(ParametreName.beta, beta));
        }

        [IgnoreMember]
        public double Mu => GetParameter(ParametreName.mu).Value;

        [IgnoreMember]
        public double Beta => GetParameter(ParametreName.beta).Value;

        [IgnoreMember]
        public override TypeDistribution Type =>TypeDistribution.Laplace;

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            var mean = values.Mean();
            var variance = values.Variance();
            var beta = Math.Sqrt(variance / 2);
            var mu = mean;
            return new List<Parameter> { new Parameter(ParametreName.mu, mu), new Parameter(ParametreName.beta, beta) };
        }

        public override double CDF(double x)
        {
            if (x < Mu)
            {
                return 0.5 * Math.Exp((x - Mu) / Beta);
            }
            else
            {
                return 1 - 0.5 * Math.Exp((Mu - x) / Beta);
            }
        }

        public override double ExpextedValue()
        {
            return Mu;
        }

        public override double Kurtosis()
        {
            return 3;
        }

        public override double PDF(double x)
        {
            return 1 / (2 * Beta) * Math.Exp(-Math.Abs(x - Mu) / Beta);
        }

        public override double Skewness()
        {
            return 0;
        }

        public override double Variance()
        {
            return 2 * Beta * Beta;
        }

        public override double[] Simulate(Random r, int nbSimulations)
        {
            double[] result = new double[nbSimulations];
            for (int i = 0; i < nbSimulations; i++)
            {
                result[i]= Simulate(r);
            }
            return result;
        }
        public override double Simulate(Random r)
        {
            double result = 0;
            double u = r.NextDouble();
            if (u < 0.5)
            {
                result = Mu + Beta * Math.Log(2 * u);
            }
            else
            {
                result = Mu - Beta * Math.Log(2 - 2 * u);
            }
            return result;
        }

    }
}
