using Stochastique;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public class DistributionWithDatas
    {
        public DistributionWithDatas(Distribution distribution)
        {
            Distribution = distribution;
        }
        public TypeDistribution TypeDistribution { get; set; }
        public Distribution Distribution { get; set; }
        public TypeCalibration Calibration { get; set; }
        public string SeuilAlphaString
        {
            get
            {
                return SeuilAlpha.ToString();
            }
            set { SeuilAlpha = value.Contains('.')? Convert.ToDouble(value,new CultureInfo("en-US") ): Convert.ToDouble(value, new CultureInfo("fr-FR")); }
        }
        public double SeuilAlpha { get; set; } = 0.05;
        public string? Comment { get; set; }

    }
}
