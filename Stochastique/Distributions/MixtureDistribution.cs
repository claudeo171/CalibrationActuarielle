﻿using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class MixtureDistribution : Distribution
    {
        public override TypeDistribution Type => throw new NotImplementedException();

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            throw new NotImplementedException();
        }

        public override double CDF(double x)
        {
            throw new NotImplementedException();
        }

        public override double ExpextedValue()
        {
            throw new NotImplementedException();
        }

        public override double Kurtosis()
        {
            throw new NotImplementedException();
        }

        public override double PDF(double x)
        {
            throw new NotImplementedException();
        }

        public override double Skewness()
        {
            throw new NotImplementedException();
        }

        public override double Variance()
        {
            throw new NotImplementedException();
        }
    }
}
