using MathNet.Numerics.Statistics;
using Newtonsoft.Json.Linq;
using Stochastique;
using Stochastique.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public class JarqueBeraTest : TestStatistique
    {
        public JarqueBeraTest(double[] values)
        {
            Name = "Jarque Bera";
            StateH0 = TypeDonnees.Normal;
            StateH1 = TypeDonnees.NotNormal;
            PValue = CalculatePValue(values);
        }

        public double CalculatePValue(double[] values)
        {
            var screwness = Statistics.Skewness(values);
            var kutosis = Statistics.Kurtosis(values);
            double stat = values.Length / 6 * (screwness * screwness + (kutosis - 3) * (kutosis - 3) / 4);
            return  1 - new Khi2Distribution(values.Length).CDF(stat);
        }

    }
}
