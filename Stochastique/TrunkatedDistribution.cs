using MathNet.Numerics.Integration;
using MathNet.Numerics.RootFinding;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public class TrunkatedDistribution : Distribution
    {
        public override bool CanComputeExpectedValueEasily => false;
        /// <summary>
        /// The distribution which is trunkated
        /// </summary>
        public Distribution BaseDistribution {  get; set; }
        /// <summary>
        /// Define if the distributtion is trunkated at is lower or upper bound
        /// </summary>
        private double? ComputedExpectedValue { get; set; }
        private double? ComputedVariance { get; set; }

        public double QuantileUp => GetParameter(ParametreName.qUp).Value;
        public double QuantileDown => GetParameter(ParametreName.qDown).Value;

        public override TypeDistribution Type => throw new NotImplementedException();

        public override double CDF(double x)
        {
            double baseCDF= BaseDistribution.CDF(x);
            if (baseCDF > QuantileUp)
            {
                return 1;
            }
            else if (baseCDF < QuantileDown)
            {
                return 0;
            }
            else
            {
                return (baseCDF - QuantileDown) / (QuantileUp - QuantileDown);
            }
        }

        private double MinValue=> Bisection.FindRoot((a) => CDF(a) - (1e-10), double.MinValue, double.MaxValue, maxIterations: 1000);
        private double MaxValue => Bisection.FindRoot((a) => 1 - CDF(a) + (1e-10), double.MinValue, double.MaxValue, maxIterations: 1000);
        public override double ExpextedValue()
        {
            if (ComputedExpectedValue == null)
            {
                var rst = NewtonCotesTrapeziumRule.IntegrateAdaptive(x => x * PDF(x), MinValue, MaxValue, 1e-2);
                ComputedExpectedValue = rst;
            }
            return ComputedExpectedValue.Value;

        }

        public override double PDF(double x)
        {
            double baseCDF = BaseDistribution.CDF(x);
            if (baseCDF > QuantileUp || baseCDF < QuantileDown)
            {
                return 0;
            }
            else
            {
                return PDF(x) / (QuantileUp - QuantileDown);
            }
        }

        public override double Variance()
        {
            if (ComputedVariance == null)
            {
                ComputedVariance = NewtonCotesTrapeziumRule.IntegrateAdaptive(x => x * x * PDF(x), MinValue, MaxValue, 1e-2) - ExpextedValue()* ExpextedValue();
            }
            return ComputedVariance.Value;
        }
    }
}
