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
    public class NormalDistribution : Distribution
    {
        public NormalDistribution()
        {

        }

        public override double CDF(double x)
        {
            return 0.5 *(1+SpecialFunctions.Erf((x - GetParameter(ParametreName.mu).Value) / (GetParameter(ParametreName.sigma).Value * Constants.Sqrt2)));
        }

        public override double InverseCDF(double x)
        {
            return SpecialFunctions.ErfInv(2*x-1)*GetParameter(ParametreName.sigma).Value*Constants.Sqrt2+GetParameter(ParametreName.mu).Value;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double mu = 0;
            double sigma = 0;
            mu = value.Sum() / value.Count();
            sigma = Math.Sqrt(value.Sum(a => a * a) / value.Count() - mu * mu);
            AddParameter(new Parameter(ParametreName.mu, mu));
            AddParameter(new Parameter(ParametreName.sigma, sigma));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(mu - 5 * sigma, mu + 5 * sigma);
            
        }

        public override double PDF(double x)
        {
            return Math.Exp(-(x - GetParameter(ParametreName.mu).Value) * (x - GetParameter(ParametreName.mu).Value) / (2 * GetParameter(ParametreName.sigma).Value * GetParameter(ParametreName.sigma).Value)) / (Math.Sqrt(Math.PI * 2) * GetParameter(ParametreName.sigma).Value);
        }

        public override double DerivePDF(ParametreName param, double x)
        {
            var xMoinMu = x - GetParameter(ParametreName.mu).Value;
            var sigma = GetParameter(ParametreName.sigma).Value; 
            if(param== ParametreName.mu)
            {
                return xMoinMu / (sigma * sigma) * PDF(x);
            }
            else if(param == ParametreName.sigma)
            {
                return (-xMoinMu * xMoinMu - sigma* sigma) / (sigma * sigma * sigma) * PDF(x);
            }
            return 0;
        }

        public override double DeriveSecondePDF(ParametreName param, double x)
        {
            var xMoinMu = x - GetParameter(ParametreName.mu).Value;
            var sigma = GetParameter(ParametreName.sigma).Value;
            if (param == ParametreName.mu)
            {
                return (- sigma * sigma + xMoinMu * xMoinMu) / (sigma * sigma * sigma * sigma) * PDF(x);
            }
            else if (param == ParametreName.sigma)
            {
                return (xMoinMu * xMoinMu * xMoinMu * xMoinMu - 5 * sigma * sigma * xMoinMu * xMoinMu + 2* sigma * sigma * sigma * sigma) / (sigma * sigma * sigma * sigma * sigma * sigma ) * PDF(x);
            }
            return 0;
        }

    }
}
