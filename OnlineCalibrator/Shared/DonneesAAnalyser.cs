using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using MathNet.Numerics.Statistics;
using MessagePack;
using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineCalibrator.Shared.MachineLearning;
using Stochastique;
using Stochastique.Distributions;
using Stochastique.Enums;
using Stochastique.Test;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public class DonneesAAnalyser
    {
        [Key(0)]
        public string? Name { get; set; }
        [Key(1)]
        public double[]? Values { get; set; }

        [Key(2)]
        public Point[]? PointsKDE { get; set; }
        [Key(3)]
        public Point[]? PointsCDF { get; set; }

        [Key(4)]
        public double Moyenne => Values?.Average() ?? 0;

        [Key(5)]
        public double Variance => Values == null ? 0 : Values.Select(a => a * a).Mean() - Moyenne * Moyenne;

        [Key(6)]
        public double Kurtosis => Values == null ? 0 : Statistics.Kurtosis(Values);

        [Key(7)]
        public double Skewness => Values == null ? 0 : Statistics.Skewness(Values);

        [Key(8)]
        public Dictionary<TestStatistique, bool>? ResultatStatistique { get; set; }

        /// <summary>
        /// List of distribution with datas. Only one element for each distribution type.
        /// </summary>
        [Key(9)]
        public List<DistributionWithDatas> Distributions { get; set; } = new List<DistributionWithDatas>();

        [Key(10)]
        public bool IsDiscreteDistribution { get; set; }
        [Key(11)]
        public bool IncludeTrunkatedDistributions { get; set; }
        [Key(13)]
        public MethodeCalibrationRetenue MethodeCalibration { get; set; }
        [Key(12)]
        public Distribution CalibratedDistribution { get; set; }

        private double valeurMinTrukated;
        private double valeurMaxTrukated;
        [Key(15)]
        public double ValeurMinTrukated
        {
            get => valeurMinTrukated;
            set
            {
                valeurMinTrukated = Math.Min(value, Values.Min());
                if (Distributions != null)
                {
                    foreach (var distribution in Distributions.Where(a => a.Distribution is TrunkatedDistribution))
                    {
                        ((TrunkatedDistribution)distribution.Distribution).ValeurMin = valeurMinTrukated;
                    }
                }
            }
        }
        [Key(16)]
        public double ValeurMaxTrukated
        {
            get => valeurMaxTrukated;
            set
            {
                valeurMaxTrukated = Math.Max(value, Values.Max());
                if (Distributions != null)
                {
                    foreach (var distribution in Distributions.Where(a => a.Distribution is TrunkatedDistribution))
                    {
                        ((TrunkatedDistribution)distribution.Distribution).ValeurMax = valeurMaxTrukated;
                    }
                }
            }
        }
        [IgnoreMember]
        public ITransformer? Model { get; set; }
        [IgnoreMember]
        public ConfusionMatrix? ConfusionMatrixML { get; set; }
        public void MajCalibrationTronque()
        {
            foreach (var distribution in Distributions.Where(a => a.Distribution is TrunkatedDistribution))
            {
                ((TrunkatedDistribution)distribution.Distribution).Initialize(Values, TypeCalibration.MaximumLikelyhood);
            }
        }

        [IgnoreMember]
        public TypeDistribution? CalibratedTypeDistribution
        {
            get
            {
                return CalibratedDistribution?.Type;
            }
            set
            {
                CalibratedDistribution = VisisbleData.FirstOrDefault(a => a.Distribution.Type == value)?.Distribution;

            }
        }
        [IgnoreMember]
        public DistributionWithDatas CurrentDistribution => Distributions.FirstOrDefault(a => a.Distribution.Type == CalibratedTypeDistribution);

        public DonneesAAnalyser() { }
        public void Initialize()
        {
            ValeurMinTrukated = Values.Min();
            ValeurMaxTrukated = Values.Max();
            PointsCDF = GenerationGraphique.GetCDF(Values);
            PointsKDE = GenerationGraphique.GetDensity(Values, 100);
        }


        public void CalculerTest()
        {

        }
        public List<Point[]> GetPPPlot(TypeDistribution? typeDistribution = null)
        {
            List<Point[]> rst = new List<Point[]>();
            rst.Add(new Point[Values.Length]);
            rst.Add(new Point[Values.Length]);
            Distribution loi;
            if (typeDistribution == null)
            {
                loi = CalibratedDistribution;
            }
            else
            {
                loi = GetDistribution(typeDistribution.GetValueOrDefault(), null).Distribution;
            }

            int i = 0;
            foreach (var elts in Values.Order())
            {
                double x = (i + 0.5) / Values.Length;
                double y = loi.CDF(elts);
                rst[1][i] = new Point() { X = x, Y = x };
                rst[0][i] = new Point() { X = x, Y = y };
                i++;
            }
            return rst;
        }
        public List<Point[]> GetCDFPDF(TypeDistribution? typeDistribution = null)
        {
            List<Point[]> rst = new List<Point[]>();
            rst.Add(new Point[101]);
            rst.Add(new Point[101]);
            Distribution loi;
            if (typeDistribution == null)
            {
                loi = CalibratedDistribution;
            }
            else
            {
                loi = GetDistribution(typeDistribution.GetValueOrDefault(), null).Distribution;
            }

            double min = Values.Min();
            double max = Values.Max();

            for (int i = 0; i <= 100; i++)
            {
                double x = min + i * (max - min) / 100;
                rst[1][i] = new Point() { X = x, Y = loi.PDF(x) };
                rst[0][i] = new Point() { X = x, Y = loi.CDF(x) };
            }

            return rst;
        }

        public Point[] GetQuantileBetaPlot()
        {
            Point[] rst = new Point[Values.Length];
            Distribution loi;
            var values = Values.Order().ToArray();
            var betaTest = CurrentDistribution.TestStatistiques.FirstOrDefault(a => a.TypeTestStatistique == TypeTestStatistique.BetaQuantile) as BetaQuantileTest;
            for (int i = 0; i < Values.Length; i++)
            {
                rst[i] = new Point() { X = values[i], Y = betaTest.PValues[i] };

            }
            return rst;
        }
        public Point[] GetQuantilePlot()
        {
            Point[] rst = new Point[Values.Length];
            Distribution loi;
            var values = Values.Order().ToArray();
            for (int i = 0; i < Values.Length; i++)
            {
                rst[i] = new Point() { X = values[i], Y = CurrentDistribution.EELQuantileTest.PValues[i] };

            }
            return rst;
        }
        public List<Point[]> GetQQPlot(TypeDistribution? typeDistribution = null)
        {
            List<Point[]> rst = new List<Point[]>();
            rst.Add(new Point[Values.Length]);
            rst.Add(new Point[Values.Length]);
            Distribution loi;
            if (typeDistribution == null)
            {
                loi = CalibratedDistribution;
            }
            else
            {
                loi = GetDistribution(typeDistribution.GetValueOrDefault(), null).Distribution;
            }

            int i = 0;
            foreach (var elts in Values.Order())
            {
                double x = elts;
                double y = loi.InverseCDF((i + 0.5) / Values.Length);

                rst[1][i] = i < Values.Length / 2 ? new Point() { X = Math.Min(x, y), Y = Math.Min(x, y) } : new Point() { X = Math.Max(x, y), Y = Math.Max(x, y) };
                rst[0][i] = new Point() { X = x, Y = y };
                i++;
            }
            return rst;
        }

        public void AddMonteCarloTest()
        {
            if (CalibratedDistribution != null)
            {
                VisisbleData.FirstOrDefault(a => a.Distribution.Type == CalibratedTypeDistribution).TestStatistiques.Add(new EELQuantileTest(Values, CalibratedDistribution, 0.995));
            }
        }

        [Key(14)]
        public List<DistributionWithDatas> VisisbleData { get; set; }

        public List<DistributionWithDatas> GetAllDistributions()
        {
            var distributions = Enum.GetValues(typeof(TypeDistribution)).Cast<TypeDistribution>().Where(a => Distribution.CreateDistribution(a) != null && Distribution.CreateDistribution(a).IsDiscreet == IsDiscreteDistribution).ToList();
            var rst = distributions.Select(a => GetDistribution(a, TypeCalibration.MaximumLikelyhood)).ToList();

            if (IncludeTrunkatedDistributions)
            {
                rst.AddRange(distributions.Where(a => Distribution.CreateDistribution(a).IsTrunkable).Select(a => GetDistribution(a, TypeCalibration.MaximumLikelyhood, true)));
            }
            VisisbleData = rst;
            return rst;

        }


        public DistributionWithDatas GetDistribution(TypeDistribution typeDistribution, TypeCalibration? calibration, bool isTrunkated = false)
        {
            var distrib = Distribution.CreateDistribution(typeDistribution);
            if (isTrunkated)
            {
                distrib = new TrunkatedDistribution(distrib);
            }
            if (calibration != null && !Distributions.Any(a => a.Distribution.Type == distrib.Type && a.Calibration == calibration))
            {
                distrib.Initialize(Values, calibration.GetValueOrDefault());
                if (Distributions.Any(a => a.Distribution.Type == distrib.Type))
                {
                    Distributions.First(a => a.Distribution.Type == distrib.Type).Distribution = distrib;
                    Distributions.First(a => a.Distribution.Type == distrib.Type).Calibration = calibration.GetValueOrDefault();
                }
                else
                {
                    Distributions.Add(new DistributionWithDatas(distrib, Values) { Calibration = calibration.Value });
                }
            }
            else if (calibration == null && !Distributions.Any(a => distrib != null && a.Distribution.Type == distrib.Type || distrib == null && a.Distribution.Type == typeDistribution))
            {
                return GetDistribution(typeDistribution, default(TypeCalibration));
            }
            return Distributions.First(a => distrib != null && a.Distribution.Type == distrib.Type || distrib == null && a.Distribution.Type == typeDistribution);
        }
        public void ChangeSelectionMethod(MethodeCalibrationRetenue m)
        {
            MethodeCalibration = m;
            if (VisisbleData != null)
            {
                switch (m)
                {
                    case MethodeCalibrationRetenue.AIC:
                        CalibratedDistribution = VisisbleData.Where(a => !double.IsNaN(a.AIC)).OrderBy(a => a.AIC).First().Distribution;
                        break;
                    case MethodeCalibrationRetenue.BIC:
                        CalibratedDistribution = VisisbleData.Where(a => !double.IsNaN(a.BIC)).OrderBy(a => a.BIC).First().Distribution;
                        break;
                    case MethodeCalibrationRetenue.Vraisemblance:
                        CalibratedDistribution = VisisbleData.Where(a => !double.IsNaN(a.LogLikelihood)).OrderBy(a => -a.LogLikelihood).First().Distribution;
                        break;
                    case MethodeCalibrationRetenue.MachineLearningImage:
                        CalibratedDistribution = VisisbleData.Where(a => !double.IsNaN(a.LogLikelihood)).OrderBy(a => -a.ProbabiliteMachineLearningImage).First().Distribution;
                        break;
                    case MethodeCalibrationRetenue.KSTest:
                        CalibratedDistribution = VisisbleData.Where(a => !double.IsNaN(a.TestStatistiques.FirstOrDefault(a => a.TypeTestStatistique == TypeTestStatistique.KolmogorovSmirnov)?.PValue ?? 0)).OrderBy(a => -a.TestStatistiques.FirstOrDefault(a => a.TypeTestStatistique == TypeTestStatistique.KolmogorovSmirnov)?.PValue ?? 0).First().Distribution;
                        break;
                }
            }

        }

        public void CalibrerMLI()
        {
            var rand = MersenneTwister.MTRandom.Create(15376869);
            DateTime date = DateTime.Now;
            StringBuilder sbTags = new StringBuilder();
            StringBuilder sbTagsTest = new StringBuilder();


            foreach (var distrib in VisisbleData)
            {
                Directory.CreateDirectory($"./{distrib.Distribution.Type}/");
                for (int i = 0; i < 1000; i++)
                {
                    var path = $"./{distrib.Distribution.Type}/Image {i + 1}";
                    GenerationGraphique.SaveChartImage(GenerationGraphique.GetDensity(distrib.Distribution.Simulate(rand, Values.Length), Math.Min(100, Values.Length), PointsKDE.Select(a => a.X).Min(), PointsKDE.Select(a => a.X).Max()), path, 500, 500);
                    if (i < 700)
                    {
                        sbTags.AppendLine($"{path}.png\t{distrib.Distribution.Type}");
                    }
                    else
                    {
                        sbTagsTest.AppendLine($"{path}.png\t{distrib.Distribution.Type}");
                    }
                }
            }
            File.WriteAllText($"tags{date.Ticks}.tsv", sbTags.ToString());
            File.WriteAllText($"tags_test{date.Ticks}.tsv", sbTagsTest.ToString());
            MLContext mlContext = new MLContext();
            Model = MachineLearningHelper.GenerateModel(mlContext, $"tags{date.Ticks}.tsv", "./");
            GenerationGraphique.SaveChartImage(GenerationGraphique.GetDensity(Values, Math.Min(100, Values.Length)), $"image{date.Ticks}", 500, 500);
            ConfusionMatrixML = MachineLearningHelper.GetConfusionMatrix(mlContext, $"tags_test{date.Ticks}.tsv", "./", Model);
            var predictions = MachineLearningHelper.ClassifySingleImage(mlContext, Model, $"image{date.Ticks}.png");

            for (int i = 0; i < VisisbleData.Count; i++)
            {
                VisisbleData[i].ProbabiliteMachineLearningImage = predictions.Score[i];
            }
            foreach (var distrib in VisisbleData)
            {
                for (int i = 0; i < 1000; i++)
                {
                    File.Delete($"./image_PDF_{date.Ticks}_{distrib.Distribution.Type}_{i}.png");
                }
            }
            File.Delete($"./image{date.Ticks}.png");
            File.Delete($"tags{date.Ticks}.tsv");
            File.Delete($"tags_test{date.Ticks}.tsv");

        }


    }
}
