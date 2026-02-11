using Accord.Statistics.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class ShapiroTest : TestStatistique
    {
        [MemoryPack.MemoryPackConstructor]
        public ShapiroTest()
        { }
        public ShapiroTest(double[] values):this()
        {
            
            StateH0 = TypeDonnees.Normal;
            StateH1 = TypeDonnees.NotNormal;

            var test = new ShapiroWilkTest(values);
            PValue = test.PValue;
        }
        [MemoryPack.MemoryPackOrder(5)]
        public double Statistic { get; set; }

    }
}
