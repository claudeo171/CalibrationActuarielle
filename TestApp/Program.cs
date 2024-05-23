// See https://aka.ms/new-console-template for more information
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using Stochastique;

var d = Double.MinValue;

Console.WriteLine(d.ToBeautifulString());
//DonneesImportes.FromMsgPack(File.ReadAllBytes("C:\\Users\\Claude\\Downloads\\log (3).data"));
var elt=FileService.GetDataFromFile(new FileStream("C:\\Users\\Claude\\Documents\\abcd.csv", FileMode.Open,FileAccess.Read), "abcd.csv");


elt.NomData= elt.Donnees.First().Name;
elt.ActualData.IncludeTrunkatedDistributions = false;
//var toto = elt.ActualData.GetAllDistributions();
elt.ActualData.IsDiscreteDistribution = false;

var toto = elt.ActualData.GetAllDistributions();
elt.ActualData?.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
elt.ActualData.GetQQPlot();
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
