using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    [MessagePackObject]
    public class Point
    {
        public Point() { }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
        [Key(0)]
        public double X { get; set; }
        [Key(1)]
        public double Y { get; set; }
    }
}
