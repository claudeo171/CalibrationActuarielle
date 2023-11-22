// See https://aka.ms/new-console-template for more information
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;

Console.WriteLine("Hello, World!");
//DonneesImportes.FromMsgPack(File.ReadAllBytes("C:\\Users\\Claude\\Downloads\\log (3).data"));
var elt=FileService.GetDataFromFile(new FileStream("C:\\Users\\Claude\\Documents\\data.csv", FileMode.Open), "data.csv");
elt.NomData= "A";
elt.ActualData.IncludeTrunkatedDistributions = true;
//var toto = elt.ActualData.GetAllDistributions();
elt.ActualData.IsDiscreteDistribution = false;
var toto = elt.ActualData.GetAllDistributions();
elt.ToMsgPack();

elt.ToMsgPack();
