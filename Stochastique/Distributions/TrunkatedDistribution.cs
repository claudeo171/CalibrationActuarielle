using Accord.Math;
using Accord.Statistics;
using MathNet.Numerics.Integration;
using MathNet.Numerics.RootFinding;
using MathNet.Numerics.Statistics;
using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static alglib;

namespace Stochastique.Distributions
{
    [MessagePackObject]
    public class TrunkatedDistribution : Distribution
    {

        public TrunkatedDistribution() { }
        public TrunkatedDistribution(Distribution? distrib)
        {
            BaseDistribution = distrib;
        }
        public TrunkatedDistribution(Distribution d, double valeurMin, double valeurMax)
        {
            BaseDistribution = d;
            ValeurMax = valeurMax;
            ValeurMin = valeurMin;
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

        [Key(10)]
        public double ValeurMin
        {
            get
            {
                return GetParameter(ParametreName.valeurMin).Value;
            }
            set 
            {
                if (!ParametresParNom.ContainsKey(ParametreName.valeurMin) )
                {
                    AddParameter(new Parameter(ParametreName.valeurMin, value));
                }
                else
                {
                    GetParameter(ParametreName.valeurMin).Value = value;
                }
            }
        }

        [Key(11)]
        public double ValeurMax
        {
            get
            {
                return GetParameter(ParametreName.ValeurMax).Value;
            }
            set
            {
                if (!ParametresParNom.ContainsKey(ParametreName.ValeurMax))
                {
                    AddParameter(new Parameter(ParametreName.ValeurMax, value));
                }
                else
                {
                    GetParameter(ParametreName.ValeurMax).Value = value;
                }
            }
        }
        [IgnoreMember]
        public double QuantileUp => BaseDistribution.CDF(ValeurMax);
        [IgnoreMember]
        public double QuantileDown => BaseDistribution.CDF(ValeurMin);

        [Key(12)]
        public override TypeDistribution Type => (TypeDistribution)((int)TypeDistribution.Trunkated + (int)BaseDistribution.Type);
        [IgnoreMember]
        public override double InconditionnalMaximumPossibleValue => BaseDistribution.InconditionnalMaximumPossibleValue;
        [IgnoreMember]
        public override double InconditionnalMinimumPossibleValue => BaseDistribution.InconditionnalMinimumPossibleValue;
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
                return (baseCDF - QuantileDown).Divide(QuantileUp - QuantileDown, 1);
            }
        }


        [IgnoreMember]
        public double[] SimulatedValue { get; set; }
        public void SimulateValue()
        {
            if (SimulatedValue == null)
            {

                SimulatedValue = Simulate(new Random(3433), 100000);
            }
        }
        public override double ExpextedValue()
        {
            if (QuantileUp - QuantileDown < 1)
            {
                return double.NaN;
            }
            else
            {
                SimulateValue();
                return SimulatedValue.Mean();
            }
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
                if (QuantileUp - QuantileDown < 1e-10)
                {
                    return 0;
                }
                return BaseDistribution.PDF(x) / (QuantileUp - QuantileDown);
            }
        }

        public override double Variance()
        {
            if (QuantileUp - QuantileDown < 1)
            {
                return double.NaN;
            }
            else
            {
                SimulateValue();
                return SimulatedValue.Variance();
            }
        }
        public override double Skewness()
        {
            if (QuantileUp - QuantileDown < 1)
            {
                return double.NaN;
            }
            else
            {
                SimulateValue();
                return SimulatedValue.Skewness();
            }
        }
        public override double Kurtosis()
        {
            if (QuantileUp - QuantileDown < 1)
            {
                return double.NaN;
            }
            else
            {
                SimulateValue();
                return SimulatedValue.Kurtosis();
            }
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            List<double> ll = new List<double>();
            var mean = value.Mean();
            var ecartType = value.StandardDeviation();
            var min = value.Min();
            var max = value.Max();
            List<List<double>> param = new List<List<double>>();
            BaseDistribution.Initialize(value, typeCalibration);
            var initialParameters = BaseDistribution.AllParameters().Select(a => a.Value).ToList();
            List<(double, double)> ratios = new List<(double, double)>()
            {
                (-ecartType,3),
                (ecartType,3),
                (-2*ecartType,5),
                (2*ecartType,5),
                (0,2),
                (0,4),
            };
            foreach (var ratio in ratios)
            {
                var parametres = BaseDistribution.GetParameterValues(value, ratio.Item1, ratio.Item2);
                for (int i = 0; i < parametres.Length; i++)
                {
                    BaseDistribution.AllParameters().ElementAt(i).SetValue(parametres[i]);

                }
                try
                {
                    RAZComputedMoment();
                    base.Initialize(value, typeCalibration);
                }
                catch
                {
                    continue;
                }

                ll.Add(GetLogLikelihood(value));
                param.Add(AllParameters().Select(a => a.Value).ToList());
            }
            if (param.Count > 0 && ll.Any(a => !double.IsNaN(a)))
            {
                var newParam = param[ll.IndexOf(ll.Max())];
                var allParam = AllParameters().ToList();
                for (int i = 0; i < allParam.Count; i++)
                {
                    allParam[i].SetValue(newParam[i]);
                }
            }
            else
            {
                var parametres = BaseDistribution.AllParameters().ToList();
                for (int i = 0; i < parametres.Count; i++)
                {
                    parametres[i].SetValue(initialParameters[i]);
                }
            }
            RAZComputedMoment();
        }

        private void RAZComputedMoment()
        {
            SimulatedValue = null;
        }

        public override double[] Simulate(Random r, int nbSimulations)
        {
            double[] rst = new double[nbSimulations];
            int i = 0;
            while (i < nbSimulations)
            {
                var baseSimulated = BaseDistribution.Simulate(r);
                if (baseSimulated >= ValeurMin && baseSimulated <= ValeurMax)
                {
                    rst[i] = baseSimulated;
                    i++;
                }
            }
            return rst;
        }

        public override Parameter GetParameter(ParametreName nomParametre)
        {
            if (ParametresParNom.ContainsKey(nomParametre))
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
            BaseDistribution.SetParameter(values);
            RAZComputedMoment();
        }

        public override IEnumerable<Parameter> AllParameters()
        {
            return base.AllParameters().Concat(BaseDistribution.AllParameters());
        }

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            var param = BaseDistribution.CalibrateWithMoment(values).ToList();
            return param;
        }
    }
}
