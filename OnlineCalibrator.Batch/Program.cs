// See https://aka.ms/new-console-template for more information
using Accord.Statistics;
using MathNet.Numerics.Statistics;
using OnlineCalibrator.Batch;
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using System.Text;

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
        d.IsDiscreteDistribution = data.ActualData.IsDiscreteDistribution;
        d.IncludeTrunkatedDistributions = data.ActualData.IncludeTrunkatedDistributions;
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
    for (int i = 5; i < 20; i++)
    {
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(true, false, 0, $"resultMMax{i}NormaliseQuantilNormInf", Environment.ProcessorCount, true, i, true);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(false, false, 0, $"resultMMax{i}NormaliseQuantilNorm1", Environment.ProcessorCount, true, i, true);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(true, false, 0, $"resultMMax{i}NormaliseVarianceNormInf", Environment.ProcessorCount, true, i, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(false, false, 0, $"resultMMax{i}NormaliseVarianceNorm1", Environment.ProcessorCount, true, i, false);
        /*
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(true, true, 0, "resultratioInf0", Environment.ProcessorCount,false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(false, true, 0, "resultRatioInf0", EnvironmentN.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(true, true, 0, "resultratioInf0", Environment.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(false, true, 0, "resultRatioInf0", Environment.ProcessorCount, false);
        //OnlineCalibrator.Batch.TestHelper.LancerCalcul(true,false,0,"resultEcartInf0", Environment.ProcessorCount, false);
        //OnlineCalibrator.Batch.TestHelper.LancerCalcul(false, false, 0, "resultEcartUn0", Environment.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(true, false, 1, "resultEcartInf1", Environment.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(false, false, 1, "resultEcartUn1", Environment.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(true, false, 0.5, "resultEcartInf05", Environment.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(false, false, 0.5, "resultEcartUn05", Environment.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(true, false, 0.25, "resultEcartInf025", Environment.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(false, false, 0.25, "resultEcartUn025", Environment.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(true, false, 0.75, "resultEcartInf075", Environment.ProcessorCount, false);
        OnlineCalibrator.Batch.TestHelper.LancerCalcul(false, false, 0.75, "resultEcartUn075", Environment.ProcessorCount, false);
        */
    }
    var normal = new NormalDistribution(0, 1);
    var laplace = new LaplaceDistribution(0, 1);
    var mlist = laplace.GetMomentList(10);
    var m2list = normal.GetMomentList(10);


}


