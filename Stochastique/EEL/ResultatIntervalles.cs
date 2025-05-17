using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class ResultatIntervalles
    {
        [MemoryPack.MemoryPackOrder(0)]
        public double[] BorneInf { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public double[] BorneSup { get; set; }
        [MemoryPack.MemoryPackOrder(2)]
        public double[] Esperance { get; set; }
        [MemoryPack.MemoryPackOrder(3)]
        public double Alpha { get; set; }
        [MemoryPack.MemoryPackOrder(4)]
        public double Eta { get; set; }
        [MemoryPack.MemoryPackOrder(5)]
        public int N { get; set; }
    }
}
