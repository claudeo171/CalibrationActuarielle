using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    [MessagePackObject]
    public class GammaDistribution : Distribution
    {
        public GammaDistribution()
        {

        }
        public GammaDistribution(double k, double theta)
        {
            //TODO à verifier
            AddParameter(new Parameter(ParametreName.k, k));
            AddParameter(new Parameter(ParametreName.theta, theta));
        }

        [Key(6)]
        public double K => GetParameter(ParametreName.k).Value;

        [Key(7)]
        public double Theta => GetParameter(ParametreName.theta).Value;

        [Key(8)]
        public override TypeDistribution Type => TypeDistribution.Gamma;
        [IgnoreMember]
        public override double InconditionnalMinimumPossibleValue => 0;

        public override double CDF(double x)
        {
            if(x<0)
            { return 0; }
            return SpecialFunctions.GammaLowerIncomplete(K,x/Theta) / SpecialFunctions.Gamma(K);
        }

        public override double ExpextedValue()
        {
            return K * Theta;
        }

        public override double PDF(double x)
        {
            return Math.Exp(Math.Log(x)* (K-1) -(x/Theta)-Math.Log(Theta)*K -SpecialFunctions.GammaLn(K));
        }

        public override double Variance()
        {
            return K * Theta * Theta;
        }
        public override double Skewness()
        {
            return 2/Math.Sqrt(K);
        }

        public override double Kurtosis()
        {
            return 6.0/K;
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameters(CalibrateWithMoment(value));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(0, 10 * Math.Sqrt(Statistics.Variance(value)));
        }

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            result.Add(new Parameter(ParametreName.theta, Math.Max(0.001, variance / ev)));
            result.Add(new Parameter(ParametreName.k, ev * ev / variance));
            return result;
        }
        public override double Simulate(Random r)
        {
            return base.Simulate(r, 1)[0];
        }
        public override double[] Simulate(Random r,int samples)
        {
            var shape = K;
            var normal=new NormalDistribution(0,1);
            var result = new double[samples];
            if (shape < 1)
            {
                double d = shape + 1.0 - 1.0 / 3.0;
                double c = (1.0 / 3.0) / Math.Sqrt(d);

                for (int i = 0; i < samples; i++)
                    result[i] = Theta * Marsaglia(d, c,r, normal) * Math.Pow(r.NextDouble(), 1.0 / shape);
            }
            else
            {
                double d = shape - 1.0 / 3.0;
                double c = (1.0 / 3.0) / Math.Sqrt(d);

                for (int i = 0; i < samples; i++)
                    result[i] = Theta * Marsaglia(d, c,r, normal);
            }

            return result;
        }
        public static double Marsaglia(double d, double c, Random r, NormalDistribution source)
        {
            // References:
            //
            // - Marsaglia, G. A Simple Method for Generating Gamma Variables, 2000
            //

            while (true)
            {
                // 2. Generate v = (1+cx)^3 with x normal
                double x, t, v;

                do
                {
                    x = source.Simulate(r);
                    t = (1.0 + c * x);
                    v = t * t * t;
                } while (v <= 0);


                // 3. Generate uniform U
                double U = r.NextDouble();

                // 4. If U < 1-0.0331*x^4 return d*v.
                double x2 = x * x;
                if (U < 1 - 0.0331 * x2 * x2)
                    return d * v;

                // 5. If log(U) < 0.5*x^2 + d*(1-v+log(v)) return d*v.
                if (Math.Log(U) < 0.5 * x2 + d * (1.0 - v + Math.Log(v)))
                    return d * v;

                // 6. Goto step 2
            }
        }
    }
}
