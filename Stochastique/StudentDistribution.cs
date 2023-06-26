using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public class StudentDistribution : Distribution
    {
        public StudentDistribution(int n)
        {
            
        }

        public override double CDF(double x)
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

        public override double InverseCDF(double x)
        {
            throw new NotImplementedException();
        }

        public override double PDF(double x)
        {
            throw new NotImplementedException();
        }
    }
}
