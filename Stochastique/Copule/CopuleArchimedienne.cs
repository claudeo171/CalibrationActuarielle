using Stochastique.Distributions;
using Stochastique.Distributions.Continous;



namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    [MessagePack.Union(0,typeof(CopuleAMH))]
    [MessagePack.Union(1, typeof(CopuleClayton))]
    [MessagePack.Union(2, typeof(CopuleFrank))]
    [MessagePack.Union(3, typeof(CopuleGumbel))]
    [MessagePack.Union(4, typeof(CopuleJoe))]
    public abstract class CopuleArchimedienne:Copule
    {
        //C(u,v) = inverseGenerateur(generateur(u)+generateur(v))
        protected abstract double Generateur(double t);
        
        protected abstract double InverseGenerateur(double t);
        //Loi dont la tranformée de Laplace est égale à la fonction "inverseGenerateur"
        [MessagePack.Key(4)]
        protected Distribution Distribution { get; set; }

        [MessagePack.Key(5)]
        public int Dimension { get; set; }
        
        public CopuleArchimedienne(int dimention)
        {
            Dimension = dimention;
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
