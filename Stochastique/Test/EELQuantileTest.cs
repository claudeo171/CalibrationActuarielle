using Accord.Math;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using Stochastique.EEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Test
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class EELQuantileTest : TestStatistique
    {

        [MemoryPack.MemoryPackOrder(8)]
        public List<double> PValues { get; set; }

        [MemoryPack.MemoryPackOrder(9)]
        public double alpha;

        [MemoryPack.MemoryPackIgnore]
        public double Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
                if (PValues != null && PValues.Count > 0)
                {
                    PValue = PValues.Max();
                }
            }
        }
        [MemoryPack.MemoryPackConstructor]
        public EELQuantileTest()
        {

        }
        public EELQuantileTest(List<double> pvalues,  double alpha)
        {
            TypeTestStatistique = TypeTestStatistique.EELQuantile;
            PValues = pvalues;
            Alpha = alpha;            
            StateH0 = TypeDonnees.FollowDistribution;
            StateH1 = TypeDonnees.NotFollowDistribution;

            //PValue = PValues[(int)(Math.Round(Alpha * (PValues.Count - 1), MidpointRounding.AwayFromZero))];

        }
        public EELQuantileTest(double[] values, Distribution distribution, double alpha)
        {
            TypeTestStatistique = TypeTestStatistique.EELQuantile;
            values = values.Order().ToArray();
            Alpha = alpha;
            ComputePValues(values,distribution);
            switch (distribution.Type)
            {
                case Enums.TypeDistribution.Normal:
                    StateH0 = TypeDonnees.Normal;
                    StateH1 = TypeDonnees.NotNormal;
                    break;
                default:
                    StateH0 = TypeDonnees.FollowDistribution;
                    StateH1 = TypeDonnees.NotFollowDistribution;
                    break;
                    //TODO A completer
            }
            //PValue = PValues[(int)(Math.Round(Alpha * (PValues.Count - 1), MidpointRounding.AwayFromZero))];

        }

        private void ComputePValues(double[] values, Distribution d)
        {
            PValues = new List<double>();
            List<double> quantiles = values.Select(a => d.CDF(a)).ToList();
            if (values.Length < 100)
            {
                for (int i = 0; i < quantiles.Count; i++)
                {
                    var quantileBeta = new Stochastique.Distributions.Continous.LoiBeta(i + 1, quantiles.Count - i).CDF(quantiles[i]);
                    var valueForEEL = quantileBeta > 0.5 ? 1 - quantileBeta : quantileBeta;
                    PValues.Add(0.5 - ((1 - HelperEEL.GetQuantile(valueForEEL, quantiles.Count)) / 2) * (quantileBeta > 0.5 ? 1 : -1));
                }
            }
            else
            {
                var valeurTest = new double[] { 0.5, 0.4, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01, 0.005, 0.002, 0.001, 0.0005, 0.00001, 0.000001 };
                var valeurrst = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < valeurTest.Length; i++)
                {
                    valeurrst[i] = HelperEEL.GetQuantile(valeurTest[i], quantiles.Count);
                }
                for (int i = 0; i < quantiles.Count; i++)
                {
                    var quantileBeta = new Stochastique.Distributions.Continous.LoiBeta(i + 1, quantiles.Count - i).CDF(quantiles[i]);
                    var valueForEEL = quantileBeta > 0.5 ? 1 - quantileBeta : quantileBeta;
                    int indice = 0;
                    while (indice<valeurTest.Length && valeurTest[indice]>=valueForEEL)
                    {
                        indice++;
                    }
                    if (indice < valeurTest.Length)
                    {
                        var ratio = (valueForEEL - valeurTest[indice]) / (valeurTest[indice - 1] - valeurTest[indice]);
                        var valeurInterpolle = ratio * valeurrst[indice - 1] + (1 - ratio) * valeurrst[indice];
                        PValues.Add(0.5 - ((1 - valeurInterpolle) / 2) * (quantileBeta > 0.5 ? 1 : -1));
                    }
                    else
                    {
                        PValues.Add(0.5 - ((1 - valeurrst.Last()) / 2) * (quantileBeta > 0.5 ? 1 : -1));
                    }
                }
            }
            PValue = PValues.Max();
        }
    }
}
