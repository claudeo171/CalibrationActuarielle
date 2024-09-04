using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    public class CopuleFrank : CopuleArchimedienne
    {
        public double Theta => GetParameter(CopuleParameterName.thetaFrank).Value;
        public CopuleFrank(double theta)
        {
            AddParameter(new CopuleParameter(CopuleParameterName.thetaFrank, theta));
        }
        protected override double Generateur(double t)
        {
            return -Math.Log((Math.Exp(-Theta * t) - 1) / (Math.Exp(-Theta) - 1));
        }

        protected override double InverseGenerateur(double t)
        {
            return -1/Theta * Math.Log(1+Math.Exp(-t)*(Math.Exp(-Theta)-1));
        }

        protected override double InverseGenerateurDerivate(double t, int ordre)
        {
            throw new NotImplementedException();
        }
    }
}
