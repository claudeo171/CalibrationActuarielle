using LiveChartsCore.Defaults;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.RootFinding;
using Stochastique.Distributions.Continous;
using Stochastique.Distributions.Discrete;
using Stochastique.Enums;
using System.ComponentModel.DataAnnotations;

namespace Stochastique.Distributions
{
    public abstract class Distribution
    {
        public virtual bool CanComputeExpectedValueEasily => true;
        public abstract TypeDistribution Type { get; }

        public virtual bool IsDiscreet => false;
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
                /*case TypeDistribution.LoiStudentAfine:
                    return new LoiAfine(new StudentDistribution(1), 1, 0);*/
                case TypeDistribution.Bernouli:
                    return new BernouliDistribution();
                case TypeDistribution.Poisson:
                    return new PoissonDistribution();
                case TypeDistribution.Weibull:
                    return new WeibullDistribution();
                case TypeDistribution.Binomial:
                    return new BinomialDistribution();
                case TypeDistribution.Beta:
                    return new BetaDistribution();
                case TypeDistribution.Cauchy:
                    return new CauchyDistribution();
                case TypeDistribution.Exponential:
                    return new ExponentialDistribution();
                case TypeDistribution.Fisher:
                    return new FisherDistribution();
                case TypeDistribution.Gamma:
                    return new GammaDistribution();
                case TypeDistribution.Geometric:
                    return new GeometricDistribution();
                case TypeDistribution.Hypergeometrical:
                    return new HyperGeometricalDistribution();
                case TypeDistribution.NegativeBinomial: return new NegativeBinomialDistribution();
                case TypeDistribution.Pascal: return new PascalDistribution();
                case TypeDistribution.Uniform: return new UniformDistribution();
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
                    Optim(value, typeCalibration);
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
        public double GetLogLikelihood(IEnumerable<double> values)
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
            func = -GetLogLikelihood(values);
            if (double.IsPositiveInfinity(func))
            {
                func = double.MaxValue;
            }
        }

        public void GetSquaredError(IEnumerable<double> values, double[] x, ref double func, object obj)
        {
            for (int i = 0; i < x.Length; i++)
            {
                ParametresParNom.Values.ElementAt(i).Value = x[i];
            }
            double increment = 1.0 / (values.Count()-1);
            double value = increment / 2;
            foreach(var val in values)
            {
                var invCDF = InverseCDF(value);
                func += (val - invCDF)* (val - invCDF);
            }
            if (double.IsPositiveInfinity(func))
            {
                func = double.MaxValue;
            }
        }
        public void Optim(IEnumerable<double> values, TypeCalibration typeCalibration)
        {
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

            alglib.minbleicoptguardsmoothness(state);

            alglib.minbleicreport rep;
            if (typeCalibration == TypeCalibration.LeastSquare)
            {
                values = values.Order();
                alglib.minbleicoptimize(state, (double[] xx, ref double yy, object zz) => GetSquaredError(values, xx, ref yy, zz), null, null);
            }
            else
            {
                alglib.minbleicoptimize(state, (double[] xx, ref double yy, object zz) => GetLogVraissemblanceOptim(values, xx, ref yy, zz), null, null);
            }
            alglib.minbleicresults(state, out x, out rep);

            alglib.optguardreport ogrep;
            alglib.minbleicoptguardresults(state, out ogrep);
        }

    }

}