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
    public enum NomParametre
    {
        aAfine, bAfine, mu, sigma, n
    }

    public enum TypeOptimisation
    {
        NewtonRaphson, FisherScoring, LevenbergMarquardt, BLEICAlgorithm
    }
}
