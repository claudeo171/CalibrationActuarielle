using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pax.BlazorChartJs
{
    public record AnnotationsData
    {
        public IList<Annotation> Annotations { get; set; } = new List<Annotation>();
    }
}
