using MathNet.Numerics.Random;
using MersenneTwister;
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
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class BetaQuantileTest : TestStatistique
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
                    PValue = PValues[(int)(Math.Round(Alpha * (PValues.Count - 1), MidpointRounding.AwayFromZero))];
                }
            }
        }
        [MemoryPack.MemoryPackConstructor]
        public BetaQuantileTest()
        {

        }
        public BetaQuantileTest(double[] values, Distribution distribution, double alpha)
        {
            TypeTestStatistique = TypeTestStatistique.BetaQuantile;
            values = values.Order().ToArray();
            Alpha=alpha;
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
            PValue = PValues[(int)(Math.Round(Alpha*(PValues.Count-1),MidpointRounding.AwayFromZero))];

        }

        private void ComputePValues(double[] values, Distribution distribution)
        {
            List<double> quantiles = values.Select(a=>distribution.CDF(a)).ToList();
            PValues = new List<double>();
            for(int i = 0; i < quantiles.Count; i++)
            {
                var distributionBeta=new LoiBeta(i+1,quantiles.Count-i);
                PValues.Add(1-distributionBeta.CDF(quantiles[i]));
            }
        }
    }
}
