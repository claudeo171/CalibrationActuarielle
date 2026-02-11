using MathNet.Numerics.Statistics;
using Newtonsoft.Json.Linq;
using Stochastique;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class JarqueBeraTest : TestStatistique
    {
        [MemoryPack.MemoryPackConstructor]
        public JarqueBeraTest() {
            TypeTestStatistique = TypeTestStatistique.JarqueBera;
        }
        public JarqueBeraTest(double[] values):this()
        {

            StateH0 = TypeDonnees.Normal;
            StateH1 = TypeDonnees.NotNormal;
            PValue = CalculatePValue(values);
        }

        public double CalculatePValue(double[] values)
        {
            double p = 0;
            alglib.jarquebera.jarqueberatest(values, values.Length, ref p, new alglib.xparams(0) { });
            return p;
        }

    }
}
