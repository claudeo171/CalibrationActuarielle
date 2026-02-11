using Accord.Math;
using Accord.Statistics;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stochastique;
using MathNet.Numerics.Distributions;
using MathNet.Numerics;
using Stochastique.Test;

namespace OnlineCalibrator.Batch
{
    public static class TestHelper
    {
        public static double[] GetMoment(this double[] value)
        {
            return new double[] { value.Average(), value.Variance(), value.Skewness(), value.Kurtosis() };
        }
        public static double GetStatistic(this double[] moments, double[] theoricalMoment, bool isInfty, bool isRatio, double powN, double[]? normalisation)
        {
            double result = 0;
            for (int i = 0; i < moments.Length; i++)
            {
                var rtemp = moments[i] * Math.Pow(moments.Length, powN);
                if (isRatio)
                {
                    if (theoricalMoment[i] != 0)
                    {
                        rtemp /= theoricalMoment[i];
                        if (isInfty)
                        {
                            result = Math.Max(result, rtemp);
                        }
                        else
                        {
                            result += rtemp;
                        }
                    }
                }
                else
                {
                    if (normalisation != null)
                    {

                        rtemp = normalisation[i] == 0 ? (Math.Abs(rtemp - theoricalMoment[i]) > 10e-12 ? 1000 : 0) : Math.Abs(rtemp - theoricalMoment[i]) / normalisation[i];
                    }
                    else
                    {
                        if (rtemp < 0)
                        {
                            rtemp = -Math.Pow(-rtemp, 1.0 / (i+1));
                        }
                        else
                        {
                            rtemp = Math.Pow(rtemp, 1.0 / (i+1));
                        }
                        rtemp -= Math.Pow(theoricalMoment[i] * Math.Pow(moments.Length, powN), 1.0 / (i + 1));
                        rtemp = Math.Abs(rtemp);
                    }

                    if (!double.IsNaN(rtemp))
                    {
                        if (isInfty)
                        {
                            result = Math.Max(result, rtemp);
                        }
                        else
                        {
                            result += rtemp;
                        }
                    }
                }
            }
            return result;
        }

        public static (double seuilInf, double seuilSup, double[]? normalisation) GetSeuilTest(Distribution d, int size, double alpha, Random r, bool isInfty, bool isRatio, double powN, bool normaliser, bool normalizedWithQuantile)
        {
            int nb = 100000;
            double[] moment = d.GetMomentList();

            if (normaliser)
            {
                double[][] val = new double[nb][];

                double[] resultats = new double[nb];
                double[] normalisation = new double[moment.Length];
                for (int i = 0; i < nb; i++)
                {
                    val[i] = GetMoment(d.Simulate(r, size));
                }
                for (int i = 0; i < moment.Length; i++)
                {
                    var array = val.Select((a) => Math.Abs(a[i] - moment[i])).ToArray();
                    array.Sort();

                    if (normalizedWithQuantile)
                    {
                        normalisation[i] = array[(int)(nb * (1 - alpha))];
                    }
                    else
                    {
                        normalisation[i] = Math.Sqrt(array.Variance());
                    }
                }
                for (int i = 0; i < nb; i++)
                {
                    resultats[i] = val[i].GetStatistic(moment, isInfty, isRatio, powN, normalisation);
                }
                resultats.Sort();
                return new(double.MinValue, resultats[(int)((1 - alpha) * nb)], normalisation);
            }
            else
            {
                double[] val = new double[nb];
                for (int i = 0; i < nb; i++)
                {
                    val[i] = d.Simulate(r, size).GetMoment().GetStatistic(moment, isInfty, isRatio, powN, null);
                }
                val.Sort();
                if (isRatio)
                {
                    return new(val[(int)((alpha / 2) * nb)], val[(int)((1 - alpha / 2) * nb)], null);
                }
                else
                {
                    return new(double.MinValue, val[(int)((1 - alpha) * nb)], null);
                }
            }
        }

        public static List<ResultPuissance> CalculerPuissance(Distribution distributionTeste, Distribution distributionCalibree, double risque1espece, Random r, bool isInfty, bool isRatio, double powN, bool normaliser, bool normalizedWithQuantile)
        {
            List<ResultPuissance> result = new List<ResultPuissance>();
            double[] moment = distributionTeste.GetMomentList();
            List<int> sampleSize = new List<int>() { 10, 20, 30, 50, 100, 200 };

            foreach (var i in sampleSize)
            {

                var seuil = GetSeuilTest(distributionTeste, i, risque1espece, r, isInfty, isRatio, powN, normaliser, normalizedWithQuantile);
                double nbFalse = 0;
                double nbFalseNormal = 0;
                double nbFalseShapiro = 0;
                double nbFalseShapiroNormal = 0;
                int nbSim = 10000;
                for (int k = 0; k < nbSim; k++)
                {
                    var sampleCalibre = distributionCalibree.Simulate(r, i);
                    var sampleTeste = distributionTeste.Simulate(r, i);
                    double pValueReferenceCalibre = 0;
                    double pValueReferenceTeste = 0;
                    try
                    {
                        if (distributionTeste.Type == Stochastique.Enums.TypeDistribution.Normal)
                        {
                            pValueReferenceCalibre = new ShapiroTest(sampleCalibre).PValue;
                            pValueReferenceTeste = new ShapiroTest(sampleTeste).PValue;
                        }
                        else
                        {
                            pValueReferenceCalibre = new KSTest(sampleCalibre, distributionTeste).PValue;
                            pValueReferenceTeste = new KSTest(sampleTeste, distributionTeste).PValue;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                    if (pValueReferenceCalibre < risque1espece)
                    {
                        nbFalseShapiro++;
                    }
                    if (pValueReferenceTeste < risque1espece)
                    {
                        nbFalseShapiroNormal++;
                    }
                    var value = sampleCalibre.GetMoment().GetStatistic(moment,isInfty, isRatio, powN, seuil.normalisation);
                    if (seuil.seuilInf > value || seuil.seuilSup < value)
                    {
                        nbFalse++;
                    }
                    var valueNormal = sampleTeste.GetMoment().GetStatistic(moment, isInfty, isRatio, powN, seuil.normalisation);
                    if (seuil.seuilInf > valueNormal || seuil.seuilSup < valueNormal)
                    {
                        nbFalseNormal++;
                    }
                }
                result.Add(new ResultPuissance { DistributionTeste = distributionTeste, DistributionSimule = distributionCalibree, Alpha = risque1espece, RisquePremierEspece = nbFalseNormal / nbSim, Puissance = nbFalse / nbSim, PuissanceTestReference = nbFalseShapiro / nbSim, RisqueTestReference = nbFalseShapiroNormal / nbSim, SizeSample = i });
            }
            return result;
        }

        public static void LancerCalculNormal(bool isInfty, bool isRatio, double powN, string nameFile, int parralleLevel, bool normaliser, int maxMoment, bool normalizedWithQuantile)
        {
            var normal = new NormalDistribution(0, 1);
            List<Distribution> distributions = new List<Distribution> {
                new LoiBeta(0.5, 0.5), new LoiBeta(1, 1), new LoiBeta(2, 2),
                new CauchyDistribution(0.5,0), new CauchyDistribution(1, 0), new CauchyDistribution(2, 0),
                new UniformDistribution(0, 1),
                new LaplaceDistribution(0,1),
                new LogisticDistribution(2,2),
                new TukeyDistribution(0.14),new TukeyDistribution(0.5), new TukeyDistribution(2), new TukeyDistribution(5),new TukeyDistribution(10),
                new StudentDistribution(1),new StudentDistribution(2),new StudentDistribution(4),new StudentDistribution(10),
                new LoiBeta(2, 1), new LoiBeta(2, 5), new LoiBeta(4, 0.5),new LoiBeta(5, 1),
                new Khi2Distribution(1),new Khi2Distribution(2),new Khi2Distribution(4),new Khi2Distribution(10),
                new GammaDistribution(2, 2), new GammaDistribution(3, 2), new GammaDistribution(5, 1),new GammaDistribution(9, 1),new GammaDistribution(15, 1),new GammaDistribution(100, 1),
                new GumbelDistribution(1,2),
                new LogNormalDistribution(0,1),
                new WeibullDistribution(0.5,1),new WeibullDistribution(1,2),new WeibullDistribution(2,3.4),new WeibullDistribution(3,4),
                new TruncatedDistribution(new NormalDistribution(0,1),normal.CDF(-1),normal.CDF(1)),new TruncatedDistribution(new NormalDistribution(0,1),normal.CDF(-2),normal.CDF(2)),new TruncatedDistribution(new NormalDistribution(0,1),normal.CDF(-3),normal.CDF(3)),
                new TruncatedDistribution(new NormalDistribution(0,1),normal.CDF(-2),normal.CDF(1)),new TruncatedDistribution(new NormalDistribution(0,1),normal.CDF(-3),normal.CDF(1)),new TruncatedDistribution(new NormalDistribution(0,1),normal.CDF(-3),normal.CDF(2))
            };
            var rst = new List<ResultPuissance>[distributions.Count];
            Parallel.For(0, distributions.Count, new ParallelOptions { MaxDegreeOfParallelism = 1 }, (int i) =>
            {
                rst[i] = OnlineCalibrator.Batch.TestHelper.CalculerPuissance(normal, distributions[i], 0.05, new Random(), isInfty, isRatio, powN, normaliser, normalizedWithQuantile);
                Console.WriteLine($"La distribution {distributions[i]} a été testé");
            });
            var stringResult = new StringBuilder();
            stringResult.AppendLine("Distribution Testé;Distribution Simulé ;samplesize;alpha;power;powerReference;Risk");
            foreach (var resultat in rst)
            {
                foreach (var v in resultat)
                {
                    stringResult.AppendLine($"{v.DistributionTeste};{v.DistributionSimule};{v.SizeSample};{v.Alpha};{v.Puissance};{v.PuissanceTestReference};{v.RisquePremierEspece}");
                }
            }
            File.WriteAllText($"./{nameFile}.csv", stringResult.ToString());
        }
        public static void LancerCalculAutre(bool isInfty, bool isRatio, double powN, string nameFile, int parralleLevel, bool normaliser, bool normalizedWithQuantile)
        {
            Random r = new Random(497672);
            List<Distribution> distributions = new List<Distribution> {
                new LoiBeta(0.5, 0.5), new LoiBeta(1, 1), new LoiBeta(2, 2),
                new UniformDistribution(0, 1),
                new ExponentialDistribution(1),
                new ParetoDistribution(2,2),
                new FisherDistribution(2,5),
                new StudentDistribution(10),
                new Khi2Distribution(2),
                new GammaDistribution(2, 2),
                new GumbelDistribution(1,2),
                new LogNormalDistribution(0,1),
                new WeibullDistribution(0.5,1),
            };
            List<List<Distribution>> distributionsATester = distributions.Select(a => new DonneesAAnalyser { Values = a.Simulate(r, 1000) }.GetAllDistributions().Select(b => b.Distribution).Where(b => b.Type != a.Type).ToList()).ToList();
            var rst = new List<ResultPuissance>[distributionsATester.Sum(a => a.Count)+100];
#if DEBUG
            bool estParralel = false;
#else
            bool estParralel = true;
#endif
            if (estParralel)
            {
                Parallel.For(0, distributions.Count, new ParallelOptions { MaxDegreeOfParallelism = parralleLevel }, (int i) =>
                {

                    for (int j = 0; j < distributionsATester[i].Count; j++)
                    {
                        rst[j * (i + 1)] = OnlineCalibrator.Batch.TestHelper.CalculerPuissance(distributions[i], distributionsATester[i][j], 0.05, new Random(), isInfty, isRatio, powN, normaliser, normalizedWithQuantile);
                        Console.WriteLine($"La distribution {distributions[i]}, {distributionsATester[i][j]}, a été testé");
                    }

                });
            }
            else
            {
                for (int i = 0; i < distributions.Count; i++)
                {
                    for (int j = 0; j < distributionsATester[i].Count; j++)
                    {
                        rst[j*(i+1)] = OnlineCalibrator.Batch.TestHelper.CalculerPuissance(distributions[i], distributionsATester[i][j], 0.05, new Random(), isInfty, isRatio, powN, normaliser, normalizedWithQuantile);
                        Console.WriteLine($"La distribution {distributions[i]}, {distributionsATester[i][j]}, a été testé");
                    }
                }
            }
            var stringResult = new StringBuilder();
            stringResult.AppendLine("Distribution Testé;Distribution Simulé ;samplesize;alpha;power;powerReference;Risk");
            foreach (var resultat in rst)
            {
                if (resultat != null)
                {
                    foreach (var v in resultat)
                    {
                        stringResult.AppendLine($"{v.DistributionTeste};{v.DistributionSimule};{v.SizeSample};{v.Alpha};{v.Puissance};{v.PuissanceTestReference};{v.RisquePremierEspece}");
                    }
                }
            }
            File.WriteAllText($"./{nameFile}.csv", stringResult.ToString());
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
