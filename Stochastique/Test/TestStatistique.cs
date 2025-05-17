using Stochastique.Distributions;
using Stochastique.Enums;
using Stochastique.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MemoryPack.MemoryPackable(MemoryPack.SerializeLayout.Explicit)]
    [MemoryPack.MemoryPackUnion(0, typeof(ShapiroTest))]
    [MemoryPack.MemoryPackUnion(1, typeof(JarqueBeraTest))]
    [MemoryPack.MemoryPackUnion(2, typeof(KolmogorovSmirnovTest))]
    [MemoryPack.MemoryPackUnion(3, typeof(BetaQuantileTest))]
    [MemoryPack.MemoryPackUnion(4, typeof(EELQuantileTest))]

    public abstract partial class TestStatistique
    {
        [MemoryPack.MemoryPackOrder(1)]
        public double PValue { get; set; }
        [MemoryPack.MemoryPackOrder(2)]
        public TypeTestStatistique TypeTestStatistique { get; set; }

        [MemoryPack.MemoryPackOrder(3)]
        public TypeDonnees StateH0 { get; set; }
        [MemoryPack.MemoryPackOrder(4)]
        public TypeDonnees StateH1 { get; set; }
        public TypeDonnees GetTypeDonnee(double alpha)
        {
            if (PValue > alpha)
            {
                return StateH0;
            }
            else
            {
                return StateH1;
            }
        }

        public static List<TestStatistique> GetTestsDistribution(Distribution distribution, double[] datas)
        {
            var rst = new List<TestStatistique>() { new KolmogorovSmirnovTest(datas,distribution), new BetaQuantileTest(datas,distribution,0.05) };
            switch (distribution.Type)
            {
                case TypeDistribution.Normal:
                    rst.Add(new ShapiroTest(datas));
                    break;
            }
            return rst;
        }
    }
}
