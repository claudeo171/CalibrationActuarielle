using Stochastique.Distributions.Discrete;
using Stochastique.Enums;
using Stochastique.SpecialFunction;

namespace Stochastique.Copule
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class CopuleJoe : CopuleArchimedienne
    {
        [MemoryPack.MemoryPackConstructor]
        public CopuleJoe() : base(2)
        {
            Type = Enums.TypeCopule.Joe;
        }
        public CopuleJoe(double theta) : base(2)
        {
            Dimension = 2;
            AddParameter(new CopuleParameter(CopuleParameterName.thetaJoe, theta));
            Distribution=new JoeDistribution(theta);
        }
        [MemoryPack.MemoryPackIgnore]
        private double Theta => GetParameter(CopuleParameterName.thetaJoe).Value;
        protected override double Generateur(double t)
        {
            return -Math.Log(1 - Math.Pow(1.0 - t, Theta));
        }
        protected override double InverseGenerateur(double t)
        {
            return 1 - Math.Pow((1 - Math.Exp(-t)), 1.0 / Theta);
        }



        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaJoe, CopuleHelper.RechercheDichotomique(1.00001, 50, (a) => FonctionTau(tau, a))));
            base.Initialize(value, typeCalibration);
            Distribution = new JoeDistribution(GetParameter(CopuleParameterName.thetaJoe).Value);
        }

        private double FonctionTau(double tau, double theta)
        {
            return tau - (1 + 4 / theta *  Debye.gsl_sf_debye_1_e(theta));
        }
        
        public override double DensityCopula(IEnumerable<double> u)
        {
            if (u.Count() != 2) { throw new NotImplementedException(); }
            var a = 1-u.First();
            var b =1- u.Last();
            var atheta = Math.Pow(a, Theta );
            var btheta = Math.Pow(b, Theta );

            return
                Math.Pow(atheta + btheta - atheta * btheta, 1 / Theta - 2) *
                Math.Pow(a, Theta - 1) * Math.Pow(b, Theta - 1) * 
                ( Theta - 1 + atheta + btheta - atheta * btheta);
        }
    }
}
