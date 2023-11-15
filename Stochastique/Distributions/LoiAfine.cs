using LiveChartsCore.Defaults;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    public class LoiAfine : Distribution
    {
        public Distribution LoiBase { get; set; }

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
    }
}
