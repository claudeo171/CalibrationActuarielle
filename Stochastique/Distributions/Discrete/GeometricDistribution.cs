﻿using MathNet.Numerics.Statistics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class GeometricDistribution : DiscreteDistribution
    {
        [MemoryPack.MemoryPackConstructor]
        public GeometricDistribution() { }
        public GeometricDistribution(double p)
        {
            AddParameter(new Parameter(ParametreName.p, p));
        }
        [MemoryPack.MemoryPackIgnore]
        private double P => GetParameter(ParametreName.p).Value;

        public override TypeDistribution Type => TypeDistribution.Geometric;

        public override double ExpextedValue()
        {
            return 1 / P;
        }

        public override double Variance()
        {
            return (1 - P) / (P * P);
        }

        public override double Skewness()
        {
            return (2 - P) / Math.Sqrt((1 - P));
        }

        public override double Kurtosis()
        {
            return 6 + P * P / (1 - P);
        }

        protected override double PDFInt(int k)
        {
            return k>0?P * Math.Pow(1 - P, k-1):0;
        }
        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameters(CalibrateWithMoment(value));

            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(Variance()));
        }
        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            var ev = Statistics.Mean(value);

            result.Add(new Parameter(ParametreName.p, Math.Min(1, Math.Max(0, 1 / ev))));
            return result;
        }
        public override double Simulate(Random r)
        {
            return Math.Ceiling(Math.Log(r.NextDouble()) / Math.Log(1 - P));
        }
        public override double[] Simulate(Random r, int nbSimulations)
        {
            double[] rst = new double[nbSimulations];
            for(int i=0;i<nbSimulations;i++)
            {
                rst[i] = Simulate(r);
            }
            return rst;
        }
    }
}
