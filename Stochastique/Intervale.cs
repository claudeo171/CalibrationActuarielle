using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class Intervale
    {
        [MemoryPack.MemoryPackOrder(0)]
        public double Min { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public double Max { get; set; }
        [MemoryPack.MemoryPackOrder(2)]
        public double Step { get; set; }
        [MemoryPack.MemoryPackOrder(3)]
        public bool IsDiscreet { get; set; }

        public Intervale(double min, double max) 
        {
            Min = min;
            Max = max;        
        }
    }
}
