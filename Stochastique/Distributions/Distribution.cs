using LiveChartsCore.Defaults;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.RootFinding;
using Stochastique.Enums;

namespace Stochastique.Distributions
{
    public abstract class Distribution
    {
        public virtual bool CanComputeExpectedValueEasily => true;
        public abstract TypeDistribution Type { get; }
        public static Distribution CreateDistribution(TypeDistribution typeDistribution)
        {
            switch (typeDistribution)
            {
                case TypeDistribution.Normal:
                    return new NormalDistribution();
                case TypeDistribution.Khi2:
                    return new Khi2Distribution();
                case TypeDistribution.Student:
                    return new StudentDistribution(0);
                case TypeDistribution.LoiStudentAfine:
                    return new LoiAfine(new StudentDistribution(1), 1, 0);
                default:
                    return null;
            }
        }
        public bool AllowMomentParameter { get; set; }
        public List<ObservablePoint> DensityGraph()
        {
            List<ObservablePoint> result = new List<ObservablePoint>();
            if (IntervaleForDisplay != null)
            {
                for (int i = 0; i <= 100; i++)
                {
                    double x = (IntervaleForDisplay.Min * (100 - i) + IntervaleForDisplay.Max * i) / 100;
                    result.Add(new ObservablePoint(x, PDF(x)));
                }
            }
            return result;
        }
        public List<ObservablePoint> CDFGraph()
        {
            List<ObservablePoint> result = new List<ObservablePoint>();
            if (IntervaleForDisplay != null)
            {
                for (int i = 0; i <= 100; i++)
                {
                    double x = (IntervaleForDisplay.Min * (100 - i) + IntervaleForDisplay.Max * i) / 100;
                    result.Add(new ObservablePoint(x, CDF(x)));
                }
            }
            return result;
        }
        public virtual void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            switch (typeCalibration)
            {
                case TypeCalibration.MaximumLikelyhood:
                    Optim(value, typeCalibration);
                    break;
                case TypeCalibration.LeastSquare:
                    break;
            }
        }

        public abstract double PDF(double x);


        public abstract double ExpextedValue();
        public abstract double Variance();
        public double EcartType()
        {
            return Math.Sqrt(Variance());
        }
        public abstract double CDF(double x);
        public virtual double InverseCDF(double x)
        {
            if (x <= 0 || x >= 1)
            {
                throw new ArgumentException("Le paramètre doit être compris entre 0 et 1");
            }
            double min = -1;
            double max = 1;
            if (CanComputeExpectedValueEasily && !double.IsNaN(ExpextedValue()) && !double.IsNaN(Variance()))
            {
                min = ExpextedValue() - EcartType() * 10;
                max = ExpextedValue() + EcartType() * 10;
            }

            while (CDF(min) > x)
            {
                if (!double.IsNaN(Variance()))
                {
                    min -= EcartType() * 10;
                }
                else
                {
                    min *= 10;
                }

            }
            while (CDF(max) < x)
            {
                if (!double.IsNaN(Variance()))
                {
                    max += EcartType() * 10;
                }
                else
                {
                    min *= 10;
                }

            }
            double abs = (max + min) / 2;
            double ordonne = CDF(abs);
            while (Math.Abs(ordonne - x) > 1e-6)
            {
                if (x < ordonne)
                {
                    max = ordonne;
                }
                else if (x > ordonne)
                {
                    min = ordonne;
                }
                abs = (max + min) / 2;
                ordonne = CDF(abs);
            }
            return abs;
        }

        public Intervale? IntervaleForDisplay { get; set; }



        private Dictionary<ParametreName, Parameter> ParametresParNom { get; set; } = new Dictionary<ParametreName, Parameter>();

        public void AddParameter(Parameter parameter)
        {
            if (ParametresParNom.ContainsKey(parameter.Name))
            {
                throw new ArgumentException("Un paramètre avec le même nom existe");
            }
            else
            {
                ParametresParNom.Add(parameter.Name, parameter);
            }
        }

        public Parameter GetParameter(ParametreName nomParametre)
        {
            return ParametresParNom[nomParametre];
        }

        public IEnumerable<Parameter> AllParameters()
        {
            return ParametresParNom.Values;
        }

        public double GetVraissemblance(IEnumerable<double> values)
        {
            double rst = 1;
            foreach (var val in values)
            {
                rst *= PDF(val);
            }
            return rst;
        }
        public double GetLogVraissemblance(IEnumerable<double> values)
        {
            double rst = 0;
            foreach (var val in values)
            {
                rst += Math.Log(PDF(val));
            }
            return rst;
        }
        public void GetLogVraissemblanceOptim(IEnumerable<double> values, double[] x, ref double func, object obj)
        {
            for (int i = 0; i < x.Length; i++)
            {
                ParametresParNom.Values.ElementAt(i).Value = x[i];
            }
            func = -GetLogVraissemblance(values);
            if (double.IsPositiveInfinity(func))
            {
                func = double.MaxValue;
            }
        }
        public void Optim(IEnumerable<double> values, TypeCalibration typeCalibration)
        {
            //
            // This example demonstrates minimization of
            //
            //     f(x,y) = 100*(x+3)^4+(y-3)^4
            //
            // subject to box constraints
            //
            //     -1<=x<=+1, -1<=y<=+1
            //
            // using BLEIC optimizer with:
            // * numerical differentiation being used
            // * initial point x=[0,0]
            // * unit scale being set for all variables (see minbleicsetscale for more info)
            // * stopping criteria set to "terminate after short enough step"
            // * OptGuard integrity check being used to check problem statement
            //   for some common errors like nonsmoothness or bad analytic gradient
            //
            // First, we create optimizer object and tune its properties:
            // * set box constraints
            // * set variable scales
            // * set stopping criteria

            var parameters = AllParameters().ToList();


            double[] x = parameters.Select(p => p.Value).ToArray();
            double[] s = new double[] { 1, 1 };

            double[] bndl = parameters.Select(p => p.MinValue).ToArray();
            double[] bndu = parameters.Select(p => p.MaxValue).ToArray();
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

            //
            // Then we activate OptGuard integrity checking.
            //
            // Numerical differentiation always produces "correct" gradient
            // (with some truncation error, but unbiased). Thus, we just have
            // to check smoothness properties of the target: C0 and C1 continuity.
            //
            // Sometimes user accidentally tries to solve nonsmooth problems
            // with smooth optimizer. OptGuard helps to detect such situations
            // early, at the prototyping stage.
            //
            alglib.minbleicoptguardsmoothness(state);

            //
            // Optimize and evaluate results
            //
            alglib.minbleicreport rep;
            alglib.minbleicoptimize(state, (double[] xx, ref double yy, object zz) => GetLogVraissemblanceOptim(values, xx, ref yy, zz), null, null);
            alglib.minbleicresults(state, out x, out rep);
            Console.WriteLine("{0}", rep.terminationtype); // EXPECTED: 4
            Console.WriteLine("{0}", alglib.ap.format(x, 2)); // EXPECTED: [-1,1]

            //
            // Check that OptGuard did not report errors
            //
            // Want to challenge OptGuard? Try to make your problem
            // nonsmooth by replacing 100*(x+3)^4 by 100*|x+3| and
            // re-run optimizer.
            //
            alglib.optguardreport ogrep;
            alglib.minbleicoptguardresults(state, out ogrep);
            Console.WriteLine("{0}", ogrep.nonc0suspected); // EXPECTED: false
            Console.WriteLine("{0}", ogrep.nonc1suspected); // EXPECTED: false
        }

        public void MaximumDeVraisemblance(List<double> values, TypeOptimisation opti = TypeOptimisation.NewtonRaphson)
        {
            if (opti == TypeOptimisation.LevenbergMarquardt)
            {
                LevenbergMarquardtMinimizer levenbergMarquardtMinimizer = new LevenbergMarquardtMinimizer();
                //levenbergMarquardtMinimizer.FindMinimum(()
            }
            else if (opti == TypeOptimisation.BLEICAlgorithm)
            {
                alglib.minbleicstate state;
            }
        }


    }

}