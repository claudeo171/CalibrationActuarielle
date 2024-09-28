using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    [MessagePack.MessagePackObject]
    public class PoissonPMFGenerator
    {
        public PoissonPMFGenerator(int max_k)
        {
            MaxK = max_k;
            log_gamma_LUT = new double[max_k + 2];
            pmf_array_ptr = new double[max_k + 1];
            for (int i = 0; i < max_k + 2; ++i)
            {
                log_gamma_LUT[i] = SpecialFunctions.GammaLn(i);
            }
            for (int i = 0; i < max_k + 1; ++i)
            {
                pmf_array_ptr[i] = 0;
            }
        }
        public double evaluate_pmf(double lambda, int k)
        {
            if (lambda == 0)
            {
                return k == 0 ? 1.0 : 0.0;
            }
            return Math.Exp(-lambda + k * Math.Log(lambda) - log_gamma_LUT[k + 1]);
        }
        public void compute_array(int k, double lambda)
        {
            if (lambda < 0)
            {
                throw new Exception("Expecting lambda>0 in PoissonPMFGenerator::compute_array()");
            }
            if (lambda == 0)
            {
                pmf_array_ptr[0] = 1;
                for (int i = 1; i < k + 1; ++i)
                {
                    pmf_array_ptr[i] = 0;
                }
                return;
            }
            double log_lambda = Math.Log(lambda);
            for (int i = 0; i < k + 1; ++i)
            {
                pmf_array_ptr[i] = Math.Exp(-lambda + i * log_lambda - log_gamma_LUT[i + 1]);
            }
        }

        [MessagePack.Key(0)]
        private int MaxK;
        [MessagePack.Key(1)]
        private double[] log_gamma_LUT;
        [MessagePack.Key(2)]
        public double[] pmf_array_ptr;
    }
}
