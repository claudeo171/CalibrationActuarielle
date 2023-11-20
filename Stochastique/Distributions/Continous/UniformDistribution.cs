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
    public class UniformDistribution : Distribution
    {
        
        public UniformDistribution()
        {

        }

        [Key(6)]
        public double A => GetParameter(ParametreName.a).Value;

        [Key(7)]
        public double B => GetParameter(ParametreName.b).Value;

        [Key(8)]
        public override TypeDistribution Type => TypeDistribution.Uniform;

        public override double CDF(double x)
        {
            if (x < A)
            {
                return 0;
            }
            else if (x > B)
            {
                return 1;
            }
            else
            {
                return (x - A) / (B - A);
            }
        }

        public override double ExpextedValue()
        {
            return (B + A) / 2;
        }

        public override double PDF(double x)
        {
            if (x < A)
            {
                return 0;
            }
            else if (x > B)
            {
                return 0;
            }
            else
            {
                return 1 / (B - A);
            }
        }

        public override double Variance()
        {
            return (B - A) * (B - A) / 12;
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            if(variance * 192 -60*ev*ev>0)
            {
                AddParameter(new Parameter(ParametreName.b, Math.Max(value.Max(), (-2 * ev + Math.Sqrt(variance*192-60*ev*ev))/8)));
                AddParameter(new Parameter(ParametreName.a, Math.Min(value.Min(), 2*ev - GetParameter(ParametreName.b).Value)));
            }
            else
            {
                AddParameter(new Parameter(ParametreName.b, value.Max()));
                AddParameter(new Parameter(ParametreName.a, value.Min()));
            }

            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(variance));
        }

    }
}
