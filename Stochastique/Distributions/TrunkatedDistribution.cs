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
    public partial class TrunkatedDistribution : Distribution
    {

        public TrunkatedDistribution() { }
        public TrunkatedDistribution(Distribution? distrib)
        {
            BaseDistribution = distrib;
        }
        public TrunkatedDistribution(Distribution d, double valeurMin,double valeurMax)
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
        [Key(8)]
        private double? ComputedExpectedValue { get; set; }
        [Key(9)]
        private double? ComputedVariance { get; set; }
        [Key(10)]
        public double ValeurMin { get; set; }

        [Key(11)]
        public double ValeurMax { get; set; }
        [IgnoreMember]
        public double QuantileUp => BaseDistribution.CDF(ValeurMax);
        [IgnoreMember]
        public double QuantileDown => BaseDistribution.CDF(ValeurMin);
        [IgnoreMember]
        public override int NumberOfParameter => AllParameters().Count()+2;

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
                return (baseCDF - QuantileDown).Divide(QuantileUp - QuantileDown,1);
            }
        }

        [Key(13)]
        private double MinValue
        {
            get
            {
                if (Type == TypeDistribution.TrunkatedBeta)
                {
                    return 0;
                }
                else
                {
                    double root = 0;
                    var rst= Bisection.TryFindRoot((a) => CDF(a) - 1e-10, GetMin(), GetMax(), accuracy: 1e-8, maxIterations: 1000,out root);
                    if (rst)
                    {
                        return root;
                    }
                    else
                    {
                        return GetMin();
                    }
                }
            }
        }

        private double GetMin()
        {
            double d = -1;
            while (CDF(d) > 1e-10)
            {
                d *= 10;
            }
            return d;
        }

        private double GetMax()
        {
            double d = 1;
            while (1 - CDF(d) > 1e-10)
            {
                d *= 10;
            }
            return d;
        }

        [Key(14)]
        private double MaxValue
        {
            get
            {
                if (Type == TypeDistribution.TrunkatedBeta)
                {
                    return 1;
                }
                else
                {
                    double root = 0;
                    var rst=Bisection.TryFindRoot((a) => 1 - CDF(a) - 1e-10, GetMin(), GetMax(), accuracy: 1e-8, maxIterations: 1000,out root);
                    if (rst)
                    {
                        return root;
                    }
                    else
                    {
                        return GetMax();
                    }
                }
            }
        }
        [Key(15)]
        private double? ComptedSkewness { get; set; }
        [Key(16)]
        private double? ComputedKurtosis { get; set; }

        [Key(17)]
        private double[] SimulatedValue { get; set; }
        public void SimulateValue()
        {
            if (SimulatedValue == null)
            {
                
                SimulatedValue = Simulate(new Random(3433), 100000);
            }
        }
        public override double ExpextedValue()
        {
            if (ComputedExpectedValue.HasValue)
            {
                return ComputedExpectedValue.Value;
            }
            else
            {
                if (QuantileUp - QuantileDown < 1)
                {
                    return double.NaN;
                }
                else
                {
                    SimulateValue();
                    ComputedExpectedValue = SimulatedValue.Mean();
                    return ComputedExpectedValue.Value;
                }
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
                if(QuantileUp - QuantileDown< 1e-10)
                {
                    return 0;
                }
                return BaseDistribution.PDF(x) / (QuantileUp - QuantileDown);
            }
        }

        public override double Variance()
        {
            if(ComputedVariance.HasValue)
            {                
                return ComputedVariance.Value;
            }
            else
            {
                if (QuantileUp - QuantileDown < 1)
                {
                    return double.NaN;
                }
                else
                {
                    SimulateValue();
                    ComputedVariance = SimulatedValue.Variance();
                    return ComputedVariance.Value;
                }
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
            ValeurMin=value.Min();
            ValeurMax=value.Max();
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
                var parametres = BaseDistribution.GetParameterValues(value,ratio.Item1,ratio.Item2);
                for (int i = 0; i < parametres.Length; i++)
                {
                    BaseDistribution.AllParameters().ElementAt(i).SetValue( parametres[i]);

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
                    allParam[i].SetValue( newParam[i]);
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
            ComptedSkewness = null;
            ComputedExpectedValue = null;
            ComputedKurtosis = null;
            ComputedVariance = null;
        }

        public override double[] Simulate(Random r, int nbSimulations)
        {
            double[] rst = new double[nbSimulations];
            int i = 0;
            while(i<nbSimulations)
            {
                var baseSimulated = BaseDistribution.Simulate(r);
                if(baseSimulated >= ValeurMin && baseSimulated <= ValeurMax)
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
            for (int i = 0; i < ParametresParNom.Count; i++)
            {
                ParametresParNom.ElementAt(i).Value.SetValue(values[i]);
            }
            BaseDistribution.SetParameter(values.Skip(ParametresParNom.Count).ToArray());
            RAZComputedMoment();
        }

        public override IEnumerable<Parameter> AllParameters()
        {
            return base.AllParameters().Concat(BaseDistribution.AllParameters());
        }

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            var param=BaseDistribution.CalibrateWithMoment(values).ToList();
            return param;
        }
    }
}
