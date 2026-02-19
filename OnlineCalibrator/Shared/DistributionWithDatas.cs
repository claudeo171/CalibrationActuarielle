using Accord.Statistics.Testing;
using Stochastique.Distributions;
using Stochastique.Enums;
using Stochastique.Test;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineCalibrator.Shared
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class DistributionWithDatas
    {
        [MemoryPack.MemoryPackConstructor]
        public DistributionWithDatas()
        {
        }
        public DistributionWithDatas(Distribution distribution, double[] data)
        {
            Distribution = distribution;
            TestStatistiques = TestStatistique.GetTestsDistribution(Distribution, data);
            ResultatTest = new Dictionary<TestStatistique, TypeDonnees>();
            foreach (var v in TestStatistiques)
            {
                ResultatTest.Add(v, TypeDonnees.NA);
            }
            UpdateTest();
            LogLikelihood = distribution.GetLogLikelihood(data);
            N=data.Length;
        }

        [MemoryPack.MemoryPackOrder(0)]
        public double N { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public double LogLikelihood { get; set; }

        [MemoryPack.MemoryPackOrder(2)]
        public double AIC=> 2*Distribution.NumberOfParameter - 2*LogLikelihood;

        [MemoryPack.MemoryPackOrder(3)]
        public double BIC => Math.Log(N) * Distribution.NumberOfParameter - 2 * LogLikelihood;


        [MemoryPack.MemoryPackOrder(5)]
        public Distribution Distribution { get; set; }
        [MemoryPack.MemoryPackOrder(6)]
        public TypeCalibration Calibration { get; set; }

        [MemoryPack.MemoryPackOrder(7)]
        public string SeuilAlphaString
        {
            get
            {
                return SeuilAlpha.ToString();
            }
            set
            {
                try
                {
                    if (value != null)
                    {
                        SeuilAlpha = value.Contains('.') ? Convert.ToDouble(value, new CultureInfo("en-US")) : Convert.ToDouble(value, new CultureInfo("fr-FR"));
                        UpdateTest();
                    }
                }
                catch
                {
                    RAZTest();
                }

            }
        }

        [MemoryPack.MemoryPackOrder(8)]
        public double SeuilAlpha { get; set; } = 0.05;
        [MemoryPack.MemoryPackOrder(9)]
        public string? Comment { get; set; }

        public void UpdateTest()
        {
            if (TestStatistiques != null)
            {
                foreach (var test in TestStatistiques)
                {
                    ResultatTest[test] = test.GetTypeDonnee(SeuilAlpha);
                }
            }
        }
        public void RAZTest()
        {
            if (TestStatistiques != null)
            {
                foreach (var test in TestStatistiques)
                {
                    ResultatTest[test] = TypeDonnees.NA;
                }
            }
        }

        [MemoryPack.MemoryPackOrder(10)]
        public List<TestStatistique> TestStatistiques { get; set; }
        [MemoryPack.MemoryPackOrder(11)]
        public Dictionary<TestStatistique, TypeDonnees> ResultatTest { get; set; }

        [MemoryPack.MemoryPackOrder(12)]
        public double ProbabiliteMachineLearningImage { get; set; }
        [MemoryPack.MemoryPackIgnore]
        public EELQuantileTest EELQuantileTest => TestStatistiques?.FirstOrDefault(a => a is EELQuantileTest) as EELQuantileTest;
    }
}
