﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pax.BlazorChartJs
{
    public record MatrixDataset:ChartJsDataset
    {
        public IndexableOption<string>? BackgroundColor { get; set; }
    }

}
