using Accord.Statistics.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public class ShapiroTest : TestStatistiques
    {
        public ShapiroTest(double[] values)
        {
            StateH0= TypeDonnees.Normal;
            StateH1= TypeDonnees.NotNormal;
            Test = new ShapiroWilkTest(values);
            PValue = Test.PValue;
        }

        public ShapiroWilkTest Test { get; set; }
    }
}
