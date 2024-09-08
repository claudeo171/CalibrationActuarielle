using Accord.Math;
using MessagePack;
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
    public class EELQuantileTest : TestStatistique
    {
        private double[] valuesSeuil=new double[] { 0.9, 0.7, 0.5, 0.3, 0.1, 0.05, 0.02, 0.01, 0.002, 0.0005 };
        [Key(5)]
        public double[] Values { get; set; }
        [Key(6)]
        public Distribution Distribution { get; set; }

        [Key(8)]
        public List<double> PValues { get; set; }

        [Key(9)]
        public double alpha;

        [IgnoreMember]
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
                    PValue = PValues[(int)(Math.Round(Alpha * (PValues.Count - 1), MidpointRounding.AwayFromZero))];
                }
            }
        }
        public EELQuantileTest(double[] values, Distribution distribution, double alpha)
        {
            TypeTestStatistique = TypeTestStatistique.EELQuantile;
            Values = values.Order().ToArray();
            Distribution = distribution;
            Alpha = alpha;
            ComputePValues();
            switch (Distribution.Type)
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
            PValue = PValues[(int)(Math.Round(Alpha * (PValues.Count - 1), MidpointRounding.AwayFromZero))];

        }

        private void ComputePValues()
        {
            PValues = new List<double>();
            List<double> quantiles = Values.Select(a => Distribution.CDF(a)).ToList();
            for (int i = 0; i < quantiles.Count; i++)
            {
                var quantileBeta = new BetaDistribution(i + 1, quantiles.Count -i).CDF(quantiles[i]);
                var valueForEEL = quantileBeta > 0.5 ? 1 - quantileBeta : quantileBeta;
                PValues.Add(0.5 - ((1 - HelperEEL.GetQuantile(valueForEEL,  quantiles.Count))/2)*( quantileBeta>0.5?1:-1));
            }
            PValue = PValues.Max();
        }
    }
}
