using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Autre
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class ValIndice
    {
        [MemoryPack.MemoryPackOrder(0)]
        public int indice = 0;
        [MemoryPack.MemoryPackOrder(1)]
        public double valeur = 0;
        [MemoryPackConstructor]
        public ValIndice() { }
        public ValIndice(int ind, double val)
        {
            indice = ind;
            valeur = val;
        }

        public static int comparaison(ValIndice v1, ValIndice v2)
        {
            int ret = 0;
            if (v1.valeur == v2.valeur)
            {
                ret = 0;
            }
            else if (v1.valeur > v2.valeur)
            {
                ret = 1;
            }
            else
            {
                ret = -1;
            }
            return ret;
        }
    }
}
