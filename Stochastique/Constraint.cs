using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public class Constraint
    {
        public double Value { get; set; }
        public List<ParametreName> Parametres { get; set; }

        public List<double> Multiplier { get; set; }
        /// <summary>
        /// Contrainte P1-P2 >=0 
        /// </summary>
        /// <param name="parametre1"></param>
        /// <param name="parametre2"></param>
        public Constraint(ParametreName parametre1, ParametreName parametre2)
        {
            Parametres = new List<ParametreName>();
            Parametres.Add(parametre1);
            Parametres.Add(parametre2);
            Multiplier = new List<double>();
            Multiplier.Add(1);
            Multiplier.Add(-1);
            Value = 1e-10;
        }
    }
}
