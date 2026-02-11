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
        aAfine, bAfine, mu, sigma, n,nStudent,
        k,
        p,lambda,r,N,a,b,d1,d2,
        theta,aCauchy,bCauchy,
        Np,xm,aBeta,bBeta,beta,s,valeurMin,ValeurMax
    }

    public enum MethodeCalibrationRetenue
    {
        Vraisemblance, AIC, BIC, Manuelle, MachineLearningImage,KSTest
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
        LogNormal=18,
        Pareto=19,
        Gumbel=20,
        Logistic=21,
        Laplace=22,
        Tukey=23,
        Logarithmique=24,
        PartieEntierePuissanceUniforme=25,
        Joe=26,
        Stable=27,
        Truncated =1000,
        TruncatedNormal = 1001,
        TruncatedStudent = 1002, 
        TruncatedKhi2 = 1003,
        TruncatedBernouli = 1004,
        TruncatedBinomial = 1005,
        TruncatedPoisson = 1006,
        TruncatedGeometric = 1007,
        TruncatedNegativeBinomial = 1008,
        TruncatedPascal = 1009,
        TruncatedHypergeometrical = 1010,
        TruncatedBeta = 1011,
        TruncatedCauchy = 1012,
        TruncatedExponential = 1013,
        TruncatedFisher = 1014,
        TruncatedGamma = 1015,
        TruncatedUniform = 1016,
        TruncatedWeibull = 1017,
        TruncatedLogNormal = 1018,
        TruncatedPareto = 1019,
        TruncatedGumbel = 1020
    }

    public enum TypeCopule
    {
        Clayton,CopuleAMH,Frank,Gumbel,Joe,Gaussian
    }
}
