﻿using MessagePack;
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
    [MessagePackObject]
    public class CopuleWithData
    {
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

        [Key(0)]
        public double N { get; set; }
        [Key(1)]
        public double LogLikelihood { get; set; }

        [Key(2)]
        public double AIC => 2 * Copule.AllParameters().Count() - 2 * LogLikelihood;

        [Key(3)]
        public double BIC => Math.Log(N) * Copule.AllParameters().Count() - 2 * LogLikelihood;


        [Key(5)]
        public Copule Copule { get; set; }
        [Key(6)]
        public TypeCalibration Calibration { get; set; }

        [Key(7)]
        public string? Comment { get; set; }
        [Key(8)]
        public float ProbabiliteMachineLearningImage { get; internal set; }
    }
}
