using LiveChartsCore.Defaults;
using MathNet.Numerics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public class LoiNormale : LoiProbabilite
    {
        public LoiNormale()
        {

        }

        public override double CDF(double x)
        {
            return 0.5 *(1+SpecialFunctions.Erf((x - GetParameter(NomParametre.mu).Value) / (GetParameter(NomParametre.sigma).Value * Constants.Sqrt2)));
        }

        public override double InverseCDF(double x)
        {
            return SpecialFunctions.ErfInv(2*x-0.5)*GetParameter(NomParametre.sigma).Value*Constants.Sqrt2+GetParameter(NomParametre.mu).Value;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double mu = 0;
            double sigma = 0;
            mu = value.Sum() / value.Count();
            sigma = Math.Sqrt(value.Sum(a => a * a) / value.Count() - mu * mu);
            AddParameter(new Parameter(NomParametre.mu, mu));
            AddParameter(new Parameter(NomParametre.sigma, sigma));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(mu - 5 * sigma, mu + 5 * sigma);
            
        }

        public override double PDF(double x)
        {
            return Math.Exp((x - GetParameter(NomParametre.mu).Value) * (x - GetParameter(NomParametre.mu).Value) / (2 * GetParameter(NomParametre.sigma).Value * GetParameter(NomParametre.sigma).Value)) / (Math.Sqrt(Math.PI * 2) * GetParameter(NomParametre.sigma).Value);
        }

        public override double DerivePDF(NomParametre param, double x)
        {
            var xMoinMu = x - GetParameter(NomParametre.mu).Value;
            var sigma = GetParameter(NomParametre.sigma).Value; 
            if(param== NomParametre.mu)
            {
                return xMoinMu / (sigma * sigma) * PDF(x);
            }
            else if(param == NomParametre.sigma)
            {
                return (-xMoinMu * xMoinMu - sigma* sigma) / (sigma * sigma * sigma) * PDF(x);
            }
            return 0;
        }

        public override double DeriveSecondePDF(NomParametre param, double x)
        {
            var xMoinMu = x - GetParameter(NomParametre.mu).Value;
            var sigma = GetParameter(NomParametre.sigma).Value;
            if (param == NomParametre.mu)
            {
                return (- sigma * sigma + xMoinMu * xMoinMu) / (sigma * sigma * sigma * sigma) * PDF(x);
            }
            else if (param == NomParametre.sigma)
            {
                return (xMoinMu * xMoinMu * xMoinMu * xMoinMu - 5 * sigma * sigma * xMoinMu * xMoinMu + 2* sigma * sigma * sigma * sigma) / (sigma * sigma * sigma * sigma * sigma * sigma ) * PDF(x);
            }
            return 0;
        }

    }
}
