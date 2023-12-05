﻿using LiveChartsCore.Defaults;
using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    [MessagePackObject]
    public class LoiAfine : Distribution
    {
        [Key(6)]
        public Distribution LoiBase { get; set; }

        [Key(7)]
        public override TypeDistribution Type => throw new NotImplementedException();

        public LoiAfine(Distribution loiBase, double a, double b)
        {
            LoiBase = loiBase;
            foreach (Parameter param in loiBase.AllParameters())
            {
                AddParameter(param);
            }
            AddParameter(new Parameter(ParametreName.aAfine, a));
            AddParameter(new Parameter(ParametreName.bAfine, b));
        }

        public override double PDF(double x)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            throw new NotImplementedException();
        }


        public override double CDF(double x)
        {
            throw new NotImplementedException();
        }

        public override double InverseCDF(double x)
        {
            throw new NotImplementedException();
        }

        public override double ExpextedValue()
        {
            return LoiBase.ExpextedValue() * GetParameter(ParametreName.aAfine).Value + GetParameter(ParametreName.bAfine).Value;
        }

        public override double Variance()
        {
            return LoiBase.Variance() * GetParameter(ParametreName.aAfine).Value * GetParameter(ParametreName.aAfine).Value;
        }

        public override double Skewness()
        {
            return LoiBase.Skewness();
        }

        public override double Kurtosis()
        {
            return LoiBase.Kurtosis();
        }
    }
}
