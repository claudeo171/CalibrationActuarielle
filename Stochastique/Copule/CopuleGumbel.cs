
using Stochastique.Distributions.Continous;
using Stochastique.Enums;


namespace Stochastique.Copule
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class CopuleGumbel : CopuleArchimedienne
    {
        [MemoryPack.MemoryPackIgnore]
        public double Theta => GetParameter(CopuleParameterName.thetaGumbel).Value;
        public CopuleGumbel() : base(2)
        {
            Type = TypeCopule.Gumbel;
            
        }
        protected override double Generateur(double t)
        {
            return Math.Pow(-Math.Log(t),Theta);
        }

        protected override double InverseGenerateur(double t)
        {
            return Math.Exp(-Math.Pow(t,1/Theta));
        }


        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaGumbel, 1/1-tau ));
            base.Initialize(value, typeCalibration);
            Distribution = new StableDistribution(0, Math.Pow(Math.Cos(Math.PI / (2 * Theta)), Theta), 1 / Theta, 1);
        }
        public override double DensityCopula(IEnumerable<double> u)
        {
            if (u.Count() != 2) { throw new NotImplementedException(); }
            var a = u.First();
            var b = u.Last();

            return CDFCopula(u.ToList()) * Math.Pow((Generateur(a) + Generateur(b)),1/Theta-2)*(Theta-1+ Math.Pow((Generateur(a) + Generateur(b)), 1 / Theta))*(Math.Pow(-Math.Log(a), Theta-1)* Math.Pow(-Math.Log(b), Theta-1))/a/b; 
        }
    }
}
