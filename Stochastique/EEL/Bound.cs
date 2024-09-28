using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    [MessagePackObject]
    public class Bound
    {
        [Key(0)]
        public double Position { get; set; }
        [Key(1)]
        public BoundType Type { get; set; }
    }
    public enum BoundType
    {
        debut, fin, end
    }
}
