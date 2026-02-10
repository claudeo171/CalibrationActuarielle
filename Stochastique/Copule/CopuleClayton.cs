using Stochastique.Distributions.Continous;
using Stochastique.Enums;


namespace Stochastique.Copule
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class CopuleClayton:CopuleArchimedienne
    {
        [MemoryPack.MemoryPackIgnore]
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
        [MemoryPack.MemoryPackConstructor]
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

        public override void Initialize(IEnumerable<IEnumerable<double>> value, TypeCalibration typeCalibration)
        {
            double tau = value.First().TauKendall(value.Last());
            AddParameter(new CopuleParameter(CopuleParameterName.thetaClayton, 2 * tau / (1 - tau)));
            base.Initialize(value, typeCalibration);
            Distribution = new GammaDistribution(1 / GetParameter(CopuleParameterName.thetaClayton).Value, GetParameter(CopuleParameterName.thetaClayton).Value);
        }
        public override double DensityCopula(IEnumerable<double> u)
        {
            if (u.Count() != 2) { throw new NotImplementedException(); }
            var a = u.First();
            var b = u.Last();
            return (1+Theta)*(Math.Pow(a*b,-1-Theta))*Math.Pow((-1+Math.Pow(b,-Theta)+ Math.Pow(a,-Theta)),-2-(1/Theta));
        }
    }
}
