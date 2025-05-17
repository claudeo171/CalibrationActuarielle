using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    [MemoryPack.MemoryPackable(MemoryPack.SerializeLayout.Explicit)]
    [MemoryPack.MemoryPackUnion(0,typeof(BinomialDistribution))]
    [MemoryPack.MemoryPackUnion(1, typeof(BernouliDistribution))]
    [MemoryPack.MemoryPackUnion(2, typeof(GeometricDistribution))]
    [MemoryPack.MemoryPackUnion(3, typeof(HyperGeometricalDistribution))]
    [MemoryPack.MemoryPackUnion(4, typeof(LogarithmiqueDistribution))]
    [MemoryPack.MemoryPackUnion(5, typeof(NegativeBinomialDistribution))]
    [MemoryPack.MemoryPackUnion(6, typeof(PascalDistribution))]
    [MemoryPack.MemoryPackUnion(7, typeof(PoissonDistribution))]
    [MemoryPack.MemoryPackUnion(8, typeof(JoeDistribution))]
    [MemoryPack.MemoryPackUnion(9, typeof(PartieEntierePuissanceUniformeDistribution))]
    public  abstract partial class DiscreteDistribution : Distribution
    {
        public override bool IsDiscreet => true;

        [MemoryPack.MemoryPackIgnore]
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
            if (Math.Abs(Math.Round(x) - x) < 1e-10 && x < int.MaxValue)
            {
                return PDFInt((int)x);
            }
            return 0;
        }
        [MemoryPack.MemoryPackIgnore]
        public List<double> ProbabilitesCummulees { get; set; }

        private void CalculerProbabiliteCummulees()
        {
            List<double> probabilites = new List<double>();
            if (ProbabilitesCummulees == null)
            {
                double probabilite = 0;
                int i = 0;
                ProbabilitesCummulees = new List<double>();
                while (probabilite < 1 - 1e-5 && (probabilite<0.99 || PDF(i)!=0))
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
