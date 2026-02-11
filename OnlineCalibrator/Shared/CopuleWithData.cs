using Stochastique.Copule;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class CopuleWithData
    {
        [MemoryPack.MemoryPackConstructor]
        public CopuleWithData()
        {
        }
        public CopuleWithData(Copule distribution, IEnumerable<IEnumerable<double>> data)
        {
            Copule = distribution;
            int nb = data.First().Count();
            LogLikelihood = distribution.GetLogLikelihood(data.Select(a => a.Rang().Select(b => (b + 0.5) / nb)).IntervertDimention());
            N = data.First().Count();
        }

        [MemoryPack.MemoryPackOrder(0)]
        public double N { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public double LogLikelihood { get; set; }

        [MemoryPack.MemoryPackOrder(2)]
        public double AIC => 2 * Copule.AllParameters().Count() - 2 * LogLikelihood;

        [MemoryPack.MemoryPackOrder(3)]
        public double BIC => Math.Log(N) * Copule.AllParameters().Count() - 2 * LogLikelihood;


        [MemoryPack.MemoryPackOrder(5)]
        public Copule Copule { get; set; }
        [MemoryPack.MemoryPackOrder(6)]
        public TypeCalibration Calibration { get; set; }

        [MemoryPack.MemoryPackOrder(7)]
        public string? Comment { get; set; }
        [MemoryPack.MemoryPackOrder(8)]
        public float ProbabiliteMachineLearningImage { get; internal set; }
    }
}
