
using MathNet.Numerics.Statistics;
using MessagePack;
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
//using Tensorflow;

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public class DonneesPourAnalyseConjointe
    {
        [Key(0)]
        public DonneesAAnalyser DonneesAAnalyser1 { get; set; }
        [Key(1)]
        public DonneesAAnalyser DonneesAAnalyser2 { get; set; }
        [IgnoreMember]
        public Point[] ScatterPlot => DonneesAAnalyser1.Values.Select((a, i) => new Point() { X = a, Y = DonneesAAnalyser2.Values[i] }).ToArray();
        [IgnoreMember]
        public Point[] ScatterPlotRank
        {
            get
            {
                var xValueOrdered = DonneesAAnalyser1.Values.Select<double, (double valeur, int indice)>((a, i) => new(a, i)).OrderBy(a => a.valeur).ToArray();
                var yValueOrdered = DonneesAAnalyser2.Values.Select<double, (double valeur, int indice)>((a, i) => new(a, i)).OrderBy(a => a.valeur).ToArray();
                Point[] rst = new Point[xValueOrdered.Length];
                for (int i = 0; i < DonneesAAnalyser1.Values.Length; i++)
                {
                    if (rst[xValueOrdered[i].indice] == null)
                    {
                        rst[xValueOrdered[i].indice] = new Point();
                    }
                    if (rst[yValueOrdered[i].indice] == null)
                    {
                        rst[yValueOrdered[i].indice] = new Point();
                    }
                    rst[xValueOrdered[i].indice].X = (double)i/ xValueOrdered.Length;
                    rst[yValueOrdered[i].indice].Y = (double)i/ xValueOrdered.Length;
                }
                return rst;
            }
        }

        [Key(2)]
        public double Correlation => Statistics.Covariance(DonneesAAnalyser1.Values, DonneesAAnalyser2.Values) / Math.Sqrt(DonneesAAnalyser1.Variance * DonneesAAnalyser2.Variance);

        [Key(3)]
        public double RankCorrelation => Statistics.Covariance(DonneesAAnalyser1.Values.Rang().Select(a=>a*1.0), DonneesAAnalyser2.Values.Rang().Select(a => a * 1.0)) / Math.Sqrt(DonneesAAnalyser1.Values.Rang().Select(a => a * 1.0).Variance() * DonneesAAnalyser2.Values.Rang().Select(a => a * 1.0).Variance());

        [Key(5)]
        public List<CopuleWithData> Copules { get; set; } = new List<CopuleWithData>();
        [Key(6)]
        public MethodeCalibrationRetenue MethodeCalibration { get; set; }

        [Key(7)]
        public Copule CalibratedCopule { get; set; }

        [Key(8)]
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

        public List<CopuleWithData> GetAllCopula()
        {
            var distributions = Enum.GetValues(typeof(TypeCopule)).Cast<TypeCopule>().Where(a => Copule.CreateCopula(a) != null).ToList();
            var rst = distributions.Select(a => GetCopule(a, TypeCalibration.MaximumLikelyhood)).ToList();
            return rst;
        }


        public CopuleWithData GetCopule(TypeCopule typeDistribution, TypeCalibration? calibration, bool isTrunkated = false)
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
            for (int i = 0; i < simulatedData.Count; i++)
            {
                rst[1][i] = new Point() { X = simulatedData[0][i], Y = simulatedData[1][i] };
            }
            return rst;
        }
    }
}
