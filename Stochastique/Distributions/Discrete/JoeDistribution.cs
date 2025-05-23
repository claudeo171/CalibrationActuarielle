﻿using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class JoeDistribution : DiscreteDistribution
    {
        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            SimulateurRejet = new SimulateurRejet(this, new PartieEntierePuissanceUniformeDistribution(Theta), 1.44269509130226 + 0.52882337976858 / Theta);
        }
        public override TypeDistribution Type => TypeDistribution.Joe;
        [MemoryPack.MemoryPackIgnore]
        public double Theta => GetParameter(ParametreName.theta).Value;
        [MemoryPack.MemoryPackConstructor]
        public JoeDistribution()
        {

        }
        public JoeDistribution(double theta)
        {
            AddParameter(new Parameter(ParametreName.theta, theta));
            SimulateurRejet = new SimulateurRejet(this, new PartieEntierePuissanceUniformeDistribution(theta), 1.44269509130226 + 0.52882337976858 / theta);
            //Voir en cas d'évolution du paramètre pour la gestion 
        }
        [MemoryPack.MemoryPackIgnore]
        private SimulateurRejet SimulateurRejet { get; set; }
        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            throw new NotImplementedException();
        }

        public override double ExpextedValue()
        {
            throw new NotImplementedException();
        }

        public override double Kurtosis()
        {
            throw new NotImplementedException();
        }

        public override double Skewness()
        {
            throw new NotImplementedException();
        }

        public override double Variance()
        {
            throw new NotImplementedException();
        }

        protected override double PDFInt(int k)
        {
            var beta = -1 / Theta - 1;
            return Math.Exp(MathNet.Numerics.SpecialFunctions.GammaLn(beta) + MathNet.Numerics.SpecialFunctions.GammaLn(k - 1 - beta) - MathNet.Numerics.SpecialFunctions.FactorialLn(k));
        }
        public override double Simulate(Random r)
        {
            if(Theta==1)
            {
                return 1;
            }
            else
            {
                return SimulateurRejet.Simuler(r);
            }    
        }
    }
}
