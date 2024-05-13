// See https://aka.ms/new-console-template for more information
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using Stochastique;

var d = Double.MinValue;

Console.WriteLine(d.ToBeautifulString());
//DonneesImportes.FromMsgPack(File.ReadAllBytes("C:\\Users\\Claude\\Downloads\\log (3).data"));
var elt=FileService.GetDataFromFile(new FileStream("C:\\Users\\Claude\\Documents\\NormaleCorrelle.csv", FileMode.Open), "NormaleCorrelle.csv");

elt.NomDataConjointe1= "A";
elt.NomDataConjointe2= "B";
elt.ActualDonneesPourAnalyseConjointe.GetAllCopula();
elt.ActualDonneesPourAnalyseConjointe.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
elt.ActualDonneesPourAnalyseConjointe.GetCopuleCopulePlot(new Random());
elt.ActualDonneesPourAnalyseConjointe.CalibratedCopule.SimulerCopule(new Random(), 111);
elt.NomData= "A";
elt.ActualData.IncludeTrunkatedDistributions = true;
//var toto = elt.ActualData.GetAllDistributions();
elt.ActualData.IsDiscreteDistribution = false;
var toto = elt.ActualData.GetAllDistributions();
foreach(var v in toto)
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
