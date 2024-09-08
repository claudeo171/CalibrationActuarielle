using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    public static class HelperEEL
    {
        public static double GetQuantile(double alphaLocal,int n)
        {
            double[] h = new double[n];
            double[] g = new double[n];
            for (int i = 0; i < n; i++)
            {
                h[i] = new Beta(i + 1, n - i).InverseCumulativeDistribution(alphaLocal / 2);
                g[i] = new Beta(i + 1, n - i).InverseCumulativeDistribution(1 - (alphaLocal / 2));
            }
            return GetLevelFromBounds(h, g);
        }
        public static ResultatIntervalles GetIntervals(double alpha, int n, double tol = 1e-8, int max_it = 100)
        {
            var alpha_epsilon = 0.00001;
            double etaMin = alpha / n;
            double etaMax = alpha;
            double etaMoy = (etaMin + etaMax) / 2;
            double[] h = new double[n];
            double[] g = new double[n];
            for (int k = 0; k < max_it; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    h[i] = new Beta(i + 1, n - i).InverseCumulativeDistribution(etaMoy / 2);
                    g[i] = new Beta(i + 1, n - i).InverseCumulativeDistribution(1 - (etaMoy / 2));
                }
                double testAlpha = GetLevelFromBounds(h, g);
                if (Math.Abs(testAlpha - alpha) / alpha <= tol)
                {
                    break;
                }
                if (testAlpha > alpha)
                {
                    etaMax = etaMoy;

                }
                else
                {
                    etaMin = etaMoy;
                }
                etaMoy = (etaMin + etaMax) / 2;
            }
            return new ResultatIntervalles { BorneInf = h, BorneSup = g, Alpha = alpha, Eta = etaMoy, Esperance = Enumerable.Repeat(1.0, n).Select((a, i) => i / (n + 1.0)).ToArray() };
        }


        public static double GetLevelFromBounds(double[] b, double[] B)
        {
            int n = b.Count();

            double[] poisson_nocross_probs = poisson_process_noncrossing_probability(n, n, b, B);

            return 1 - poisson_nocross_probs[n] / poisson_pmf(n, n);
        }

        public static double poisson_pmf(double lambda, int k)
        {
            if (lambda == 0.0)
            {
                return k == 0 ? 1.0 : 0.0;
            }
            return Math.Exp(-lambda + k * Math.Log(lambda) - SpecialFunctions.GammaLn(k + 1));
        }
        public static double[] poisson_process_noncrossing_probability(int n, double intensity, double[] b, double[] B)
        {
            Bound[] bounds = join_all_bounds(b, B);

            DoubleBuffer<double> buffers = new DoubleBuffer<double>(n + 1, 0.0);
            buffers.get_src[0] = 1.0;

            FFTWConvolver fftconvolver = new FFTWConvolver(n + 1);
            PoissonPMFGenerator pmfgen = new PoissonPMFGenerator(n + 1);

            int b_step_count = 0;
            int B_step_count = 0;

            double prev_location = 0.0;

            for (int i = 0; i < bounds.Length; ++i)
            {
                int cur_size = b_step_count - B_step_count + 1;

                double lambda = intensity * (bounds[i].Position - prev_location);
                if (lambda > 0)
                {
                    pmfgen.compute_array(cur_size, lambda);
                    double[] rst;
                    if (cur_size < 80)
                    {
                        rst = convolve_same_size(cur_size, B_step_count, pmfgen.pmf_array_ptr, buffers.get_src);
                    }
                    else
                    {
                        rst = fftconvolver.convolve_same_size(cur_size, B_step_count, pmfgen.pmf_array_ptr, buffers.get_src);

                    }
                    for (int j = 0; j < cur_size; ++j)
                    {
                        buffers.get_dest[j + B_step_count] = rst[j];
                    }

                }

                if (bounds[i].Type == BoundType.debut)
                {
                    ++b_step_count;
                    buffers.get_dest[b_step_count] = 0.0;
                }
                else if (bounds[i].Type == BoundType.fin)
                {
                    buffers.get_dest[B_step_count] = 0.0;
                    ++B_step_count;
                }
                if (lambda > 0)
                {
                    buffers.flip();
                }
                prev_location = bounds[i].Position;
            }
            return buffers.get_src;
        }

        public static double[] convolve_same_size(int size, int decalage, double[] source, double[] source2)
        {
            double[] dest = new double[source.Length] ;
            for (int i = 0; i < size; ++i)
            {
                double convoli = 0.0;
                for (int j = 0; j <= i; ++j)
                {
                    convoli += source[j] * source2[i - j+ decalage];
                }
                dest[i]=convoli;
            }
            return dest;

        }

        static Bound[] join_all_bounds(double[] b, double[] B)
        {
            List<Bound> bounds = new List<Bound>();


            Bound bound;
            for (int i = 0; i <b.Length; ++i)
            {
                bound = new Bound();
                bound.Position = b[i];
                bound.Type = BoundType.debut;
                bounds.Add(bound);
            }

            for (int i = 0; i < B.Length; ++i)
            {
                bound = new Bound();
                bound.Position = B[i];
                bound.Type = BoundType.fin;
                bounds.Add(bound);
            }

            bounds = bounds.OrderBy(a => a.Position).ToList();
            bound = new Bound();
            bound.Position = 1.0;
            bound.Type = BoundType.end;
            bounds.Add(bound);

            return bounds.ToArray();
        }
    }
}
