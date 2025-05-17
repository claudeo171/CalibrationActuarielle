using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Testing;
using Newtonsoft.Json.Linq;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Test
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class KolmogorovSmirnovTest : TestStatistique
    {
        [MemoryPack.MemoryPackConstructor]
        public KolmogorovSmirnovTest() {
            TypeTestStatistique = TypeTestStatistique.KolmogorovSmirnov;
        }
        public KolmogorovSmirnovTest(double[] values, Distribution d) : this()
        {
            try
            {
                Test = new Accord.Statistics.Testing.KolmogorovSmirnovTest(values, d);
            }
            catch (Exception e)
            {
                Test = new Accord.Statistics.Testing.TwoSampleKolmogorovSmirnovTest(values, values.Select((x, i) => Double.IsNaN(d.CDF((i + 0.5) / values.Length))? d.CDF((i + 0.5) / values.Length) : 0).ToArray());
            }
            switch(d.Type)
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
            PValue = Test.PValue;
        }
        [MemoryPack.MemoryPackIgnore]
        public Accord.Statistics.Testing.HypothesisTest<KolmogorovSmirnovDistribution> Test { get; set; }

    }
}
