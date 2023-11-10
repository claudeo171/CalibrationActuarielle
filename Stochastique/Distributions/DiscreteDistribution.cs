using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions
{
    public abstract class DiscreteDistribution:Distribution
    {
        protected virtual double MaxValue => 0;
        public override double CDF(double k)
        {
            if (k > MaxValue)
            {
                return 1;
            }
            else
            {
                var rst = 0.0;
                for (int i = 0; i < k; i++)
                {
                    rst += PDF((double)i);
                }
                return rst;
            }
        }
        public override double PDF(double x)
        {
            if (Math.Abs(Math.Round(x) - x) < 1e-10)
            {
                return PDFInt((int)x);
            }
            return 0;
        }
        protected abstract double PDFInt(int k);

    }
}
