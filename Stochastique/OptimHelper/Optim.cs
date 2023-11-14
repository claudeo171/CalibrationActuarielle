using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.OptimHelper
{
    public static class OptimHelper
    {
        public static double GetOptimalParametre( double initialPoint,double min,double max, Func<double, double> function)
        {
            double[] x = new double[] { initialPoint };
            double[] s = new double[] { 1 };
            double[] bndl = new double[] { min };
            double[] bndu = new double[] { max };
            alglib.minbleicstate state;
            double epsg = 0;
            double epsf = 0;
            double epsx = 0.000001;
            int maxits = 0;
            double diffstep = 1.0e-6;

            alglib.minbleiccreatef(x, diffstep, out state);
            alglib.minbleicsetbc(state, bndl, bndu);
            alglib.minbleicsetscale(state, s);
            alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);

            alglib.minbleicoptguardsmoothness(state);

            alglib.minbleicreport rep;

            alglib.minbleicoptimize(state, (double[] xx, ref double yy, object zz) => yy = function(xx[0]), null, null);

            alglib.minbleicresults(state, out x, out rep);

            alglib.optguardreport ogrep;
            alglib.minbleicoptguardresults(state, out ogrep);

            return x[0];
        }
    }
}
