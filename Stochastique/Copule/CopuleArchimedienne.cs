using Stochastique.Distributions;
using Stochastique.Distributions.Continous;



namespace Stochastique.Copule
{
    [MemoryPack.MemoryPackable(MemoryPack.SerializeLayout.Explicit)]
    [MemoryPack.MemoryPackUnion(0,typeof(CopuleAMH))]
    [MemoryPack.MemoryPackUnion(1, typeof(CopuleClayton))]
    [MemoryPack.MemoryPackUnion(2, typeof(CopuleFrank))]
    [MemoryPack.MemoryPackUnion(3, typeof(CopuleGumbel))]
    [MemoryPack.MemoryPackUnion(4, typeof(CopuleJoe))]
    public abstract partial class CopuleArchimedienne:Copule
    {
        //C(u,v) = inverseGenerateur(generateur(u)+generateur(v))
        protected abstract double Generateur(double t);
        
        protected abstract double InverseGenerateur(double t);
        //Loi dont la tranformée de Laplace est égale à la fonction "inverseGenerateur"
        [MemoryPack.MemoryPackOrder(4)]
        protected Distribution Distribution { get; set; }

        [MemoryPack.MemoryPackOrder(5)]
        public int Dimension { get; set; }
        
        public CopuleArchimedienne(int dimention)
        {
            Dimension = dimention;
        }
        [MemoryPack.MemoryPackConstructor]
        public CopuleArchimedienne()
        {

        }


        public override List<List<double>> SimulerCopule(Random r, int nbSim)
        {
            double[] N = Distribution.Simulate(r,nbSim);
            List<List<double>> uniformes = new List<List<double>>();
            ExponentialDistribution loiExp1 = new ExponentialDistribution(1);

            for (int i = 0; i < Dimension; i++)
            {
                uniformes.Add(loiExp1.Simulate(r, nbSim).Select((a, i) => InverseGenerateur( a / N[i])).ToList());
            }

            return uniformes;
        }

        public override double CDFCopula(List<double> u)
        {
            return InverseGenerateur(u.Sum(a => Generateur(a)));
        }
    }
}
