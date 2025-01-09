using Stochastique.Distributions.Continous;
using Stochastique.Enums;
using Expr = MathNet.Symbolics.SymbolicExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Symbolics;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public partial class CopuleClayton:CopuleArchimedienne
    {
        [MessagePack.IgnoreMember]
        private double Theta => GetParameter(CopuleParameterName.thetaClayton).Value;

        public CopuleClayton(int dimension, double theta) : base(dimension)
        {
            communConstructeurs(theta);
            CheckDimension(dimension);
        }

        public CopuleClayton(double theta) : base(2)
        {
            communConstructeurs(theta);
            Dimension = 2;
        }

        public CopuleClayton() : base(2)
        {
            Type = TypeCopule.Clayton;
        }

        private void communConstructeurs(double theta)
        {
            if (theta < -1 || theta == 0)
            {
                throw new Exception("Theta doit être supérieur ou égal à -1 et non nul");
            }

            AddParameter(new CopuleParameter(CopuleParameterName.thetaClayton,theta));
            Distribution = new GammaDistribution(1 / theta, theta);
        }

        protected override double Generateur(double t)
        {
            return (Math.Pow(t, -Theta) - 1) / Theta;
        }

        protected override double InverseGenerateur(double t)
        {
            return Math.Pow(Theta * t + 1, -1 / Theta);
        }

        public override double CDFCopula(List<double> u)
        {
           return Math.Pow(Math.Max(0,u.Sum(a=>Math.Pow(a,-Theta))-1),-1/Theta);
        }

        protected override Expr InverseGenerator(SymbolicExpression param,List<SymbolicExpression> copuleParam)
        {
            return (1 + copuleParam[0] * param).Pow(-1/ copuleParam[0]);
        }
        protected override Expr Generator(SymbolicExpression param, List<SymbolicExpression> copuleParam)
        {
            return 1 / copuleParam[0] * (param.Pow(-copuleParam[0]) - 1);
        }
        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaClayton, 2 * tau / (1 - tau)));
            base.Initialize(value, typeCalibration);
            Distribution = new GammaDistribution(1 / GetParameter(CopuleParameterName.thetaClayton).Value, GetParameter(CopuleParameterName.thetaClayton).Value);
        }
    }
}
