using Stochastique.Distributions;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    [MessagePack.Union(0, typeof(CopuleAMH))]
    [MessagePack.Union(1, typeof(CopuleClayton))]
    [MessagePack.MessagePackObject]
    public abstract class Copule
    {

        /// <summary>
        /// List of all parameter of the distribution in dictionary by parameter name
        /// </summary>
        [MessagePack.IgnoreMember]
        protected Dictionary<CopuleParameterName, CopuleParameter> ParametresParNom { get; set; } = new Dictionary<CopuleParameterName, CopuleParameter>();
        /// <summary>
        /// Storing the list of parameter for serialization purpose. (MessagePack doesn't serialize dictionnary properly. Maybe because of me^^)
        /// </summary>
        [MessagePack.Key(1)]
        public List<CopuleParameter> ParametersList { get; set; }
        [MessagePack.Key(2)]
        public int Dimension { get; set; }
        [MessagePack.Key(3)]
        public TypeCopule Type { get; set; }
        public abstract List<List<double>> SimulerCopule(Random r,int nbSim);

        public virtual void AppliquerCopule(Random r, List<List<double>> variablesAleatoires)
        {
            if (variablesAleatoires.Count != Dimension)
            {
                throw new Exception("Nombre de variables aléatoires diffère de la dimension de la copule");
            }

            int nbSim = variablesAleatoires[0].Count;

            for (int i = 1; i < Dimension; i++)
            {
                if (variablesAleatoires[1].Count != nbSim)
                {
                    throw new Exception("La taille des échantillons n'est pas comptatibles");
                }
            }

            List<List<double>> uniformes = SimulerCopule(r,nbSim);

            List<int> nouvelOrdre = RendreComonotone(variablesAleatoires[0], uniformes[0]);

            for (int i = 1; i < Dimension; i++)
            {
                uniformes[i].ReordonnerSimulations(nouvelOrdre);
                RendreComonotone(uniformes[i], variablesAleatoires[i]);
            }
        }

        protected List<int> RendreComonotone(List<double> x, List<double> y)
        {
            if (x.Count != y.Count)
            {
                throw new Exception("La taille des échantillons n'est pas comptatibles");
            }

            //On conserve l'ordre des simulations de la premiere variable: x
            List<int> rang_x = x.Rang();
            List<int> sortIndex_y = y.SortIndex();
            List<int> nouvelOrdreSimulations = new List<int>();
            for (int i = 0; i < x.Count; i++)
            {
                nouvelOrdreSimulations.Add(sortIndex_y[rang_x[i]]);
            }

            y.ReordonnerSimulations(nouvelOrdreSimulations);
            return nouvelOrdreSimulations;

        }

        //Méthode utilisée dans les constructeurs des classes filles
        protected void CheckDimension(int dimension)
        {
            if (dimension < 2)
            {
                throw new Exception();
            }
            else
            {
                Dimension = dimension;
            }
        }


        public void AddParameter(CopuleParameter parameter)
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
        /// <summary>
        /// Get a parameter by its name
        /// </summary>
        /// <param name="nomParametre">Name of parameter</param>
        /// <returns>The parameter</returns>
        public virtual CopuleParameter GetParameter(CopuleParameterName nomParametre)
        {
            return ParametresParNom[nomParametre];
        }
        /// <summary>
        /// Return all parameters of the distribution
        /// </summary>
        /// <returns> A ienumerable containing all parameters</returns>
        public virtual IEnumerable<CopuleParameter> AllParameters()
        {
            return ParametresParNom.Values;
        }

        public abstract double DensityCopula(IEnumerable<double> u);
        public abstract double CDFCopula(List<double> u);

        public virtual void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            Dimension = value.Count();
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
        public void Optim(IEnumerable<IEnumerable< double>> values, TypeCalibration typeCalibration)
        {
            var parameters = AllParameters().ToList();
            double[] x = parameters.Select(p => p.Value).ToArray();
            double[] s = Enumerable.Repeat(1.0, x.Length).ToArray();
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
                var valueByPoint= values.IntervertDimention();
                alglib.minbleicoptimize(state, (double[] xx, ref double yy, object zz) => GetSquaredError(valueByPoint,valueByPoint.GetCDF(), xx, ref yy, zz), null, null);
            }
            else
            {
                int nb = values.First().Count();
                var rankVal = values.Select(a => a.Rang().Select(b => (b + 0.5) / nb)).IntervertDimention();
                alglib.minbleicoptimize(state, (double[] xx, ref double yy, object zz) => GetLogVraissemblanceOptim(rankVal, xx, ref yy, zz), null, null);
            }
            alglib.minbleicresults(state, out x, out rep);

            alglib.optguardreport ogrep;
            alglib.minbleicoptguardresults(state, out ogrep);
        }

        private void CreateConstraints(alglib.minbleicstate state)
        {
            var parameters = AllParameters().ToList();
            double[] bndl = parameters.Select(p => p.MinValue).ToArray();
            double[] bndu = parameters.Select(p => p.MaxValue).ToArray();
            alglib.minbleicsetbc(state, bndl, bndu);
        }
        public void GetLogVraissemblanceOptim(IEnumerable<IEnumerable<double>> values, double[] x, ref double func, object obj)
        {
            SetParameter(x);
            func = -GetLogLikelihood(values);
            if (double.IsPositiveInfinity(func))
            {
                func = double.MaxValue;
            }
        }
        public double GetLogLikelihood(IEnumerable<IEnumerable<double>> values)
        {
            double rst = 0;
            foreach (var val in values)
            {
                rst += Math.Log(DensityCopula(val));
            }
            return rst;
        }

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

        private void GetSquaredError(List<List<double>> values,List<double> empiricalCDF, double[] xx, ref double func, object zz)
        {
            SetParameter(xx);
            double increment = 1.0 / (values.Count() - 1);
            double value = increment / 2;
            int index = 0;
            foreach (var val in values)
            {
                var cdf = CDFCopula(val);
                func += (cdf - empiricalCDF[index]) * (cdf - empiricalCDF[index]);
                value += increment;
                index++;
            }
            if (double.IsPositiveInfinity(func))
            {
                func = double.MaxValue;
            }
        }

        public static Copule CreateCopula(TypeCopule typeCopule)
        {
            switch( typeCopule)
            {
                case TypeCopule.Clayton:
                    return new CopuleClayton();
                case TypeCopule.CopuleAMH:
                    return new CopuleAMH();
                default:
                    throw new Exception("Type de copule non reconnu");
            }
        }

    }
}
