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
    public class BetaDistribution : Distribution
    {
        [Key(6)]
        public override TypeDistribution Type => TypeDistribution.Beta;

        [Key(7)]
        public double A => GetParameter(ParametreName.aBeta).Value;

        [Key(8)]
        public double B => GetParameter(ParametreName.bBeta).Value;
        [IgnoreMember]
        public override double InconditionnalMinimumPossibleValue => 0;
        [IgnoreMember]
        public override double InconditionnalMaximumPossibleValue => 1;

        public override double CDF(double x)
        {
            if(x<=0) return 0;
            if(x>=1) return 1;
            return Accord.Math.Beta.Incomplete(A, B, x);
        }

        public override double InverseCDF(double x)
        {
            try
            {
                return Accord.Math.Beta.IncompleteInverse(A, B, x);
            }
            catch(Exception e)
            {
                return 0;
            }
        }

        public override double ExpextedValue()
        {
            return A / (A + B);
        }

        public override double PDF(double x)
        {
            return (x>1 || x<0)? 0 : 1 / SpecialFunctions.Beta(A, B) * Math.Pow(x,A-1) * Math.Pow(1-x, B - 1);
        }

        public override double Variance()
        {
            return A*B / ((A + B)* (A + B)+ (A + B + 1));
        }
        public override double Skewness()
        {
            return 2*(B-A)*Math.Sqrt(A+B+1)/((A+B+2)*Math.Sqrt(A*B));
        }

        public override double Kurtosis()
        {
            return 6* ((B - A)* (B - A)*(B + A + 1)-A*B*(A+B+2))/(A*B*(A+B+2)*(A+B+3));
        }

        public override void Initialize(IEnumerable<double> value, TypeCalibration typeCalibration)
        {
            double k = 0;

            AddParameters( CalibrateWithMoment(value));
            base.Initialize(value, typeCalibration);
            IntervaleForDisplay = new Intervale(Math.Max(0, k - 10 * k), k + 10 * k);
        }

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> value)
        {
            List<Parameter> result = new List<Parameter>();
            var ev = Statistics.Mean(value);
            var variance = Statistics.Variance(value);
            alglib.complex[] rst = new alglib.complex[3];
            var repot = new alglib.polynomialsolver.polynomialsolverreport();
            alglib.xparams xparams = new alglib.xparams(1);
            alglib.polynomialsolver.polynomialsolve(new double[4] { 0, 0, variance / ev - 1 + ev, variance / ev / ev }, 3, ref rst, repot, xparams);
            result.Add(new Parameter(ParametreName.aBeta, Math.Max(1e-6, rst.Where(a => a.y == 0 && a.x != 0).Min(a => a.x))));
            result.Add(new Parameter(ParametreName.bBeta, Math.Max(1e-6, result[0].Value * (1 - ev) / ev)));
            return result;
        }

        public override double[] Simulate(Random r, int nbSimulations)
        {
            var gamma1=new GammaDistribution(A, 1).Simulate(r,nbSimulations);
            var gamma2 = new GammaDistribution(B, 1).Simulate(r, nbSimulations);
            return gamma1.Select((a, i) => a / (a + gamma2[i])).ToArray();
        }
        public override double Simulate(Random r)
        {
            return base.Simulate(r, 1)[0];
        }
    }
}
