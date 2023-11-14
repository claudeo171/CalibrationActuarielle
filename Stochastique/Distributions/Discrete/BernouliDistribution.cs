using MathNet.Numerics.Statistics;
using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    public class BernouliDistribution : DiscreteDistribution
    {
        private double P => GetParameter(ParametreName.p).Value;
        public override TypeDistribution Type => TypeDistribution.Bernouli;
        protected override double MaxValue => 1;

        public override double ExpextedValue()
        {
            return P;
        }

        protected override double PDFInt(int x)
        {
            if (x == 0)
            {
                return 1 - P;
            }
            else
            {
                return P;
            }
        }

        public override double Variance()
        {
            return P * (1 - P);
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);

            AddParameter(new Parameter(ParametreName.p, Math.Min(1,Math.Max(0,ev))));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(Variance()));
        }
    }
}
