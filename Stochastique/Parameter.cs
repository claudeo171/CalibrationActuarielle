using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public class Parameter
    {
        public Parameter(NomParametre nom, double valeur ) {
            Value = valeur;
            NomParametre = nom;
        }
        public Parameter(NomParametre nom, double valeur, LoiProbabilite? estimateur)
        {
            Value = valeur;
            NomParametre = nom;
            Estimateur = estimateur;
        }
        public double Value { get; set; }
        public LoiProbabilite? Estimateur { get; set; }

        public NomParametre NomParametre { get; set; }

        public double MinValue
        {
            get
            {
                switch (NomParametre)
                {
                    case NomParametre.aAfine:
                        return double.MinValue;
                    case NomParametre.bAfine:
                        return double.MinValue;
                    case NomParametre.mu:
                        return double.MinValue;
                    case NomParametre.sigma:
                        return 0;
                    case NomParametre.n:
                        return 0;
                    default:
                        return 0;
                }
            }
        }
        public double MaxValue
        {
            get
            {
                switch (NomParametre)
                {
                    case NomParametre.aAfine:
                        return double.MaxValue;
                    case NomParametre.bAfine:
                        return double.MaxValue;
                    case NomParametre.mu:
                        return double.MaxValue;
                    case NomParametre.sigma:
                        return double.MaxValue;
                    case NomParametre.n:
                        return double.MaxValue;
                    default:
                        return 0;
                }
            }
        }
    }
}
