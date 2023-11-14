using MathNet.Numerics.Statistics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class CauchyDistribution : Distribution
    {
        public double A => GetParameter(ParametreName.a).Value;
        public double B => GetParameter(ParametreName.b).Value;
        public override TypeDistribution Type => TypeDistribution.Cauchy;

        public override double CDF(double x)
        {
            return 1 / Math.PI * Math.Atan((x-B)/A) +0.5;
        }

        public override double ExpextedValue()
        {
            return double.NaN;
        }

        public override double PDF(double x)
        {
            return A / (Math.PI * (A * A + (x - B) * (x - B)));
        }

        public override double Variance()
        {
            return double.NaN;
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double k = 0;
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            alglib.complex[] rst = new alglib.complex[3];
            var repot = new alglib.polynomialsolver.polynomialsolverreport();
            alglib.xparams xparams = new alglib.xparams(1);
            alglib.polynomialsolver.polynomialsolve(new double[4] { 0, ev * (ev - 1), variance * (4 * ev * ev * ev - 8 * ev * ev + 5 * ev + ev - 1), variance * (8 * ev * ev * ev - 12 * ev * ev + 6 * ev - 1) }, 3, ref rst, repot, xparams);

            AddParameter(new Parameter(ParametreName.k, k));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(Math.Max(0, k - 10 * k), k + 10 * k);
            base.Initialize(value, typeCalibration);
        }
    }
}
