using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public class CopuleGumbel : CopuleArchimedienne
    {
        [MessagePack.IgnoreMember]
        public double Theta => GetParameter(CopuleParameterName.thetaGumbel).Value;
        protected override double Generateur(double t)
        {
            return Math.Pow(-Math.Log(t),Theta);
        }

        protected override double InverseGenerateur(double t)
        {
            return Math.Exp(Math.Pow(-t,1/Theta));
        }

        protected override double InverseGenerateurDerivate(double t, int ordre)
        {
            throw new NotImplementedException();
        }
    }
}
