using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    [MessagePackObject]
    public class Parameter
    {
        public Parameter() { }
        public Parameter(ParametreName nom, double valeur)
        {
            Value = valeur;
            Name = nom;
        }
        public Parameter(ParametreName nom, double valeur, Distribution? estimateur)
        {
            Value = valeur;
            Name = nom;
            Estimateur = estimateur;
        }

        public override string ToString()
        {
            return Name + " : " + Value.ToBeautifulString();
        }

        public void SetValue(double v)
        {
            Value = v;
        }

        [Key(0)]
        public double Value { get; set; }
        [Key(1)]
        public Distribution? Estimateur { get; set; }

        [Key(2)]
        public ParametreName Name { get; set; }

        [Key(3)]
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
                        return Math.Pow(10, -100);
                    case ParametreName.qDown:
                        return 0;
                    case ParametreName.qUp:
                        return 0;
                    case ParametreName.p:
                        return Math.Pow(10, -14);
                    case ParametreName.n:
                        return 0;
                    case ParametreName.nStudent:
                        return 1;
                    case ParametreName.a:
                         return double.MinValue;
                    case ParametreName.b:
                        return double.MinValue;
                    case ParametreName.aBeta:
                        return Math.Pow(10, -5);
                    case ParametreName.bBeta:
                        return Math.Pow(10, -5);
                    case ParametreName.aCauchy:
                        return Math.Pow(10, -10);
                    case ParametreName.bCauchy:
                        return double.MinValue;
                    case ParametreName.lambda:
                        return Math.Pow(10, -100);
                    case ParametreName.k:
                        return Math.Pow(10, -100);
                    case ParametreName.theta:
                        return Math.Pow(10, -10);
                    case ParametreName.d1:
                        return 1;
                    case ParametreName.d2:
                        return 1;
                    case ParametreName.N:
                        return 1;
                    case ParametreName.r:
                        return 1;
                    case ParametreName.Np:
                        return 0;
                    default:
                        return 0;
                }
            }
        }

        [Key(4)]
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
                    case ParametreName.p:
                        return 1- Math.Pow(10, -14);
                    case ParametreName.n:
                        return double.MaxValue;
                    case ParametreName.qDown:
                        return 1;
                    case ParametreName.qUp:
                        return 1;
                    case ParametreName.lambda:
                        return double.MaxValue;
                    case ParametreName.a:
                        return double.MaxValue;
                    case ParametreName.b:
                        return double.MaxValue;
                    case ParametreName.nStudent:
                        return 100000;
                    case ParametreName.aCauchy:
                        return double.MaxValue;
                    case ParametreName.bCauchy:
                        return double.MaxValue;
                    case ParametreName.d1:
                        return double.MaxValue;
                    case ParametreName.d2:
                        return double.MaxValue;
                    case ParametreName.k:
                        return double.MaxValue;
                    case ParametreName.N:
                        return double.MaxValue;
                    case ParametreName.Np:
                        return double.MaxValue;
                    case ParametreName.r:
                        return double.MaxValue;
                    case ParametreName.theta:
                        return double.MaxValue;
                    case ParametreName.aBeta:
                    case ParametreName.bBeta:
                        return double.MaxValue;
                    default:
                        return 0;
                }
            }
        }

        public static List<Constraint> Contraints
        {
            get
            {
                return new List<Constraint>
                {
                    new Constraint(ParametreName.qUp, ParametreName.qDown)
                };
            }
        }
    }
}
