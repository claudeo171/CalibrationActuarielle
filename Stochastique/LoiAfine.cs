using LiveChartsCore.Defaults;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public class LoiAfine:LoiProbabilite
    {
        public LoiProbabilite LoiBase { get; set; }

        public LoiAfine(LoiProbabilite loiBase,double a, double b)
        {
            LoiBase = loiBase;
            foreach (Parameter param in loiBase.AllParameters())
            {
                AddParameter(param);
            }
            AddParameter(new Parameter(Enums.NomParametre.aAfine, a));
            AddParameter(new Parameter(Enums.NomParametre.bAfine, b));
        }

        public override double PDF(double x)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            throw new NotImplementedException();
        }

        public override double DerivePDF(NomParametre param, double x)
        {
            throw new NotImplementedException();
        }

        public override double DeriveSecondePDF(NomParametre param, double x)
        {
            throw new NotImplementedException();
        }

        public override double CDF(double x)
        {
            throw new NotImplementedException();
        }
    }
}
