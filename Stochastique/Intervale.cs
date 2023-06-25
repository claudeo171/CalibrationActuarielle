using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public class Intervale
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }
        public bool IsDiscreet { get; set; }

        public Intervale(double min, double max) 
        {
            Min = min;
            Max = max;        
        }
    }
}
