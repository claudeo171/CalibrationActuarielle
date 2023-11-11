﻿using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    public class GammaDistribution : Distribution
    {
        public double K => GetParameter(ParametreName.k).Value;
        public double Theta => GetParameter(ParametreName.theta).Value;
        public override TypeDistribution Type => TypeDistribution.Gamma;

        public override double CDF(double x)
        {
            return SpecialFunctions.GammaLowerIncomplete(K,x/Theta) / SpecialFunctions.Gamma(K);
        }

        public override double ExpextedValue()
        {
            return K * Theta;
        }

        public override double PDF(double x)
        {
            return Math.Exp(Math.Log(x)* (K-1) -(x/Theta)-Math.Log(Theta)*K -SpecialFunctions.GammaLn(K));
        }

        public override double Variance()
        {
            return K * Theta * Theta;
        }
    }
}