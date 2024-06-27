using Accord.Math;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Batch
{
    public static class TestHelper
    {
        public static double GetMoment(this double[] value, double[] moment, int maxMoment)
        {
            double result = 0;
            
            for (int i = 1; i <= maxMoment; i++)
            {
                var rtemp = value.Sum(a => Math.Pow(a, i));
                if (rtemp < 0)
                {
                    rtemp = -Math.Pow(-rtemp, 1.0 / i);
                }
                else
                {
                    rtemp = Math.Pow(rtemp, 1.0 / i);
                }
                rtemp -= Math.Pow(moment[i - 1]*value.Length, 1.0 / i);
                rtemp = Math.Abs(rtemp);
                result +=  rtemp;
            }

            return result;
        }

        public static double GetSeuilTest(Distribution d, int size, int maxMoment, double alpha, Random r)
        {
            int nb = 100000;
            double[] moment = d.GetMomentList(maxMoment);
            double[] val = new double[nb];
            for (int i = 0; i < nb; i++)
            {
                val[i] = GetMoment(d.Simulate(r, size), moment, maxMoment);
            }
            val.Sort();
            return val[(int)((1 - alpha) * nb)];
        }

        public static List<ResultPuissance> CalculerPuissance(Distribution distributionTeste, Distribution distributionCalibree, double risque1espece, Random r)
        {
            List<ResultPuissance> result = new List<ResultPuissance>();
            double[] moment = distributionTeste.GetMomentList(10);
            List<int> sampleSize = new List<int>() { 10, 20, 30, 50, 100, 200, 300, 400, 500, 1000, 2000 };
            foreach (var i in sampleSize)
            {
                double seuil = GetSeuilTest(distributionTeste, i, 10, risque1espece, r);
                double nbFalse = 0;
                double nbFalseNormal = 0;
                double nbFalseShapiro = 0;
                double nbFalseShapiroNormal = 0;
                int nbSim = 10000;
                for (int k = 0; k < nbSim; k++)
                {
                    var sample = distributionCalibree.Simulate(r, i);
                    var sampleNormal = distributionTeste.Simulate(r, i);
                    sample = sample.Select(a => a * distributionTeste.StandardDeviation() / distributionCalibree.StandardDeviation() - distributionCalibree.ExpextedValue() + distributionTeste.ExpextedValue()).ToArray();
                    
                    ShapiroTest shapiro = new ShapiroTest(sample);
                    ShapiroTest shapiroNormal = new ShapiroTest(sampleNormal);
                    if (shapiro.PValue<0.05)
                    {
                        nbFalseShapiro++;
                    }
                    if (shapiroNormal.PValue < 0.05)
                    {
                        nbFalseShapiroNormal++;
                    }
                    if (seuil< sample.GetMoment(moment, 10))
                    {
                        nbFalse++;
                    }
                    if (seuil< sampleNormal.GetMoment(moment, 10))
                    {
                        nbFalseNormal++;
                    }
                }
                result.Add(new ResultPuissance { DistributionTeste = distributionTeste, DistributionSimule = distributionCalibree, Alpha = risque1espece,RisquePremierEspece= nbFalseNormal/nbSim, Puissance = nbFalse/nbSim, PuissanceTestReference= nbFalseShapiro/nbSim, RisqueTestReference = nbFalseShapiroNormal/nbSim, SizeSample = i });
            }
            return result;
        }
    }

    public class ResultPuissance
    {
        public Distribution DistributionTeste { get; set; }
        public Distribution DistributionSimule { get; set; }
        public double Alpha { get; set; }
        public double RisquePremierEspece { get; set; }
        public double RisqueTestReference { get; set; }
        public int SizeSample { get; set; }
        public double Puissance { get; set; }
        public double PuissanceTestReference { get; set; }

    }
}
