using Stochastique.Distributions.Discrete;
using Stochastique.Enums;
using Stochastique.SpecialFunction;


namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public partial class CopuleFrank : CopuleArchimedienne
    {
        [MessagePack.IgnoreMember]
        public double Theta => GetParameter(CopuleParameterName.thetaFrank).Value;

        public CopuleFrank() : base(2)
        {
            Type = TypeCopule.Frank;
        }
        public CopuleFrank(double theta) : base(2)
        {
            AddParameter(new CopuleParameter(CopuleParameterName.thetaFrank, theta));
            Distribution = new LogarithmiqueDistribution(1-Math.Exp(-Theta));
        }
        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaFrank, CopuleHelper.RechercheDichotomique(0.00001, 30, (a) => FonctionTau(tau, a))));
            base.Initialize(value, typeCalibration);
            Distribution = new LogarithmiqueDistribution(1 - Math.Exp(-Theta));
        }

        private double FonctionTau(double tau, double theta)
        {
            return tau - (1 - 4 / theta * (1 - Debye.gsl_sf_debye_1_e(theta)));
        }

        protected override double Generateur(double t)
        {
            return -Math.Log((Math.Exp(-Theta * t) - 1) / (Math.Exp(-Theta) - 1));
        }

        protected override double InverseGenerateur(double t)
        {
            return -1/Theta * Math.Log(1+Math.Exp(-t)*(Math.Exp(-Theta)-1));
        }
        public override double DensityCopula(IEnumerable<double> u)
        {
            if (u.Count() != 2) { throw new NotImplementedException(); }
            var a = u.First();
            var b = u.Last();

            return (-Theta) * (Math.Exp(-Theta * (a + b)) * (Math.Exp(-Theta) - 1)) / Math.Pow( (Math.Exp(-Theta)- Math.Exp(-Theta*a)- Math.Exp(-Theta*b)+ Math.Exp(-Theta*(a+b))),2);
        }
    }
}
