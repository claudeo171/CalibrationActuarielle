using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    public class Bound
    {
        public double Position { get; set; }
        public BoundType Type { get; set; }
    }
    public enum BoundType
    {
        debut, fin, end
    }
}
