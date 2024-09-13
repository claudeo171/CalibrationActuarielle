using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    public class CopuleGaussienne : Copule
    {
        public override double CDFCopula(List<double> u)
        {
            throw new NotImplementedException();
        }

        public override double DensityCopula(IEnumerable<double> u)
        {
            throw new NotImplementedException();
        }

        public override List<List<double>> SimulerCopule(Random r, int nbSim)
        {
            throw new NotImplementedException();
        }
    }
}
