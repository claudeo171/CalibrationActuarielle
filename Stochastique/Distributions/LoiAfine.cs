using LiveChartsCore.Defaults;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class LoiAfine : Distribution
    {
        [MemoryPack.MemoryPackOrder(6)]
        public Distribution LoiBase { get; set; }

        [MemoryPack.MemoryPackOrder(7)]
        public override TypeDistribution Type => throw new NotImplementedException();
        [MemoryPack.MemoryPackConstructor]
        public LoiAfine() { }
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
            return LoiBase.PDF((x - GetParameter(ParametreName.bAfine).Value) / GetParameter(ParametreName.aAfine).Value) / GetParameter(ParametreName.aAfine).Value;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            throw new NotImplementedException();
        }


        public override double CDF(double x)
        {
            return LoiBase.CDF((x - GetParameter(ParametreName.bAfine).Value) / GetParameter(ParametreName.aAfine).Value);
        }

        public override double InverseCDF(double x)
        {
            return LoiBase.InverseCDF(x) * GetParameter(ParametreName.aAfine).Value + GetParameter(ParametreName.bAfine).Value;
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
        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            var param = LoiBase.CalibrateWithMoment(values).ToList();
            param.Add(new Parameter(ParametreName.aAfine, 1));
            param.Add(new Parameter(ParametreName.bAfine, 0));
            return param;
        }
    }
}
