using MathNet.Numerics.Random;
using MersenneTwister;
using MessagePack;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Test
{
    [MessagePackObject]
    public class BetaQuantileTest : TestStatistique
    {
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
        public BetaQuantileTest()
        {

        }
        public BetaQuantileTest(double[] values,Distribution distribution, double alpha)
        {
            TypeTestStatistique = TypeTestStatistique.BetaQuantile;
            Values=values.Order().ToArray();
            Distribution = distribution;
            Alpha=alpha;
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
            PValue = PValues[(int)(Math.Round(Alpha*(PValues.Count-1),MidpointRounding.AwayFromZero))];

        }

        private void ComputePValues()
        {
            List<double> quantiles = Values.Select(a=>Distribution.CDF(a)).ToList();
            PValues = new List<double>();
            for(int i = 0; i < quantiles.Count; i++)
            {
                var distributionBeta=new LoiBeta(i+1,quantiles.Count-i);
                PValues.Add(1-distributionBeta.CDF(quantiles[i]));
            }
        }
    }
}
