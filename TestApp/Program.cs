// See https://aka.ms/new-console-template for more information
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using Stochastique;

var d = Double.MinValue;

Console.WriteLine(d.ToBeautifulString());
//DonneesImportes.FromMsgPack(File.ReadAllBytes("C:\\Users\\Claude\\Downloads\\log (3).data"));
var elt=FileService.GetDataFromFile(new FileStream("C:\\Users\\Claude\\Documents\\data.csv", FileMode.Open,FileAccess.Read), "abcd.csv");


elt.NomData= elt.Donnees.First().Name;
elt.ActualData.IncludeTrunkatedDistributions = false;
//var toto = elt.ActualData.GetAllDistributions();
elt.ActualData.IsDiscreteDistribution = false;

var toto = elt.ActualData.GetAllDistributions();
elt.ActualData.CalibratedDistribution=toto.First(a=>a.Distribution.Type==Stochastique.Enums.TypeDistribution.Beta).Distribution;
elt.ActualData.GetQQPlot();
elt.ActualData?.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
elt.ActualData.GetQQPlot();
elt.ActualData.AddMonteCarloTest();
var toto2 = elt.ActualData.CurrentDistribution.CarloQuantileTest.Alpha;
elt.ActualData.GetQuantilePlot();
foreach (var v in toto)
{
    v.Distribution.ExpextedValue();
    v.Distribution.Variance();
    v.Distribution.Kurtosis();
    v.Distribution.Skewness();
}
elt.ActualData?.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
DonneesImportes.FromMsgPack(elt.ToMsgPack());
elt.ActualData.CalibrerMLI();
FileService fs = new FileService();
File.WriteAllBytes("test.docx", fs.ExportFileDocx(elt));
elt.ToMsgPack();

elt.ToMsgPack();
