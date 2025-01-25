using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pax.BlazorChartJs
{
    public record MatrixPoint
    {
        public object? X { get; set; }
        public object? Y { get; set; }
        public double V { get; set; }
    }
}
