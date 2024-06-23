// See https://aka.ms/new-console-template for more information
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using System.Text;

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
    foreach(var param in distributions[i].AllParameters())
    {
        sb.Append($"{param.Name};{param.Value};");
    }
    sb.AppendLine("");
}
File.WriteAllText("result.csv", sb.ToString());


