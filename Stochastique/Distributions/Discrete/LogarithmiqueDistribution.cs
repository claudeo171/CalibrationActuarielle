using MathNet.Numerics.Statistics;
using MathNet.Numerics;
using MathNet.Symbolics;
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
    }
}
