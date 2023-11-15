﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Enums
{
    public enum TypeCalibration
    {
        Moment, LeastSquare, MaximumLikelyhood
    }
    public enum ParametreName
    {
        aAfine, bAfine, mu, sigma, n,
        k,qUp,qDown,
        p,lambda,r,N,a,b,d1,d2,
        theta,aCauchy,bCauchy,
        Np
    }

    public enum TypeOptimisation
    {
        NewtonRaphson, FisherScoring, LevenbergMarquardt, BLEICAlgorithm
    }
    public enum TypeDistribution
    {
        Normal, Student, /*LoiStudentAfine,*/
        Khi2, Bernouli, Binomial,Poisson, Geometric,NegativeBinomial,Pascal,Hypergeometrical,Beta,Cauchy,Exponential,Fisher,Gamma,Uniform,Weibull
    }
}
