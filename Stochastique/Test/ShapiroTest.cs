using Accord.Statistics.Testing;
using MessagePack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public class ShapiroTest : TestStatistique, IMessagePackSerializationCallbackReceiver
    {
        public ShapiroTest()
        { }
        public ShapiroTest(double[] values):this()
        {
            
            StateH0 = TypeDonnees.Normal;
            StateH1 = TypeDonnees.NotNormal;
            Values = values;
            Test = new ShapiroWilkTest(values);
            PValue = Test.PValue;
        }
        [Key(5)]
        public double[] Values { get; set; }

        [IgnoreMember]
        public ShapiroWilkTest Test { get; set; }

        public void OnAfterDeserialize()
        {
            Test = new ShapiroWilkTest(Values);
        }

        public void OnBeforeSerialize()
        {

        }
    }
}
