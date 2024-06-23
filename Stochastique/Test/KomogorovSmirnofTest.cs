using Accord.Statistics.Testing;
using MessagePack;
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
    public class KolmogorovSmirnovTest : TestStatistique, IMessagePackSerializationCallbackReceiver
    {
        [Key(5)]
        public double[] Values { get; set; }
        [Key(6)]
        public Distribution Distribution { get; set; }
        public KolmogorovSmirnovTest() {
            TypeTestStatistique = TypeTestStatistique.KolmogorovSmirnov;
        }
        public KolmogorovSmirnovTest(double[] values, Distribution d) : this()
        {
            Values = values;
            Distribution = d;
            try
            {
                Test = new Accord.Statistics.Testing.TwoSampleKolmogorovSmirnovTest(values, values.Select((x, i) => Distribution.CDF((i + 0.5) / values.Length)).ToArray());
            }
            catch (Exception e)
            {
                Test = new Accord.Statistics.Testing.TwoSampleKolmogorovSmirnovTest(values, values.Select((x, i) => Double.IsNaN(Distribution.CDF((i + 0.5) / values.Length))? Distribution.CDF((i + 0.5) / values.Length) : 0).ToArray());
            }
            switch(Distribution.Type)
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
        [IgnoreMember]
        public Accord.Statistics.Testing.TwoSampleKolmogorovSmirnovTest Test { get; set; }
        public void OnAfterDeserialize()
        {
            Test = new Accord.Statistics.Testing.TwoSampleKolmogorovSmirnovTest(Values, Values.Select((x, i) => Distribution.CDF((i + 0.5) / Values.Length)).ToArray());
        }

        public void OnBeforeSerialize()
        {

        }
    }
}
