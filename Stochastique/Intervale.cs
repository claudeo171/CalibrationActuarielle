using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    [MessagePackObject]
    public class Intervale
    {
        [Key(0)]
        public double Min { get; set; }
        [Key(1)]
        public double Max { get; set; }
        [Key(2)]
        public double Step { get; set; }
        [Key(3)]
        public bool IsDiscreet { get; set; }

        public Intervale(double min, double max) 
        {
            Min = min;
            Max = max;        
        }
    }
}
