// See https://aka.ms/new-console-template for more information
using Accord.Statistics;
using OnlineCalibrator.Batch;
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using System.Text;

if (false)
{
    if (args.Length < 3)
    {
        Console.WriteLine("Le nombre d'argument n'est pas bon");
    }
    var path = args[0];
    var variable = args[1];
    int nombreSimulation = Convert.ToInt32(args[2]);
    var data = FileService.GetDataFromFile(new FileStream(path, FileMode.Open, FileAccess.Read), Path.GetFileName(path));
    data.NomData = variable;
    var calibratedDistribution = data.ActualData.CalibratedDistribution;
    var random = new Random();
    var distributions = new Distribution[nombreSimulation];
    for (int i = 0; i < nombreSimulation; i++)
    {
        var elts = calibratedDistribution.Simulate(random, data.ActualData.Values.Length);
        DonneesAAnalyser d = new DonneesAAnalyser();
        d.Values = elts;
        d.IsDiscreteDistribution = data.ActualData.IsDiscreteDistribution;
        d.IncludeTrunkatedDistributions = data.ActualData.IncludeTrunkatedDistributions;
        d.GetAllDistributions();
        d.ChangeSelectionMethod(data.ActualData.MethodeCalibration);
        distributions[i] = d.CalibratedDistribution;
    }
    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < nombreSimulation; i++)
    {

        sb.Append($"Simulation {i + 1};{distributions[i].Type};");
        foreach (var param in distributions[i].AllParameters())
        {
            sb.Append($"{param.Name};{param.Value};");
        }
        sb.AppendLine("");
    }
    File.WriteAllText("result.csv", sb.ToString());
}
else
{

    List<Distribution> distributions = new List<Distribution> { new BetaDistribution(2, 2),new UniformDistribution(0, 1), new StudentDistribution(7), new GammaDistribution(4, 5), new GammaDistribution(1, 5), new Khi2Distribution(4), new NormalDistribution(0.1,1), new NormalDistribution(0,1.1) };
    var rst = new List<ResultPuissance>[distributions.Count];
    Parallel.For(0, distributions.Count, new ParallelOptions { MaxDegreeOfParallelism = 1 }, i =>
    {
        rst[i]=OnlineCalibrator.Batch.TestHelper.CalculerPuissance(new NormalDistribution(0,1),distributions[i], 0.05, new Random());
        Console.WriteLine($"La distribution {distributions[i]} a été testé");
    });
    var stringResult = new StringBuilder();
    stringResult.AppendLine("Distribution Testé;Distribution Simulé ;samplesize;alpha;power;powerReference;Risk");
    foreach(var resultat in rst)
    {
        foreach (var v in resultat)
        {
            stringResult.AppendLine($"{v.DistributionTeste};{v.DistributionSimule};{v.SizeSample};{v.Alpha};{v.Puissance};{v.PuissanceTestReference};{v.RisquePremierEspece}");
        }
    }
    File.WriteAllText("./ResultatTest.csv", stringResult.ToString());
}


