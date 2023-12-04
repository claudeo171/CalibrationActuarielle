using MessagePack;
using Stochastique.Distributions;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public class DistributionWithDatas
    {
        public DistributionWithDatas()
        {
        }
        public DistributionWithDatas(Distribution distribution, double[] data)
        {
            Distribution = distribution;
            TestStatistiques = TestStatistique.GetTestsDistribution(Distribution.Type, data);
            ResultatTest = new Dictionary<TestStatistique, TypeDonnees>();
            foreach (var v in TestStatistiques)
            {
                ResultatTest.Add(v, TypeDonnees.NA);
            }
            UpdateTest();
            LogLikelihood = distribution.GetLogLikelihood(data);
            N=data.Length;
        }

        [Key(0)]
        public double N { get; set; }
        [Key(1)]
        public double LogLikelihood { get; set; }

        [Key(2)]
        public double AIC=> 2*Distribution.AllParameters().Count()-2*LogLikelihood;

        [Key(3)]
        public double BIC => Math.Log(N) * Distribution.AllParameters().Count() - 2 * LogLikelihood;


        [Key(5)]
        public Distribution Distribution { get; set; }
        [Key(6)]
        public TypeCalibration Calibration { get; set; }

        [Key(7)]
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
                    SeuilAlpha = value.Contains('.') ? Convert.ToDouble(value, new CultureInfo("en-US")) : Convert.ToDouble(value, new CultureInfo("fr-FR"));
                    UpdateTest();
                }
                catch
                {
                    RAZTest();
                }

            }
        }

        [Key(8)]
        public double SeuilAlpha { get; set; } = 0.05;
        [Key(9)]
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

        [Key(10)]
        public List<TestStatistique> TestStatistiques { get; set; }
        [Key(11)]
        public Dictionary<TestStatistique, TypeDonnees> ResultatTest { get; set; }
    }
}
