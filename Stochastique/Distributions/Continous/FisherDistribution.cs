﻿using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class FisherDistribution : Distribution
    {
        public FisherDistribution(int d1, int d2)
        {
            AddParameter(new Parameter(ParametreName.d1, d1));
            AddParameter(new Parameter(ParametreName.d2, d2));
        }
        [MemoryPack.MemoryPackConstructor]
        public FisherDistribution() { }

        [MemoryPack.MemoryPackOrder(6)]
        public double D1 => GetParameter(ParametreName.d1).Value;

        [MemoryPack.MemoryPackOrder(7)]
        public double D2 => GetParameter(ParametreName.d2).Value;

        [MemoryPack.MemoryPackOrder(8)]
        public override TypeDistribution Type => TypeDistribution.Fisher;
        public override double InconditionnalMinimumPossibleValue => 0;

        public override double CDF(double x)
        {
            if (x < 0) return 0;
            return SpecialFunctions.BetaRegularized(D1 / 2, D2 / 2, D1 * x / (D1 * x + D2));
        }

        public override double ExpextedValue()
        {
            return D2 > 2 ? D2 / (D2 - 2) : double.NaN;
        }

        public override double PDF(double x)
        {


            return Math.Exp(0.5 * (Math.Log(D1 * x) * D1 + Math.Log(D2) * D2 - Math.Log(D1 * x + D2) * (D1 + D2)) - (Math.Log(x) + SpecialFunctions.BetaLn(D1 / 2, D2 / 2)));
        }

        public override double Variance()
        {
            return D2 > 4 ? 2 * D2 * D2 * (D1 + D2 - 2) / (D1 * (D2 - 2) * (D2 - 2) * (D2 - 4)) : double.NaN;
        }
        public override double Skewness()
        {
            return (2 * D1 + D2 - 2) * Math.Sqrt(8 * (D2 - 4)) / ((D2 - 6) * Math.Sqrt(D1 * (D1 + D2 - 2)));
        }

        public override double Kurtosis()
        {
            return 12 * (D1 * (5 * D2 - 22) * (D1 + D2 - 2) + (D2 - 4) * (D2 - 2) * (D2 - 2)) / (D1 * (D2 - 6) * (D2 - 8) * (D1 + D2 - 2));
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameters(CalibrateWithMoment(value));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Statistics.Mean(value));
        }

        public override double[] Simulate(Random r, int nbSimulations)
        {

            double[] x = new Khi2Distribution((int)D1).Simulate(r, nbSimulations);
            double[] y = new Khi2Distribution((int)D2).Simulate(r, nbSimulations);

            for (int i = 0; i < x.Length; i++)
                x[i] /= y[i] * D1 / D2;
            return x;
        }
        public override double Simulate(Random r)
        {
            return Simulate(r, 1)[0];
        }
        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            result.Add(new Parameter(ParametreName.d2, Math.Max(5, (int)(2 * ev / ev - 1))));
            var d2 = result[0].Value;
            result.Add(new Parameter(ParametreName.d1, (int)(2 * d2 * d2 * d2 - 4 * d2 * d2 / (variance * (d2 - 2) * (d2 - 2) * (d2 - 4) - 2 * d2))));
            return result;
        }
    }
}
