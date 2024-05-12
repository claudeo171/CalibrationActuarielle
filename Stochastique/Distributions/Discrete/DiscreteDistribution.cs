using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    public abstract class DiscreteDistribution : Distribution
    {
        [MessagePack.IgnoreMember]
        public override bool IsDiscreet => true;

        [MessagePack.IgnoreMember]
        protected virtual double MaxValue => double.MaxValue;
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
                    rst += PDF(i);
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
        [MessagePack.Key(11)]
        public List<double> ProbabilitesCummulees { get; set; }

        private void CalculerProbabiliteCummulees()
        {
            if (ProbabilitesCummulees == null)
            {
                double probabilite = 0;
                int i = 0;
                ProbabilitesCummulees = new List<double>();
                while (probabilite < 1 - 1e-15)
                {
                    probabilite += PDF(i);
                    ProbabilitesCummulees.Add(probabilite);
                    i++;
                }
            }
        }
        public override double InverseCDF(double x)
        {
            if (x <= 0 || x >= 1)
            {
                throw new ArgumentException("Le paramètre doit être compris entre 0 et 1");
            }
            CalculerProbabiliteCummulees();
            int indmin = 0;
            int indmax = ProbabilitesCummulees.Count-1;
            int indmoy = 0;
            while(x >= ProbabilitesCummulees[indmin] && x >= ProbabilitesCummulees[indmin+1])
            {
                indmoy=(indmin+indmax)/2;
                if ((ProbabilitesCummulees[indmin]-x)*(ProbabilitesCummulees[indmoy]-x)>0)
                {
                    indmin = indmoy;
                }
                else
                {
                    indmax = indmoy;
                }
            }
            return x < ProbabilitesCummulees[indmin] ? indmin: indmin +1;
        }

        /// <summary>
        /// Value of mass distribution for int for discrete distribution
        /// </summary>
        /// <param name="k">the value of the absice</param>
        /// <returns></returns>
        protected abstract double PDFInt(int k);

    }
}
