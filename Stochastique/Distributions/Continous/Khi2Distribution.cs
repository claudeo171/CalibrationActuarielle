using Accord.Math;
using MathNet.Numerics;
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
    public class Khi2Distribution : Distribution
    {
        [Key(6)]
        public override TypeDistribution Type => TypeDistribution.Khi2;
        public Khi2Distribution()
        {
        }

        [Key(7)]
        public double K => GetParameter(ParametreName.k).Value;

        [IgnoreMember]
        public override double InconditionnalMinimumPossibleValue => 0;

        public Khi2Distribution(int k) : base()
        {
            AddParameter(new Parameter(ParametreName.k, k));
        }
        public override double CDF(double x)
        {
            if (x < 0)
            {
                return 0;
            }
            return Gamma.LowerIncomplete(GetParameter(ParametreName.k).Value / 2, x);
        }

        public override double InverseCDF(double p)
        {
            return Gamma.InverseLowerIncomplete(GetParameter(ParametreName.k).Value / 2, p);
        }

        public override double PDF(double x)
        {
            return Math.Exp(Math.Log(0.5) * K / 2 - SpecialFunctions.GammaLn(K / 2) * Math.Log(x) * (K / 2 - 1)  -x / 2);
        }

        public override double ExpextedValue()
        {
            return GetParameter(ParametreName.k).Value;
        }

        public override double Variance()
        {
            return ExpextedValue() * 2;
        }
        public override double Skewness()
        {
            return Math.Sqrt(8.0 / K);
        }

        public override double Kurtosis()
        {
            return 12.0 / K;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameters(CalibrateWithMoment(value));
            base.Initialize(value, typeCalibration);

            IntervaleForDisplay = new Intervale(Math.Max(0, K - 10 * K), K + 10 * K);
        }
        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            double k = 0;
            k = value.Sum() / value.Count();

            result.Add(new Parameter(ParametreName.k, k));
            return result;
        }
        public override double[] Simulate(Random r, int nbSimulations)
        {
            return new GammaDistribution(K/2.0,2).Simulate(r,nbSimulations);
        }
        public override double Simulate(Random r)
        {
            return Simulate(r, 1)[0];
        }
    }
}
