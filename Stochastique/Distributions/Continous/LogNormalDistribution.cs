﻿using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class LogNormalDistribution : NormalDistribution
    {
        [MemoryPack.MemoryPackConstructor]
        public LogNormalDistribution()
        {

        }
        public LogNormalDistribution(double mu, double sigma)
        {
            AddParameter(new Parameter(ParametreName.mu, mu));
            AddParameter(new Parameter(ParametreName.sigma, sigma));
        }
        [MemoryPack.MemoryPackOrder(6)]
        public override TypeDistribution Type => TypeDistribution.LogNormal;

        [MemoryPack.MemoryPackOrder(7)]
        public double mu => GetParameter(ParametreName.mu).Value;

        [MemoryPack.MemoryPackOrder(8)]
        public double sigma => GetParameter(ParametreName.sigma).Value;

        public override double InconditionnalMinimumPossibleValue => 0;

        public override double CDF(double x)
        {
            if(x<0)
            {
                return 0;
            }
            return 0.5 + 0.5 * SpecialFunctions.Erf((Math.Log(x) - mu) / (sigma * Constants.Sqrt2));
        }

        public override double ExpextedValue()
        {
            return Math.Exp(mu + sigma / 2);
        }

        public override double PDF(double x)
        {
            return 1 / (x * sigma * Constants.Sqrt2Pi) * Math.Exp(-Math.Pow(Math.Log(x) - mu, 2) / (2 * sigma * sigma));
        }

        public override double Variance()
        {
            return (Math.Exp(sigma * sigma) - 1) * Math.Exp(2 * mu + sigma * sigma);
        }

        public override double InverseCDF(double x)
        {
            return Math.Exp(base.InverseCDF(x));
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameters(CalibrateWithMoment(value));
            if (IsInInconditionnalSupport(value))
            {
                switch (typeCalibration)
                {
                    case TypeCalibration.MaximumLikelyhood:
                        Optim(value, typeCalibration);
                        break;
                    case TypeCalibration.LeastSquare:
                        Optim(value, typeCalibration);
                        break;
                }
            }
            VerifyParameterValue();
            IntervaleForDisplay = new Intervale(0, Math.Exp(mu + 5 * sigma));

        }

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            double mu = 0;
            double sigma = 0;
            mu = value.Sum(a => Math.Log(a)) / value.Count();
            sigma = Math.Sqrt(value.Sum(a => Math.Log(a) * Math.Log(a)) / value.Count() - mu * mu);


            result.Add(new Parameter(ParametreName.mu, mu));
            result.Add(new Parameter(ParametreName.sigma, sigma));
            return result;
        }
        public override double Simulate(Random r)
        {
            return Math.Exp(base.Simulate(r));
        }
        public override double[] Simulate(Random r, int n)
        {
            return base.Simulate(r, n).Select(a => Math.Exp(a)).ToArray();
        }
        public override double Skewness()
        {
            return (Math.Exp(sigma * sigma) + 2) * Math.Sqrt(Math.Exp(sigma * sigma) - 1);
        }

        public override double Kurtosis()
        {
            return Math.Exp(4 * sigma * sigma) + 2 * Math.Exp(3 * sigma * sigma) + 3 * Math.Exp(2 * sigma * sigma) - 6;
        }
    }
}
