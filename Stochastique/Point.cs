using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class Point
    {
        [MemoryPack.MemoryPackConstructor]
        public Point() { }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
        [MemoryPack.MemoryPackOrder(0)]
        public double X { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public double Y { get; set; }
    }
}
