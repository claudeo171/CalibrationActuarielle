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
        public Parameter(ParametreName nom, double valeur ) {
            Value = valeur;
            Name = nom;
        }
        public Parameter(ParametreName nom, double valeur, Distribution? estimateur)
        {
            Value = valeur;
            Name = nom;
            Estimateur = estimateur;
        }
        public double Value { get; set; }
        public Distribution? Estimateur { get; set; }

        public ParametreName Name { get; set; }

        public double MinValue
        {
            get
            {
                switch (Name)
                {
                    case ParametreName.aAfine:
                        return double.MinValue;
                    case ParametreName.bAfine:
                        return double.MinValue;
                    case ParametreName.mu:
                        return double.MinValue;
                    case ParametreName.sigma:
                        return Math.Pow(10,-100);
                    case ParametreName.n:
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
                switch (Name)
                {
                    case ParametreName.aAfine:
                        return double.MaxValue;
                    case ParametreName.bAfine:
                        return double.MaxValue;
                    case ParametreName.mu:
                        return double.MaxValue;
                    case ParametreName.sigma:
                        return double.MaxValue;
                    case ParametreName.n:
                        return double.MaxValue;
                    default:
                        return 0;
                }
            }
        }
    }
}
