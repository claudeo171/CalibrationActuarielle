using MessagePack;
using Stochastique.Distributions;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    public enum CopuleParameterName:int
    {
        thetaClayton,thetaAMH, rho
    }
    [MessagePackObject]
    public class CopuleParameter
    {
        public CopuleParameter() { }
        public CopuleParameter(CopuleParameterName nom, double valeur)
        {
            Value = valeur;
            Name = nom;
        }
        public CopuleParameter(CopuleParameterName nom, double valeur, Copule? estimateur)
        {
            Value = valeur;
            Name = nom;
            Estimateur = estimateur;
        }

        public override string ToString()
        {
            return Name + " : " + Value.ToString("F5");
        }

        [Key(0)]
        public double Value { get; set; }
        [Key(1)]
        public Copule? Estimateur { get; set; }

        [Key(2)]
        public CopuleParameterName Name { get; set; }

        [Key(3)]
        public double MinValue
        {
            get
            {
                switch (Name)
                {
                    case CopuleParameterName.rho:
                        return -1;
                    case CopuleParameterName.thetaClayton:
                        return -1+Math.Pow(10,-10);
                    case CopuleParameterName.thetaAMH:
                        return -1 + Math.Pow(10, -10);
                    default:
                        return double.MinValue;
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
                    case CopuleParameterName.rho:
                        return 1;
                    case CopuleParameterName.thetaClayton:
                        return double.MaxValue;
                    case CopuleParameterName.thetaAMH:
                        return 1 - Math.Pow(10, -10);
                    default:
                        return double.MinValue;
                }
            }
        }
    }
}
