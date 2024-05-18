using Accord.Statistics.Distributions.Univariate;
using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class ParetoDistribution : Distribution
    {
        public override TypeDistribution Type => TypeDistribution.Pareto;

        public double K => GetParameter(ParametreName.k).Value;
        public double XM => GetParameter(ParametreName.xm).Value;
        [IgnoreMember]
        public override double InconditionnalMinimumPossibleValue => 0;

        public override double CDF(double x)
        {
            if (x < XM)
            {
                return 0;
            }
            else
            {
                return 1-Math.Pow(XM/x,K);
            }
        }

        public override double ExpextedValue()
        {
            return K * XM / (K - 1);
        }

        public override double Kurtosis()
        {
            if (K > 4)
            {
                return 6 * (K * K * K + K * K - 6 * K - 2) / (K * (K - 3) * (K - 4));
            }
            else
            {
                return double.NaN;
            }
        }

        public override double PDF(double x)
        {
            if (x < XM)
            {
                return 0;
            }
            else
            {
                return K * Math.Pow(XM, K) / Math.Pow(x, K + 1);
            }
        }

        public override double Skewness()
        {
            if (K > 3)
            {
                return 2 * (1 + K) / (K - 3) * Math.Sqrt((K - 2) / K);
            }
            else
            {
                return double.NaN;
            }
        }

        public override double Variance()
        {
            if (K > 2)
            {
                return XM * XM * K / ((K - 2) * (K - 1) * (K - 1));
            }
            else
            {
                return double.NaN;
            }
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameter(new Parameter(ParametreName.k, 1+Math.Sqrt(1+Math.Pow(ExpextedValue(),2)/Variance())));
            AddParameter(new Parameter(ParametreName.xm, ExpextedValue() * (K - 1) / K));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(XM, XM + 15 * Math.Sqrt(Variance()));

        }

        public override double[] Simulate(Random r, int nbSimulations)
        {
            double[] result = new double[nbSimulations];
            for (int i = 0; i < nbSimulations; i++)
                result[i] = XM / Math.Pow(r.NextDouble(), 1.0 / K);
            return result;
        }
        public override double Simulate(Random r)
        {
            return base.Simulate(r, 1)[0];
        }
    }
}
