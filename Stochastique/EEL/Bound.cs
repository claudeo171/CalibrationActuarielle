using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class Bound
    {
        [MemoryPack.MemoryPackOrder(0)]
        public double Position { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public BoundType Type { get; set; }
    }
    public enum BoundType
    {
        debut, fin, end
    }
}
