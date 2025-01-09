using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    [MessagePack.MessagePackObject]
    public class SimulateurRejet
    {
        [MessagePack.Key(0)]
        public Distribution Distribution { get; set; }
        [MessagePack.Key(1)]
        public Distribution DistributionAuxiliaire {  get; set; }
        [MessagePack.Key(2)]
        public double ConstanteMajoration { get; set; }

        public SimulateurRejet(Distribution distribution, Distribution distributionAuxiliaire, double constanteMajoration)
        {
            Distribution = distribution;
            DistributionAuxiliaire = distributionAuxiliaire;
            ConstanteMajoration = constanteMajoration;
        }

        public double Simuler(Random r)
        {
            bool rejet = true;
            double x = 0;
            while (rejet)
            {
                x = DistributionAuxiliaire.Simulate(r);
                rejet = ConstanteMajoration * DistributionAuxiliaire.PDF(x) * r.NextDouble() > Distribution.PDF(x);
            }

            return x;
        }

    }
}
