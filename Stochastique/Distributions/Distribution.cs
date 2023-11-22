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
    public abstract class Distribution : IMessagePackSerializationCallbackReceiver
    {
        [MessagePack.Key(0)]
        public virtual bool CanComputeExpectedValueEasily => true;

        [MessagePack.Key(1)]
        public abstract TypeDistribution Type { get; }

        [MessagePack.Key(2)]
        public virtual bool IsDiscreet => false;
        [IgnoreMember]
        public virtual bool IsTrunkable => true;
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

        [MessagePack.Key(3)]
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

        [MessagePack.Key(4)]
        public Intervale? IntervaleForDisplay { get; set; }



        [MessagePack.IgnoreMember]
        protected Dictionary<ParametreName, Parameter> ParametresParNom { get; set; } = new Dictionary<ParametreName, Parameter>();
        [MessagePack.Key(5)]
        public List<Parameter> ParametersList { get; set; }

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

        public virtual Parameter GetParameter(ParametreName nomParametre)
        {
            return  ParametresParNom[nomParametre];
        }

        public virtual IEnumerable<Parameter> AllParameters()
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
        public virtual void SetParameter(double[] values)
        {
            int i = 0;
            var parameters = AllParameters();
            foreach(var param in parameters)
            {
                param.Value = values[i];
                i++;
            }

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
            SetParameter(x);
            func = -GetLogLikelihood(values);
            if (double.IsPositiveInfinity(func))
            {
                func = double.MaxValue;
            }
        }

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

        private void CreateConstraints(alglib.minbleicstate state)
        {

            var parameters= AllParameters().ToList();
            double[] bndl = parameters.Select(p => p.MinValue).ToArray();
            double[] bndu = parameters.Select(p => p.MaxValue).ToArray();
            alglib.minbleicsetbc(state, bndl, bndu);
            if (Parameter.Contraints.Any(a => a.Parametres.All(b => parameters.Any(c => c.Name == b))))
            {
                var constraint = Parameter.Contraints.Where(a => a.Parametres.All(b => parameters.Any(c => c.Name == b))).ToList();
                int x = parameters.Count * 2 + constraint.Count;
                int y = parameters.Count + 1;
                double[,] c = (double[,])Array.CreateInstance(typeof(double), x, y);
                for(int i = 0; i < parameters.Count; i++)
                {
                    c[i, i] = 1;
                    c[i,parameters.Count] = parameters[i].MinValue;
                    c[i + parameters.Count, i] = -1;
                    c[i + parameters.Count, parameters.Count] = -parameters[i].MaxValue;
                }
                foreach(var contrainte in constraint)
                {
                    int i = 0;
                    foreach(var param in parameters)
                    {
                        int indice =contrainte.Parametres.IndexOf(param.Name);
                        if (indice!=-1)
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


        public void Optim(IEnumerable<double> values, TypeCalibration typeCalibration)
        {
            var parameters = AllParameters().ToList();
            double[] x = parameters.Select(p => p.Value).ToArray();
            double[] s = Enumerable.Repeat(1.0, x.Length).ToArray() ;
            if(this is TrunkatedDistribution)
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

        public void OnBeforeSerialize()
        {
            ParametersList= AllParameters()?.ToList();
        }

        public void OnAfterDeserialize()
        {
            ParametresParNom= ParametersList.ToDictionary(a=>a.Name, a=>a);
        }
    }

}