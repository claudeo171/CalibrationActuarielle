using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public class TestStatistique
    {
        public string Name { get; set; }
        public double PValue { get; set; }
        public TypeTestStatistique TypeTestStatistique { get; set; }

        public TypeDonnees StateH0 { get; set; }
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
