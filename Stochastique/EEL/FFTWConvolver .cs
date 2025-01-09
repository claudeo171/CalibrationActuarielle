using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NWaves.Transforms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    [MessagePack.MessagePackObject]
    public class FFTWConvolver
    {
        public FFTWConvolver() { }
        public FFTWConvolver(int maximum_input_size)
        {
        }
        public int next2Power(int i)
        {
            int rst = 128;
            while(rst<i)
            {
                rst *= 2;
            }
            return rst;
        }
        public double[] convolve_same_size(int size, int decalage, double[] input_a, double[] input_b)
        {
            
            int padded_size = next2Power(2 * size+2);
            var c_ar = new double[padded_size];
            //var c_ai = new double[padded_size];
            var r_ar = new double[padded_size];
            var r_ai = new double[padded_size];
            var c_br = new double[padded_size];
            //var c_bi = new double[padded_size];
            var r_br = new double[padded_size];
            var r_bi = new double[padded_size];
            for (int i = 0; i < size; i++)
            {
                c_ar[i] = input_a[i];
                c_br[i] = input_b[i + decalage];
            }

            var fft = new RealFft64(padded_size);
            fft.Direct(c_ar, r_ar, r_ai);
            fft.Direct(c_br,  r_br, r_bi);
            for(int i=0;i< padded_size;i++)
            {
                double r= r_ar[i]* r_br[i]- r_ai[i] * r_bi[i];
                double im = r_ar[i] * r_bi[i] + r_ai[i] * r_br[i];
                r_ar[i] = r/ padded_size;
                r_ai[i]= im/ padded_size;
            }
            fft.Inverse(r_ar, r_ai, r_br);
            return r_br.Take(size).ToArray();
            
            /*
            int padded_size = next2Power(2 * size + 2); 
            var c_a = new Complex[padded_size];
            var c_b = new Complex[padded_size];
            for (int i = 0; i < padded_size; i++)
            {
                if (i< size)
                {
                    c_a[i] = new Complex(input_a[i], 0);
                    c_b[i] = new Complex(input_b[i+ decalage], 0);
                }
                else
                {
                    c_a[i] = new Complex(0, 0);
                    c_b[i] = new Complex(0, 0);
                }
            }

            Fourier.Forward(c_a, FourierOptions.NoScaling);
            Fourier.Forward(c_b, FourierOptions.NoScaling);
            
            Complex[] complexes = new Complex[padded_size];
            for (int i = 0; i < complexes.Length; i++)
            {
                complexes[i] = c_a[i] * c_b[i]/ padded_size;
            }
            Fourier.Inverse(complexes, FourierOptions.NoScaling);
            return complexes.Select(a => a.Real).Take(size).ToArray();
            */

        }


    }
}
