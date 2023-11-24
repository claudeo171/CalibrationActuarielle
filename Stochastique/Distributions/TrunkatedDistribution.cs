using MathNet.Numerics.Integration;
using MathNet.Numerics.RootFinding;
using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    [MessagePackObject]
    public class TrunkatedDistribution : Distribution
    {
        public TrunkatedDistribution() { }
        public TrunkatedDistribution(Distribution distrib)
        {
            BaseDistribution= distrib;
        }

        [Key(6)]
        public override bool CanComputeExpectedValueEasily => false;
        [IgnoreMember]
        public override bool CanComputeVarianceEasily => false;

        /// <summary>
        /// The distribution which is trunkated
        /// </summary>
        [Key(7)]
        public Distribution BaseDistribution { get; set; }

        /// <summary>
        /// Define if the distributtion is trunkated at is lower or upper bound
        /// </summary>
        [Key(8)]
        private double? ComputedExpectedValue { get; set; }
        [Key(9)]
        private double? ComputedVariance { get; set; }

        [Key(10)]
        public double QuantileUp => GetParameter(ParametreName.qUp).Value;

        [Key(11)]
        public double QuantileDown => GetParameter(ParametreName.qDown).Value;

        [Key(12)]
        public override TypeDistribution Type => (TypeDistribution)((int)TypeDistribution.Trunkated+(int)BaseDistribution.Type);

        public override double CDF(double x)
        {
            double baseCDF = BaseDistribution.CDF(x);
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

        [Key(13)]
        private double MinValue => Bisection.FindRoot((a) => CDF(a) - 1e-10, double.MinValue, double.MaxValue, maxIterations: 1000);

        [Key(14)]
        private double MaxValue => Bisection.FindRoot((a) => 1 - CDF(a) + 1e-10, double.MinValue, double.MaxValue, maxIterations: 1000);
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
                return BaseDistribution.PDF(x) / (QuantileUp - QuantileDown);
            }
        }

        public override double Variance()
        {
            if (ComputedVariance == null)
            {
                ComputedVariance = NewtonCotesTrapeziumRule.IntegrateAdaptive(x => x * x * PDF(x), MinValue, MaxValue, 1e-2) - ExpextedValue() * ExpextedValue();
            }
            return ComputedVariance.Value;
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            List<double> ll=new List<double>();
            List<List<double>> param = new List<List<double>>();
            BaseDistribution.Initialize(value, typeCalibration);
            var initialParameters = BaseDistribution.AllParameters().Select(a => a.Value).ToList();
            List<double> ratios = new List<double>() { 1, 0.1, 0.5, 0.9,  1.1, 1.5, 2,10 };
            AddParameter(new Parameter(ParametreName.qUp, 1));
            AddParameter(new Parameter(ParametreName.qDown, 0));
            foreach (var ratio in ratios)
            {
                var parametres = BaseDistribution.AllParameters().ToList();
                for(int i=0;i< parametres.Count; i++)
                {
                    parametres[i].Value = initialParameters[i] * ratio;
                }
                GetParameter(ParametreName.qUp).Value=1 ;
                GetParameter(ParametreName.qDown).Value = 0;
                try
                {
                    base.Initialize(value, typeCalibration);
                }
                catch
                {
                    continue;
                }
                
                ll.Add(GetLogLikelihood(value));
                param.Add(AllParameters().Select(a=>a.Value).ToList());
            }
            var newParam = param[ll.IndexOf(ll.Max())];
            var allParam = AllParameters().ToList();
            for (int i=0;i< allParam.Count; i++)
            {
                allParam[i].Value = newParam[i];
            }
            
        }

        public override Parameter GetParameter(ParametreName nomParametre)
        {
            if(ParametresParNom.ContainsKey(nomParametre))
            {
                return ParametresParNom[nomParametre];
            }
            else
            {
                return BaseDistribution.GetParameter(nomParametre);
            }
            
        }

        public override void SetParameter(double[] values)
        {
            for(int i=0;i< ParametresParNom.Count; i++)
            {
                ParametresParNom.ElementAt(i).Value.Value = values[i];
            }
            BaseDistribution.SetParameter(values.Skip(ParametresParNom.Count).ToArray());
        }

        public override IEnumerable<Parameter> AllParameters()
        {
            return base.AllParameters().Concat(BaseDistribution.AllParameters());
        }
    }
}
