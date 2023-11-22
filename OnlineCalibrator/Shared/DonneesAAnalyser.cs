using GenerationImageDistribution;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using MessagePack;
using Newtonsoft.Json;
using Stochastique.Distributions;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public double Moyenne => Values?.Average()??0;

        [Key(5)]
        public double Variance => Values == null ? 0: Values.Select(a=>a*a).Mean() - Moyenne* Moyenne;

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

        public DonneesAAnalyser() { }
        public void Initialize() 
        {
            PointsCDF= GenerationGraphique.GetCDF(Values);
            PointsKDE= GenerationGraphique.GetDensity(Values,100);
        }


        public void CalculerTest()
        {

        }

        public List<Point[]> GetQQPlot(TypeDistribution typeDistribution)
        {
            List<Point[]> rst = new List<Point[]>();
            rst.Add(new Point[Values.Length]);
            rst.Add(new Point[Values.Length]);
            var loi = GetDistribution(typeDistribution,null);

            int i = 0;
            foreach(var elts in Values.Order())
            {
                double x = loi.Distribution.InverseCDF((0.5 + i) / (Values.Length + 1));
                double y = elts;
                if (i<Values.Length/2)
                {
                    rst[1][i] = new Point() { X = Math.Min(x,y), Y= Math.Min(x, y) };
                }
                else
                {
                    rst[1][i] = new Point() { X = Math.Max(x, y), Y = Math.Max(x, y) };
                }
                rst[0][i]=new Point() { X = x, Y = y };
                i++;
            }
            return rst;
        }


        public List<DistributionWithDatas> GetAllDistributions()
        {
            var distributions= Enum.GetValues(typeof(TypeDistribution)).Cast<TypeDistribution>().Where(a=> Distribution.CreateDistribution(a)!=null && Distribution.CreateDistribution(a).IsDiscreet==IsDiscreteDistribution).ToList();
            var rst=distributions.Select(a => GetDistribution(a, TypeCalibration.MaximumLikelyhood)).ToList();

            if (IncludeTrunkatedDistributions)
            {
                rst.AddRange(distributions.Where(a=> Distribution.CreateDistribution(a).IsTrunkable).Select(a => GetDistribution(a, TypeCalibration.MaximumLikelyhood,true)));
            }
            return rst;

        }


        public DistributionWithDatas GetDistribution(TypeDistribution typeDistribution, TypeCalibration? calibration,bool isTrunkated=false)
        {
            var distrib= Distribution.CreateDistribution(typeDistribution);
            if(isTrunkated)
            {
                distrib=new TrunkatedDistribution(distrib);
            }
            if (calibration!=null && !Distributions.Any(a => a.Distribution.Type == distrib.Type && a.Calibration == calibration))
            {
                distrib.Initialize(Values, calibration.GetValueOrDefault());
                if (Distributions.Any(a => a.Distribution.Type == distrib.Type))
                {
                    Distributions.First(a => a.Distribution.Type == distrib.Type).Distribution = distrib;
                    Distributions.First(a => a.Distribution.Type == distrib.Type).Calibration = calibration.GetValueOrDefault();
                }
                else
                {
                    Distributions.Add(new DistributionWithDatas(distrib,Values) { Calibration=calibration.Value});
                }
            }
            else if(calibration ==null  && !Distributions.Any(a => a.Distribution.Type == distrib.Type))
            {
                return GetDistribution(typeDistribution, default(TypeCalibration));
            }
            return Distributions.First(a => a.Distribution.Type == distrib.Type); 
        } 


    }
}
