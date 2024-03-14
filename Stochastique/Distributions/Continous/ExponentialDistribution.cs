﻿using MathNet.Numerics.Statistics;
using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    [MessagePackObject]
    public class ExponentialDistribution : Distribution
    {
        public ExponentialDistribution() { }
        public ExponentialDistribution(double lambda)
        {
            AddParameter(new Parameter(ParametreName.lambda, lambda));
        }
        [Key(6)]
        public override TypeDistribution Type => TypeDistribution.Exponential;

        [Key(7)]
        public double Labda => GetParameter(ParametreName.lambda).Value;
        [IgnoreMember]
        public override double InconditionnalMinimumPossibleValue => 0;
        public override double CDF(double x)
        {
            return x < 0 ? 0 : 1 - Math.Exp(-Labda * x);
        }

        public override double ExpextedValue()
        {
            return 1 / Labda;
        }

        public override double PDF(double x)
        {

            return x < 0 ? 0 : Labda * Math.Exp(-Labda * x);
        }

        public override double Variance()
        {
            return 1 / (Labda * Labda);
        }
        public override double Skewness()
        {
            return 2;
        }

        public override double Kurtosis()
        {
            return 6;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            AddParameter(new Parameter(ParametreName.lambda, (1/ev + Math.Sqrt( 1/variance))/2));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10*ev);
        }
    }
}
