﻿using MathNet.Numerics.Statistics;
using MessagePack;
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
    [MessagePackObject]
    public class JarqueBeraTest : TestStatistique
    {
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
            var screwness = Statistics.Skewness(values);
            var kutosis = Statistics.Kurtosis(values);
            double stat = values.Length / 6 * (screwness * screwness + (kutosis - 3) * (kutosis - 3) / 4);
            return  1 - new Khi2Distribution(values.Length).CDF(stat);
        }

    }
}
