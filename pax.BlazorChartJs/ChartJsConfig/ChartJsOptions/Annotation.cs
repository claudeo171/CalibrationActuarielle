using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pax.BlazorChartJs
{
    public record Annotation
    {
        public string? type { get; set; }
        public string? backgroundColor { get; set; }
        public string? borderColor { get; set; }
        public int? borderWidth { get; set; }
        public string? drawTime { get; set; }
        public object? xMax { get; set; }
        public object? xMin { get; set; }
        public string? xScaleID { get; set; }
        public string? yScaleID { get; set; }
        public object? yMax { get; set; }
        public object? yMin { get; set; }
    }
}
