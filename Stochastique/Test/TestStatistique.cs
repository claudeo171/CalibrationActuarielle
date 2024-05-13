using MessagePack;
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
    [MessagePackObject]
    [Union(0, typeof(ShapiroTest))]
    [Union(1, typeof(JarqueBeraTest))]
    [Union(2, typeof(KolmogorovSmirnovTest))]

    public abstract class TestStatistique
    {
        [Key(1)]
        public double PValue { get; set; }
        [Key(2)]
        public TypeTestStatistique TypeTestStatistique { get; set; }

        [Key(3)]
        public TypeDonnees StateH0 { get; set; }
        [Key(4)]
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
            var rst = new List<TestStatistique>() { new KolmogorovSmirnovTest(datas,distribution) };
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
