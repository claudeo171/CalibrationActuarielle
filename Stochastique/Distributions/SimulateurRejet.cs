using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class  SimulateurRejet
    {
        [MemoryPack.MemoryPackOrder(0)]
        public Distribution Distribution { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public Distribution DistributionAuxiliaire {  get; set; }
        [MemoryPack.MemoryPackOrder(2)]
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
