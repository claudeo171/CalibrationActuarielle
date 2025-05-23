﻿using Stochastique.Distributions;
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
        thetaClayton,thetaAMH,thetaFrank,thetaGumbel, thetaJoe, rho
    }
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class CopuleParameter
    {
        [MemoryPack.MemoryPackConstructor]
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

        [MemoryPack.MemoryPackOrder(0)]
        public double Value { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public Copule? Estimateur { get; set; }

        [MemoryPack.MemoryPackOrder(2)]
        public CopuleParameterName Name { get; set; }

        [MemoryPack.MemoryPackOrder(3)]
        public double MinValue
        {
            get
            {
                switch (Name)
                {
                    case CopuleParameterName.rho:
                        return -1;
                    case CopuleParameterName.thetaClayton:
                        return 0.00001;
                    case CopuleParameterName.thetaAMH:
                        return 0.0001;
                    case CopuleParameterName.thetaFrank:
                        return 0.00001;
                    case CopuleParameterName.thetaGumbel:
                        return 1.001;
                    case CopuleParameterName.thetaJoe:
                        return 1.00001;
                    default:
                        return double.MinValue;
                }
            }
        }

        [MemoryPack.MemoryPackOrder(4)]
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
                        return 1 - Math.Pow(10, -8);
                    case CopuleParameterName.thetaFrank:
                        return 30;
                    case CopuleParameterName.thetaGumbel:
                        return 1e10;
                    case CopuleParameterName.thetaJoe:
                        return 50;
                    default:
                        return double.MinValue;
                }
            }
        }
    }
}
