using LiveChartsCore.Defaults;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.RootFinding;
using MessagePack;
using Stochastique.Distributions.Continous;
using Stochastique.Distributions.Discrete;
using Stochastique.Enums;
using System.ComponentModel.DataAnnotations;

namespace Stochastique.Distributions
{
    [MessagePackObject]
    [MessagePack.Union(0, typeof(BetaDistribution))]
    [MessagePack.Union(1, typeof(CauchyDistribution))]
    [MessagePack.Union(2, typeof(ExponentialDistribution))]
    [MessagePack.Union(3, typeof(FisherDistribution))]
    [MessagePack.Union(4, typeof(GammaDistribution))]
    [MessagePack.Union(5, typeof(Khi2Distribution))]
    [MessagePack.Union(6, typeof(LogNormalDistribution))]
    [MessagePack.Union(7, typeof(NormalDistribution))]
    [MessagePack.Union(8, typeof(StudentDistribution))]
    [MessagePack.Union(9, typeof(UniformDistribution))]
    [MessagePack.Union(10, typeof(WeibullDistribution))]
    [MessagePack.Union(11, typeof(BernouliDistribution))]
    [MessagePack.Union(12, typeof(BinomialDistribution))]
    [MessagePack.Union(13, typeof(DiscreteDistribution))]
    [MessagePack.Union(14, typeof(GeometricDistribution))]
    [MessagePack.Union(15, typeof(HyperGeometricalDistribution))]
    [MessagePack.Union(16, typeof(NegativeBinomialDistribution))]
    [MessagePack.Union(17, typeof(PascalDistribution))]
    [MessagePack.Union(18, typeof(PoissonDistribution))]
    [MessagePack.Union(19, typeof(ParetoDistribution))]
    [MessagePack.Union(20, typeof(TrunkatedDistribution))]
    [MessagePack.Union(21, typeof(LoiAfine))]
    [MessagePack.Union(22, typeof(MixtureDistribution))]
    public abstract class Distribution : IMessagePackSerializationCallbackReceiver
    {
        /// <summary>
        /// If true, the expected value can be computed easily.
        /// </summary>
        [MessagePack.Key(0)]
        public virtual bool CanComputeExpectedValueEasily => true;
        /// <summary>
        /// If true, the variance can be computed easily.
        /// </summary>
        [IgnoreMember]
        public virtual bool CanComputeVarianceEasily => true;
        /// <summary>
        /// The expected value of the distribution.
        /// </summary>

        [MessagePack.Key(1)]
        public abstract TypeDistribution Type { get; }
        /// <summary>
        /// If true, the distribution is discreet.
        /// </summary>
        [MessagePack.Key(2)]
        public virtual bool IsDiscreet => false;
        /// <summary>
        /// If true, the distribution can be trukated. Useful for uniform distribution witch is not well defined with four parameter.
        /// </summary>
        [IgnoreMember]
        public virtual bool IsTrunkable => true;

        [IgnoreMember]
        public virtual double InconditionnalMinimumPossibleValue => double.MinValue;

        [IgnoreMember]
        public virtual double InconditionnalMaximumPossibleValue => double.MaxValue;

        /// <summary>
        /// Create a distribution with the type of the distribution.
        /// </summary>
        /// <param name="typeDistribution">The type of distribution that will be created</param>
        /// <returns>A distribution of the asked type </returns>
        /// 



        public static Distribution CreateDistribution(TypeDistribution typeDistribution)
        {
            switch (typeDistribution)
            {
                case TypeDistribution.Normal:
                    return new NormalDistribution();
                case TypeDistribution.Khi2:
                    return new Khi2Distribution();
                case TypeDistribution.Student:
                    return new StudentDistribution();
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
                case TypeDistribution.LogNormal: return new LogNormalDistribution();
                default:
                    return null;
            }
        }
        /// <summary>
        /// If true, the distribution can be calibrated with moment method. (for example cauchy or hypergeometrical distribution can't be calibrated with moment method)
        /// </summary>
        [MessagePack.Key(3)]
        public bool AllowMomentParameter { get; set; }
        /// <summary>
        /// Return the kernel density graph of the distribution.
        /// </summary>
        /// <returns>List of 100 point with uniformly distributed absices, and all PDF Values</returns>
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
        /// <summary>
        /// Comtpute points for display CDF graph.
        /// </summary>
        /// <returns>List of 100 point with uniformly distributed absices, and all CDF Values</returns>
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

        /// <summary>
        /// Compute all the parameters of the disttribution for a given set of value and type of calibration
        /// </summary>
        /// <param name="value">List of value that represents a sample of a distribution</param>
        /// <param name="typeCalibration">
        /// The type of calibration between:
        ///     - Moments
        ///     - MaximumLikelyhood (https://en.wikipedia.org/wiki/Maximum_likelihood_estimation)
        ///     - LeastSquare on the CDF curve (https://en.wikipedia.org/wiki/Least_squares)
        /// </param>
        public virtual void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
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
        }
        protected void VerifyParameterValue()
        {
            foreach (var v in ParametresParNom)
            {
                if (v.Value.Value < v.Value.MinValue)
                {
                    v.Value.Value = v.Value.MinValue + Math.Pow(10,-10);
                }
                if (v.Value.Value > v.Value.MaxValue)
                {
                    v.Value.Value = v.Value.MaxValue - Math.Pow(10, -10);
                }
                if(double.IsNaN(v.Value.Value))
                {
                    v.Value.Value = v.Value.MinValue + Math.Pow(10, -10);
                }
            }
        }
        protected bool IsInInconditionnalSupport(IEnumerable<double> values)
        {
            return values.All(a => a > InconditionnalMinimumPossibleValue && a < InconditionnalMaximumPossibleValue);
        }
        /// <summary>
        /// Compute the value of the PDF for a given value
        /// </summary>
        /// <param name="x">The value</param>
        /// <returns></returns>
        public abstract double PDF(double x);

        /// <summary>
        /// Compute the expected value of the distribution
        /// </summary>
        /// <returns>The expected value of the distribution</returns>
        public abstract double ExpextedValue();
        /// <summary>
        /// Compute the variance of the distribution
        /// </summary>
        /// <returns>The variance of the distribution</returns>
        public abstract double Variance();
        /// <summary>
        /// Compute the standard deviation of the distribution
        /// </summary>
        /// <returns>The standard deviation of the distribution</returns>
        public double StandardDeviation()
        {
            return Math.Sqrt(Variance());
        }
        /// <summary>
        /// Compute the CDF of the distribution for a given value
        /// </summary>
        /// <param name="x">the value of CDF parameter</param>
        /// <returns>The CDF value for x.</returns>
        public abstract double CDF(double x);
        /// <summary>
        /// Compute the inverse CDF of the distribution for a given value. Can be called quantile function.
        /// </summary>
        /// <param name="x">The value in [0;1] </param>
        /// <returns>The quantile</returns>
        /// <exception cref="ArgumentException">If the parameter is not between 0 and 1</exception>
        public virtual double InverseCDF(double x)
        {
            if (x <= 0 || x >= 1)
            {
                throw new ArgumentException("Le paramètre doit être compris entre 0 et 1");
            }
            double min = -1;
            double max = 1;
            if (CanComputeExpectedValueEasily && !double.IsNaN(ExpextedValue()) && !double.IsNaN(StandardDeviation()))
            {
                min = ExpextedValue() - StandardDeviation() * 10;
                max = ExpextedValue() + StandardDeviation() * 10;
            }

            while (CDF(min) > x)
            {
                if (min > 0)
                {
                    min = -1;
                }
                if (CanComputeVarianceEasily && !double.IsNaN(Variance()))
                {
                    min -= StandardDeviation() * 10;
                }
                else
                {
                    min *= 10;
                }

            }
            while (CDF(max) < x)
            {
                if (max < 0)
                {
                    max = 1;
                }
                if (CanComputeVarianceEasily && !double.IsNaN(Variance()))
                {
                    max += StandardDeviation() * 10;
                }
                else
                {
                    max *= 10;
                }

            }
            double abs = (max + min) / 2;
            double ordonne = CDF(abs);
            while (Math.Abs(ordonne - x) > 1e-6)
            {
                if (x < ordonne)
                {
                    max = abs;
                }
                else if (x > ordonne)
                {
                    min = abs;
                }
                abs = (max + min) / 2;
                ordonne = CDF(abs);
            }
            return abs;
        }
        /// <summary>
        /// Intervale selected for displaying CDF and PDF. Must be set in all distributions. Necessary for displaing distribution on non finite interval.
        /// </summary>
        [MessagePack.Key(4)]
        public Intervale? IntervaleForDisplay { get; set; }


        /// <summary>
        /// List of all parameter of the distribution in dictionary by parameter name
        /// </summary>
        [MessagePack.IgnoreMember]
        protected Dictionary<ParametreName, Parameter> ParametresParNom { get; set; } = new Dictionary<ParametreName, Parameter>();
        /// <summary>
        /// Storing the list of parameter for serialization purpose. (MessagePack doesn't serialize dictionnary properly. Maybe because of me^^)
        /// </summary>
        [MessagePack.Key(5)]
        public List<Parameter> ParametersList { get; set; }
        /// <summary>
        /// Adding a parameter to the distribution. If a parameter with the same name already exists, an exception is thrown.
        /// </summary>
        /// <param name="parameter"> The parameter added</param>
        /// <exception cref="ArgumentException">If a parameter exist with the same name, exception is throw </exception>
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
        public void AddParameters(IEnumerable<Parameter> parameter)
        {
            foreach (var param in parameter)
            {
                AddParameter(param);
            }
        }
        /// <summary>
        /// Get a parameter by its name
        /// </summary>
        /// <param name="nomParametre">Name of parameter</param>
        /// <returns>The parameter</returns>
        public virtual Parameter GetParameter(ParametreName nomParametre)
        {
            return ParametresParNom[nomParametre];
        }
        /// <summary>
        /// Return all parameters of the distribution
        /// </summary>
        /// <returns> A ienumerable containing all parameters</returns>
        public virtual IEnumerable<Parameter> AllParameters()
        {
            return ParametresParNom.Values;
        }
        /// <summary>
        /// Fuction that returns the likelihood of a set of values.
        /// </summary>
        /// <param name="values">A set of double values</param>
        /// <returns>The likelihood</returns>
        public double GetVraissemblance(IEnumerable<double> values)
        {
            double rst = 1;
            foreach (var val in values)
            {
                rst *= PDF(val);
            }
            return rst;
        }
        /// <summary>
        /// Function that set all parameters value from an array of double
        /// </summary>
        /// <param name="values">Array of double that contains values of parameter</param>
        public virtual void SetParameter(double[] values)
        {
            int i = 0;
            var parameters = AllParameters();
            foreach (var param in parameters)
            {
                param.Value = values[i];
                i++;
            }

        }
        /// <summary>
        /// Fuction that returns the loglikelihood of a set of values.
        /// </summary>
        /// <param name="values">A set of double values</param>
        /// <returns>The likelihood</returns>
        public double GetLogLikelihood(IEnumerable<double> values)
        {
            double rst = 0;
            foreach (var val in values)
            {
                rst += Math.Log(PDF(val));
            }
            return rst;
        }
        /// <summary>
        /// Method that compute the loglikelihood of a set of values. This method is used for optimization purpose.
        /// </summary>
        /// <param name="values">The set of value used for calibration</param>
        /// <param name="x">The values of the parameters of the distribution</param>
        /// <param name="func">The result of loglikelihood</param>
        /// <param name="obj">A parameter of the optimizer not used</param>
        public void GetLogVraissemblanceOptim(IEnumerable<double> values, double[] x, ref double func, object obj)
        {
            SetParameter(x);
            func = -GetLogLikelihood(values);
            if (double.IsPositiveInfinity(func))
            {
                func = double.MaxValue;
            }
        }
        /// <summary>
        /// Method that compute the squared error (from CDF) of a set of values. This method is used for optimization purpose.
        /// </summary>
        /// <param name="values">The set of value used for calibration</param>
        /// <param name="x">The values of the parameters of the distribution</param>
        /// <param name="func">The result of square error</param>
        /// <param name="obj">A parameter of the optimizer not used</param>
        public void GetSquaredError(IEnumerable<double> values, double[] x, ref double func, object obj)
        {
            SetParameter(x);
            double increment = 1.0 / (values.Count() - 1);
            double value = increment / 2;
            foreach (var val in values)
            {
                var invCDF = InverseCDF(value);
                func += (val - invCDF) * (val - invCDF);
            }
            if (double.IsPositiveInfinity(func))
            {
                func = double.MaxValue;
            }
        }
        /// <summary>
        /// Function that create constraints for the optimization algorithm
        /// </summary>
        /// <param name="state">The state of the optimizer</param>
        private void CreateConstraints(alglib.minbleicstate state)
        {

            var parameters = AllParameters().ToList();
            double[] bndl = parameters.Select(p => p.MinValue).ToArray();
            double[] bndu = parameters.Select(p => p.MaxValue).ToArray();
            alglib.minbleicsetbc(state, bndl, bndu);
            if (Parameter.Contraints.Any(a => a.Parametres.All(b => parameters.Any(c => c.Name == b))))
            {
                var constraint = Parameter.Contraints.Where(a => a.Parametres.All(b => parameters.Any(c => c.Name == b))).ToList();
                int x = parameters.Count * 2 + constraint.Count;
                int y = parameters.Count + 1;
                double[,] c = (double[,])Array.CreateInstance(typeof(double), x, y);
                for (int i = 0; i < parameters.Count; i++)
                {
                    c[i, i] = 1;
                    c[i, parameters.Count] = parameters[i].MinValue;
                    c[i + parameters.Count, i] = -1;
                    c[i + parameters.Count, parameters.Count] = -parameters[i].MaxValue;
                }
                foreach (var contrainte in constraint)
                {
                    int i = 0;
                    foreach (var param in parameters)
                    {
                        int indice = contrainte.Parametres.IndexOf(param.Name);
                        if (indice != -1)
                        {
                            c[i + parameters.Count * 2, indice] = contrainte.Multiplier[indice];
                        }

                    }
                    c[i + parameters.Count * 2, parameters.Count] = contrainte.Value;
                    i++;
                }
                int[] ct = Enumerable.Repeat(1, x).ToArray();
                alglib.minbleicsetlc(state, c, ct);
            }

        }

        /// <summary>
        /// Function used for schearching the best parameters of the distribution by optimisation of loglikelihood or square error
        /// </summary>
        /// <param name="values"></param>
        /// <param name="typeCalibration"></param>
        public void Optim(IEnumerable<double> values, TypeCalibration typeCalibration)
        {
            var parameters = AllParameters().ToList();
            double[] x = parameters.Select(p => p.Value).ToArray();
            double[] s = Enumerable.Repeat(1.0, x.Length).ToArray();
            if (this is TrunkatedDistribution)
            {
                s[0] = 0.1;
                s[1] = 0.1;
            }

            alglib.minbleicstate state;
            double epsg = 0;
            double epsf = 0;
            double epsx = 0;
            int maxits = 1000;
            double diffstep = 1.0e-6;

            alglib.minbleiccreatef(x, diffstep, out state);
            CreateConstraints(state);
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

        /// <summary>
        /// Fonction called before serialization to store parameters in a list
        /// </summary>
        public void OnBeforeSerialize()
        {
            ParametersList = AllParameters()?.ToList();
        }
        /// <summary>
        /// Fuction called after deserialization to store parameters in a dictionnary
        /// </summary>
        public void OnAfterDeserialize()
        {
            ParametresParNom = ParametersList.ToDictionary(a => a.Name, a => a);
        }
        /// <summary>
        /// Function that simulate a sample of the distribution from a random generator
        /// </summary>
        /// <param name="r">The random generator</param>
        /// <returns>A value simulated from the distribution</returns>
        public virtual double Simulate(Random r)
        {
            return InverseCDF(r.NextDouble());
        }
        /// <summary>
        /// Function that simulate multiple sample of the distribution from a random generator
        /// </summary>
        /// <param name="r">The random generator</param>
        /// <param name="nbSimulations">The number of element in the result</param>
        /// <returns>A sample of the distribution with nbSimulations values.</returns>
        public virtual double[] Simulate(Random r, int nbSimulations)
        {
            return Enumerable.Range(0, nbSimulations).Select(a => Simulate(r)).ToArray();
        }

        public abstract double Skewness();

        public abstract double Kurtosis();
        public abstract IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values);
        public double[] GetParameterValues(IEnumerable<double> values, double decalage, double ratio)
        {
            return CalibrateWithMoment(values.Select(a=>a*ratio+decalage)).Select(a=> a.Value).ToArray();
        }
    }

}