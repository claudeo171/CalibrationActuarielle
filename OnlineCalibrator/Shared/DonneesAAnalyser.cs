using GenerationImageDistribution;
using MathNet.Numerics.Statistics;
using Stochastique;
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
        public string Name { get; set; }
        public double[] Values { get; set; }

        public Point[] PointsKDE { get; set; }
        public Point[] PointsCDF { get; set; }

        public double Moyenne => Values?.Average()??0;
        public double Variance => Values == null ? 0: Values.Select(a=>a*a).Mean() - Moyenne* Moyenne;
        
        public double Kurtosis => Values == null ? 0 : Statistics.Kurtosis(Values);

        public double Skewness => Values == null ? 0 : Statistics.Skewness(Values);

        public List<TestStatistiques> TestStatistiques { get; set; }

        public DonneesAAnalyser() { }
        public void Initialize() 
        {
            PointsCDF= GenerationGraphique.GetCDF(Values);
            PointsKDE= GenerationGraphique.GetDensity(Values,100);
        }

        public TestStatistiques? GetTest(TypeTestStatistique type)
        {
            if(type== TypeTestStatistique.ShapiroWilk)
            {
                return TestStatistiques.FirstOrDefault(a=>a.GetType() == typeof(ShapiroTest));
            }
            return null;
        }
        public void CalculerTest()
        {
            TestStatistiques = new List<TestStatistiques>();
            TestStatistiques.Add(new ShapiroTest(Values));
        }

        public List<Point> GetQQPlot(Distribution loi, TypeCalibration calibration)
        {
            List<Point> rst = new List<Point>();
            loi.Initialize(Values, calibration);
            int i = 0;
            foreach(var elts in Values.Order())
            { 
                rst.Add(new Point() { X = loi.InverseCDF((0.5+i)/(Values.Length+1)), Y = elts });
                i++;
            }
            return rst;
        }
        

    }
}
