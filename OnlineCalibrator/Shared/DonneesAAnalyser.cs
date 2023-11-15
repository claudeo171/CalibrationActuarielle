using GenerationImageDistribution;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
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
    public class DonneesAAnalyser
    {
        public string? Name { get; set; }
        public double[]? Values { get; set; }

        public Point[]? PointsKDE { get; set; }
        public Point[]? PointsCDF { get; set; }

        public double Moyenne => Values?.Average()??0;
        public double Variance => Values == null ? 0: Values.Select(a=>a*a).Mean() - Moyenne* Moyenne;
        
        public double Kurtosis => Values == null ? 0 : Statistics.Kurtosis(Values);

        public double Skewness => Values == null ? 0 : Statistics.Skewness(Values);


        public Dictionary<TestStatistique, bool>? ResultatStatistique { get; set; }
        /// <summary>
        /// List of distribution with datas. Only one element for each distribution type.
        /// </summary>
        public List<DistributionWithDatas> Distributions { get; set; } = new List<DistributionWithDatas>(); 

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


        public List<DistributionWithDatas> GetAllDistributions(bool isDiscrete, bool includeTrunkated)
        {
            var distributions= Enum.GetValues(typeof(TypeDistribution)).Cast<TypeDistribution>().Where(a=> Distribution.CreateDistribution(a).IsDiscreet==isDiscrete).ToList();
            if(includeTrunkated)
            {
                //TODO Create all trunkated distribution
            }
            return distributions.Select(a=> GetDistribution(a, TypeCalibration.MaximumLikelyhood)).ToList();

        }


        public DistributionWithDatas GetDistribution(TypeDistribution typeDistribution, TypeCalibration? calibration)
        {
            var distrib= Distribution.CreateDistribution(typeDistribution);
            if (calibration!=null && !Distributions.Any(a => a.Distribution.GetType() == distrib.GetType() && a.Calibration == calibration))
            {
                distrib.Initialize(Values, calibration.GetValueOrDefault());
                if (Distributions.Any(a => a.Distribution.GetType() == distrib.GetType()))
                {
                    Distributions.First(a => a.Distribution.GetType() == distrib.GetType()).Distribution = distrib;
                    Distributions.First(a => a.Distribution.GetType() == distrib.GetType()).Calibration = calibration.GetValueOrDefault();
                }
                else
                {
                    Distributions.Add(new DistributionWithDatas(distrib,Values) { Calibration=calibration.Value});
                }
            }
            else if(calibration ==null  && !Distributions.Any(a => a.Distribution.GetType() == distrib.GetType()))
            {
                return GetDistribution(typeDistribution, default(TypeCalibration));
            }
            return Distributions.First(a => a.Distribution.GetType() == distrib.GetType()); 
        } 


    }
}
