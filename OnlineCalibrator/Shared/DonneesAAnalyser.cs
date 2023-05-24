using GenerationImageDistribution;
using MathNet.Numerics.Statistics;
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

        public List<TestStatistiques> TestStatistiques { get; set; }

        public DonneesAAnalyser() { }
        public void Initialize() 
        {
            PointsCDF= GenerationGraphique.GetCDF(Values);
            PointsKDE= GenerationGraphique.GetDensity(Values,100);
        }


    }
}
