using MessagePack;
using Stochastique.Enums;
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
    
    public abstract class TestStatistique
    {
        [Key(0)]
        public string Name { get; set; }
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

        public static List<TestStatistique> GetTestsDistribution(TypeDistribution typeDistribution, double[] datas)
        {
            switch (typeDistribution)
            {
                case TypeDistribution.Normal:
                    return new List<TestStatistique>
                    {
                        new ShapiroTest(datas),
                        new JarqueBeraTest(datas)
                    };
            }
            return new List<TestStatistique>();
        }
    }
}
