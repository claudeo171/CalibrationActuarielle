using Stochastique.Distributions.Discrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public class CopuleJoe : CopuleArchimedienne
    {
        public CopuleJoe() { }
        public CopuleJoe(double theta)
        {
            Dimension = 2;
            AddParameter(new CopuleParameter(CopuleParameterName.thetaJoe, theta));
            Distribution=new JoeDistribution(theta);
        }
        [MessagePack.IgnoreMember]
        private double Theta => GetParameter(CopuleParameterName.thetaJoe).Value;
        protected override double Generateur(double t)
        {
            return Math.Log(1 - Math.Pow(1 - t, Theta));
        }

        protected override double InverseGenerateur(double t)
        {
            return 1 - Math.Pow((1 - Math.Exp(-t)), 1 / Theta);
        }

        protected override double InverseGenerateurDerivate(double t, int ordre)
        {
            throw new NotImplementedException();
        }
    }
}
