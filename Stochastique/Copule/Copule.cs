using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
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
        [MessagePack.Key(5)]
        public List<CopuleParameter> ParametersList { get; set; }
        [MessagePack.Key(6)]
        public int Dimension { get; set; }
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

        public abstract double DensityCopula(List<double> u);
        public abstract double CDFCopula(List<double> u);
    }
}
