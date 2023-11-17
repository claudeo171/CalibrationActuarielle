using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerationImageDistribution
{
    [MessagePackObject]
    public class Point
    {
        [Key(0)]
        public double X { get; set; }
        [Key(1)]
        public double Y { get; set; }
    }
}
