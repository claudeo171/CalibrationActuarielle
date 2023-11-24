using System;
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

    public enum MethodeCalibrationRetenue
    {
        Vraisemblance, AIC, BIC, Manuelle
    }

    public enum TypeOptimisation
    {
        NewtonRaphson, FisherScoring, LevenbergMarquardt, BLEICAlgorithm
    }
    public enum TypeDistribution
    {
        Normal =1,
        Student =2, /*LoiStudentAfine,*/
        Khi2 =3, 
        Bernouli=4, 
        Binomial=5,
        Poisson=6, 
        Geometric=7,
        NegativeBinomial=8,
        Pascal=9,
        Hypergeometrical=10,
        Beta=11,
        Cauchy=12,
        Exponential=13,
        Fisher=14,
        Gamma=15,
        Uniform=16,
        Weibull=17,
        Trunkated=1000,
        TrunkatedNormal = 1001,
        TrunkatedStudent = 1002, 
        TrunkatedKhi2 = 1003,
        TrunkatedBernouli = 1004,
        TrunkatedBinomial = 1005,
        TrunkatedPoisson = 1006,
        TrunkatedGeometric = 1007,
        TrunkatedNegativeBinomial = 1008,
        TrunkatedPascal = 1009,
        TrunkatedHypergeometrical = 1010,
        TrunkatedBeta = 1011,
        TrunkatedCauchy = 1012,
        TrunkatedExponential = 1013,
        TrunkatedFisher = 1014,
        TrunkatedGamma = 1015,
        TrunkatedUniform = 1016,
        TrunkatedWeibull = 1017
    }
}
