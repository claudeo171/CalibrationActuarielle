using MathNet.Numerics.Statistics;
using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Stochastique.Distributions.Discrete
{
    [MessagePackObject]
    public class LogarithmiqueDistribution : DiscreteDistribution
    {
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameters(CalibrateWithMoment(value));

            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(Variance()));
        }
        public LogarithmiqueDistribution() { }
        public LogarithmiqueDistribution(double p)
        {
            AddParameter(new Parameter(ParametreName.p, p));
        }
        [IgnoreMember]
        public override TypeDistribution Type => TypeDistribution.Logarithmique;
        [IgnoreMember]
        public double P => GetParameter(ParametreName.p).Value;

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            List<Parameter> result = new List<Parameter>();
            var ev = Statistics.Mean(values);
            result.Add(new Parameter(ParametreName.p, OptimHelper.OptimHelper.GetOptimalParametre(ev, new Parameter(ParametreName.p, 0).MinValue, new Parameter(ParametreName.p, 0).MaxValue, (a) =>
            {
                return Math.Pow(ev +1 / Math.Log(1 - a) * a / (1 - a), 2);
            })));

            return result;
        }

        public override double ExpextedValue()
        {
            return -1 / Math.Log(1 - P) * P / (1 - P);
        }

        public override double Kurtosis()
        {
            return double.NaN;
        }

        public override double Skewness()
        {
            return double.NaN;
        }

        public override double Variance()
        {
            return -P*(P+Math.Log(1-P))/((1-P)* (1 - P)/Math.Pow(Math.Log(1-P), 2));
        }

        protected override double PDFInt(int k)
        {
            if(k<=0)
            {
                return 0;
            }
            return -1 / Math.Log(1 - P) * Math.Pow(P, k) / k;
        }

        public override double CDF(double k)
        {
            return SpecialFunctions.BetaIncomplete((int)k+1,0.000000001,P)/Math.Log(1-P);
        }
        public override double Simulate(Random r)
        {
            var u = r.NextDouble();
            var v = r.NextDouble();
            return Math.Truncate(1 + (Math.Log(v) / Math.Log(1 - Math.Pow(1 - P, u))));
        }

    }
}
