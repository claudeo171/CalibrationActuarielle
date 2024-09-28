using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Vecteur
{
    [MessagePack.MessagePackObject]
    [MessagePack.Union(0, typeof(VecteurGaussien))]
    public abstract class VecteurAleatoire
    {
        [MessagePack.Key(0)]
        public int Dimension { get; set; }

        public abstract List<List<double>> Simuler(Random random, int nbSim);

        public List<List<double>> SimulerRealisation(Random random)
        {
            return Simuler(random,1);
        }
    }
}
