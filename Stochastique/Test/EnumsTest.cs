using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public enum TypeTestStatistique
    {
        ShapiroWilk, JarqueBera,
        KolmogorovSmirnov
    }
    public enum TypeDonnees
    {
        Normal,NotNormal,NA
    }
}
