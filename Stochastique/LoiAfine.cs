using LiveChartsCore.Defaults;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public class LoiAfine:Distribution
    {
        public Distribution LoiBase { get; set; }

        public LoiAfine(Distribution loiBase,double a, double b)
        {
            LoiBase = loiBase;
            foreach (Parameter param in loiBase.AllParameters())
            {
                AddParameter(param);
            }
            AddParameter(new Parameter(Enums.ParametreName.aAfine, a));
            AddParameter(new Parameter(Enums.ParametreName.bAfine, b));
        }

        public override double PDF(double x)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            throw new NotImplementedException();
        }

        public override double DerivePDF(ParametreName param, double x)
        {
            throw new NotImplementedException();
        }

        public override double DeriveSecondePDF(ParametreName param, double x)
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
    }
}
