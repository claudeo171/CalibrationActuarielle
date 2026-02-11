using Accord.Math;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Continous
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class StudentDistribution : Distribution
    {
        [MemoryPack.MemoryPackConstructor]
        public StudentDistribution() { }
        public StudentDistribution(int n)
        {
            AddParameter(new Parameter(ParametreName.nStudent, n));
        }
        [MemoryPack.MemoryPackOrder(6)]
        public override TypeDistribution Type => TypeDistribution.Student;

        [MemoryPack.MemoryPackOrder(7)]
        public double n => GetParameter(ParametreName.nStudent).Value;


        public override double CDF(double x)
        {
            double sqrt = Math.Sqrt(x * x + n);
            double u = (x + sqrt) / (2 * sqrt);
            return Beta.Incomplete(n / 2.0, n / 2.0, u);

        }


        public override double PDF(double x)
        {
            return 1/Math.Sqrt(Math.PI * n) * Math.Exp( SpecialFunctions.GammaLn((n + 1) / 2) - SpecialFunctions.GammaLn(n / 2)  + Math.Log(1 + x * x / n)* (-(n + 1) / 2));
        }

        public override double ExpextedValue()
        {
            if (n > 1)
            {
                return 0;
            }
            else
            {
                return double.NaN;
            }
        }

        public override double Variance()
        {
            if (n > 2)
            {
                return n / (n - 2);
            }
            else
            {
                return double.NaN;
            }
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            AddParameters(CalibrateWithMoment(value));

            base.Initialize(value, typeCalibration);    
        }
        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            var variance = value.Variance();
            result.Add(new Parameter(ParametreName.nStudent, Math.Max(3, 2 * variance / (variance - 1))));
            return result;
        }

        public override double Skewness()
        {
            if (n > 3)
            {
                return 0;
            }
            else
            {
                return double.NaN;
            }
        }

        public override double Kurtosis()
        {
            if (n > 4)
            {
                return 6/(n-4);
            }
            else
            {
                return double.NaN;
            }
        }

        public override double[] Simulate(Random r, int nbSimulations)
        {
            var distributionNormal=new NormalDistribution(0, 1);
            var distributionGamma =new GammaDistribution(n / 2, 2);
            var x = distributionNormal.Simulate(r, nbSimulations);
            var y = distributionGamma.Simulate(r, nbSimulations);
            for (int i = 0; i < nbSimulations; i++)
            {
                x[i] = x[i] * Math.Sqrt(n / y[i]);
            }
            return x;
        }
    }
}
