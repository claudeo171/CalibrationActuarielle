using GenerationImageDistribution;
public class Program
{

    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        for (int i = 0; i < 1000; i++)
        {
            GenerationGraphique.SaveChartImage(GenerationGraphique.GetCDF(Simulation.SimulerNormale(1000, 0, 1)),$"NCDF_{i}");
            GenerationGraphique.SaveChartImage(GenerationGraphique.GetCDF(Simulation.SimulerStudent(1000, 10)), $"T10CDF_{i}");
        }
    }
}
        
