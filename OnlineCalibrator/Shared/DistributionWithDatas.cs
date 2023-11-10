using Stochastique.Distributions;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public class DistributionWithDatas
    {
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

        }
        public TypeDistribution TypeDistribution { get; set; }
        public Distribution Distribution { get; set; }
        public TypeCalibration Calibration { get; set; }
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
        public double SeuilAlpha { get; set; } = 0.05;
        public string? Comment { get; set; }
        public void UpdateTest()
        {
            foreach (var test in TestStatistiques)
            {
                ResultatTest[test] = test.GetTypeDonnee(SeuilAlpha);
            }
        }
        public void RAZTest()
        {
            foreach (var test in TestStatistiques)
            {
                ResultatTest[test] = TypeDonnees.NA;
            }
        }

        public List<TestStatistique> TestStatistiques { get; set; }
        public Dictionary<TestStatistique, TypeDonnees> ResultatTest { get; set; }
    }
}
