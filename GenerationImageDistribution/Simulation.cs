using MathNet.Numerics.Distributions;

namespace GenerationImageDistribution
{
    public static class Simulation
    {
        public static Random Random { get; set; } = new Random(259382390);
        public static double[] SimulerNormale(int nbSim,double mean, double stdd)
        {
            Normal normal = new Normal(mean, stdd,Random);
            double[] sim=new double[nbSim];
            normal.Samples(sim);
            return sim;
        }
        public static double[] SimulerStudent(int nbSim, int n)
        {
            StudentT normal = new StudentT(0,1,n, Random);
            double[] sim = new double[nbSim];
            normal.Samples(sim);
            return sim;
        }

    }
}