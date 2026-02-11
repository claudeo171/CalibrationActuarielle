// See https://aka.ms/new-console-template for more information
using Accord.Statistics;
using MathNet.Numerics.Statistics;
using OnlineCalibrator.Batch;
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using System.Text;



ErreurModeleRetraitement retraitement = new ErreurModeleRetraitement();
retraitement.Import();
retraitement.Calculer(1000);
retraitement.CalibrateLoiNombre();
retraitement.CalibrateLoiCout();
retraitement.Export();
if (args.Length == 3)
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
        d.ValeurMinTrukated = elts.Min();
        d.ValeurMinTrukated = elts.Max();
        d.IsDiscreteDistribution = data.ActualData.IsDiscreteDistribution;
        d.IncludeTruncatedDistributions = data.ActualData.IncludeTruncatedDistributions;
        d.GetAllDistributions();
        d.ChangeSelectionMethod(data.ActualData.MethodeCalibration);
        distributions[i] = d.CalibratedDistribution;
    }
    StringBuilder sb = new StringBuilder();
    List<double> values = new List<double>();
    for (int i = 0; i < nombreSimulation; i++)
    {
        values.AddRange(distributions[i].Simulate(random, 1000));
        sb.Append($"Simulation {i + 1};{distributions[i].Type};");
        foreach (var param in distributions[i].AllParameters())
        {
            sb.Append($"{param.Name};{param.Value};");
        }
        sb.AppendLine("");
    }
    var variance = values.Variance();
    var theoricalDistribution = calibratedDistribution.Variance();
    File.WriteAllText("result.csv", sb.ToString());
}
else
{
    TestHelper.LancerCalculAutre(true, false, 0, $"resultMMax{4}NormaliseQuantilNormInf", Environment.ProcessorCount, true, true);

}


