
using LiveChartsCore.SkiaSharpView.Painting;
using MathNet.Numerics.Statistics;
using Microsoft.ML;
using Microsoft.ML.Data;
using OnlineCalibrator.Shared.MachineLearning;
using SkiaSharp;
using Stochastique;
using Stochastique.Copule;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow.Keras.Engine;
//using Tensorflow;

namespace OnlineCalibrator.Shared
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class DonneesPourAnalyseConjointe
    {
        [MemoryPack.MemoryPackOrder(0)]
        public DonneesAAnalyser DonneesAAnalyser1 { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public DonneesAAnalyser DonneesAAnalyser2 { get; set; }
        [MemoryPack.MemoryPackIgnore]
        public Point[] ScatterPlot => DonneesAAnalyser1.Values.Select((a, i) => new Point() { X = a, Y = DonneesAAnalyser2.Values[i] }).ToArray();
        [MemoryPack.MemoryPackIgnore]
        public Point[] ScatterPlotRank
        {
            get
            {
                var elts = new List<List<double>>();
                elts.Add(DonneesAAnalyser1.Values.ToList());
                elts.Add(DonneesAAnalyser2.Values.ToList());
                return elts.GetDensity();
            }
        }

        [MemoryPack.MemoryPackOrder(2)]
        public double Correlation => Statistics.Covariance(DonneesAAnalyser1.Values, DonneesAAnalyser2.Values) / Math.Sqrt(DonneesAAnalyser1.Variance * DonneesAAnalyser2.Variance);

        [MemoryPack.MemoryPackIgnore]
        public double PValueCorrel => new StudentDistribution(DonneesAAnalyser1.Values.Length - 2).CDF(Math.Abs(Correlation) / Math.Sqrt(1 - Correlation * Correlation / (DonneesAAnalyser1.Values.Length - 2)));
        [MemoryPack.MemoryPackOrder(3)]
        public double RankCorrelation => Statistics.Covariance(DonneesAAnalyser1.Values.Rang().Select(a=>a*1.0), DonneesAAnalyser2.Values.Rang().Select(a => a * 1.0)) / Math.Sqrt(DonneesAAnalyser1.Values.Rang().Select(a => a * 1.0).Variance() * DonneesAAnalyser2.Values.Rang().Select(a => a * 1.0).Variance());

        [MemoryPack.MemoryPackOrder(5)]
        public List<CopuleWithData> Copules { get; set; } = new List<CopuleWithData>();
        [MemoryPack.MemoryPackOrder(6)]
        public MethodeCalibrationRetenue MethodeCalibration { get; set; }

        [MemoryPack.MemoryPackOrder(7)]
        public Copule CalibratedCopule { get; set; }

        [MemoryPack.MemoryPackOrder(8)]
        public TypeCopule? CalibratedCopuleType
        {
            get
            {
                return CalibratedCopule?.Type;
            }
            set
            {
                CalibratedCopule = Copules.FirstOrDefault(a => a.Copule.Type == value)?.Copule;

            }
        }
        [MemoryPack.MemoryPackIgnore]
        public ITransformer? Model { get; set; }
        [MemoryPack.MemoryPackIgnore]
        public double[][]? ConfusionMatrixML { get; set; }
        [MemoryPack.MemoryPackIgnore]
        public double[][]? ConfusionMatrixMaximumVraissemblance { get; set; }
        public List<CopuleWithData> GetAllCopula()
        {
            var distributions = Enum.GetValues(typeof(TypeCopule)).Cast<TypeCopule>().Where(a => Copule.CreateCopula(a) != null).ToList();
            var rst = distributions.Select(a => GetCopule(a, TypeCalibration.MaximumLikelyhood)).ToList();
            return rst;
        }


        public CopuleWithData GetCopule(TypeCopule typeDistribution, TypeCalibration? calibration, bool isTruncated = false)
        {
            var distrib = Copule.CreateCopula(typeDistribution);
            var values = new List<double[]> { DonneesAAnalyser1.Values, DonneesAAnalyser2.Values };
            if (calibration != null && !Copules.Any(a => a.Copule.Type == distrib.Type && a.Calibration == calibration))
            {
                distrib.Initialize(values, calibration.GetValueOrDefault());
                if (Copules.Any(a => a.Copule.Type == distrib.Type))
                {
                    Copules.First(a => a.Copule.Type == distrib.Type).Copule = distrib;
                    Copules.First(a => a.Copule.Type == distrib.Type).Calibration = calibration.GetValueOrDefault();
                }
                else
                {
                    Copules.Add(new CopuleWithData(distrib, values) { Calibration = calibration.Value });
                }
            }
            else if (calibration == null && !Copules.Any(a => distrib != null && a.Copule.Type == distrib.Type || distrib == null && a.Copule.Type == typeDistribution))
            {
                return GetCopule(typeDistribution, default(TypeCalibration));
            }
            return Copules.First(a => distrib != null && a.Copule.Type == distrib.Type || distrib == null && a.Copule.Type == typeDistribution);
        }

        public void ChangeSelectionMethod(MethodeCalibrationRetenue m)
        {
            MethodeCalibration = m;
            if (Copules != null)
            {
                switch (m)
                {
                    case MethodeCalibrationRetenue.AIC:
                        CalibratedCopule = Copules.Where(a => !double.IsNaN(a.AIC)).OrderBy(a => a.AIC).First().Copule;
                        break;
                    case MethodeCalibrationRetenue.BIC:
                        CalibratedCopule = Copules.Where(a => !double.IsNaN(a.BIC)).OrderBy(a => a.BIC).First().Copule;
                        break;
                    case MethodeCalibrationRetenue.Vraisemblance:
                        CalibratedCopule = Copules.Where(a => !double.IsNaN(a.LogLikelihood)).OrderBy(a => -a.LogLikelihood).First().Copule;
                        break;
                    case MethodeCalibrationRetenue.MachineLearningImage:
                        CalibratedCopule = Copules.Where(a => !double.IsNaN(a.ProbabiliteMachineLearningImage)).OrderBy(a => -a.ProbabiliteMachineLearningImage).First().Copule;
                        break;
                }
            }

        }
        public List<Point[]> GetCopuleCopulePlot(Random r,TypeCopule? typeDistribution = null)
        {
            List<Point[]> rst = new List<Point[]>();
            rst.Add(ScatterPlotRank);
            rst.Add(new Point[DonneesAAnalyser1.Values.Length]);
            Copule copule;
            if (typeDistribution == null)
            {
                copule = CalibratedCopule;
            }
            else
            {
                copule = GetCopule(typeDistribution.GetValueOrDefault(), null).Copule;
            }
            var simulatedData=copule.SimulerCopule(r, DonneesAAnalyser1.Values.Length);
            for (int i = 0; i < simulatedData.First().Count; i++)
            {
                rst[1][i] = new Point() { X = simulatedData[0][i], Y = simulatedData[1][i] };
            }
            return rst;
        }
        public void CalibrerMLI()
        {
            var rand = MersenneTwister.MTRandom.Create(15376869);
            DateTime date = DateTime.Now;
            StringBuilder sbTags = new StringBuilder();
            StringBuilder sbTagsTest = new StringBuilder();


            foreach (var distrib in Copules)
            {
                Directory.CreateDirectory($"./{distrib.Copule.Type}/");
                for (int i = 0; i < 1000; i++)
                {
                    var path = $"./{distrib.Copule.Type}/Image {i + 1}";
                    GenerationGraphique.SaveChartImage(new List<Point[]> { GenerationGraphique.GetRank(distrib.Copule.SimulerCopule(rand, ScatterPlotRank.Length)) },
                        new List<SolidColorPaint>{ null },
                        new List<SolidColorPaint>{ null },
                        new List<SolidColorPaint>{ new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4, Color = SKColors.Blue } },
                        new List<int> { 5},
                    path, 500, 500);
                    if (i < 700)
                    {
                        sbTags.AppendLine($"{path}.png\t{distrib.Copule.Type}");
                    }
                    else
                    {
                        sbTagsTest.AppendLine($"{path}.png\t{distrib.Copule.Type}");
                    }
                }
            }
            File.WriteAllText($"tags{date.Ticks}.tsv", sbTags.ToString());
            File.WriteAllText($"tags_test{date.Ticks}.tsv", sbTagsTest.ToString());
            MLContext mlContext = new MLContext();
            Model = MachineLearningHelper.GenerateModel(mlContext, $"tags{date.Ticks}.tsv", "./");
            GenerationGraphique.SaveChartImage(ScatterPlotRank, $"image{date.Ticks}", 500, 500);
            ConfusionMatrixML = MachineLearningHelper.GetConfusionMatrix(mlContext, $"tags_test{date.Ticks}.tsv", "./", Model).GetProba();
            var predictions = MachineLearningHelper.ClassifySingleImage(mlContext, Model, $"image{date.Ticks}.png");

            for (int i = 0; i < Copules.Count; i++)
            {
                Copules[i].ProbabiliteMachineLearningImage = predictions.Score[i];
            }
            foreach (var distrib in Copules)
            {
                for (int i = 0; i < 1000; i++)
                {
                    File.Delete($"./image_PDF_{date.Ticks}_{distrib.Copule.Type}_{i}.png");
                }
            }
            File.Delete($"./image{date.Ticks}.png");
            File.Delete($"tags{date.Ticks}.tsv");
            File.Delete($"tags_test{date.Ticks}.tsv");
            ComputeMLEConfusionMatrix();

        }
        public void ComputeMLEConfusionMatrix()
        {
            double[][] rstVraissemblance = new double[Copules.Count][];
            for (int i = 0; i < Copules.Count; i++)
            {
                rstVraissemblance[i] = new double[Copules.Count];
            }
            var random = new Random(1535664);
            int nbSimulations = 1000;
            for (int i = 0; i < nbSimulations; i++)
            {
                for (int j = 0; j < Copules.Count; j++)
                {
                    var values = Copules[j].Copule.SimulerCopule(random, DonneesAAnalyser1.Values.Length);
                    List<double[]> valeursMiseEnForme = new List<double[]>();
                    for(int w = 0; w < values[0].Count;w++)
                    {
                        valeursMiseEnForme.Add(new double[] { values[0][w], values[1][w] });
                    }
                    var distrib = Copules.MaxObject(a =>  a.Copule.GetLogLikelihood(valeursMiseEnForme));
                    rstVraissemblance[j][Copules.IndexOf(distrib)]++;

                }
            }
            for (int i = 0; i < Copules.Count; i++)
            {
                for (int j = 0; j < Copules.Count; j++)
                {
                    rstVraissemblance[i][j] /= nbSimulations;
                }
            }
            ConfusionMatrixMaximumVraissemblance = rstVraissemblance;
        }
    }

}

