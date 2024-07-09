// See https://aka.ms/new-console-template for more information
using LiveChartsCore.SkiaSharpView.Painting;
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using SkiaSharp;
using Stochastique;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;

var normal = new NormalDistribution(0, 1);
var truncated = new TrunkatedDistribution(normal, 0.1, 0.75);
var student = new StudentDistribution(3);
var afine = new LoiAfine(student, 2, 1);
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, normal.PDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, truncated.PDF(-5 + (i) / 10.0))).ToArray() },
    new List<Paint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "truncatedpdf", 600, 400, true

    );
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, normal.CDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, truncated.CDF(-5 + (i) / 10.0))).ToArray() },
    new List<Paint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "truncatedCdf", 600, 400, true

    );
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, student.PDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, afine.PDF(-5 + (i) / 10.0))).ToArray() },
    new List<Paint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "afinepdf", 600, 400, true

    );
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, student.CDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, afine.CDF(-5 + (i) / 10.0))).ToArray() },
    new List<Paint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "afineCdf", 600, 400, true

    );
var d = Double.MinValue;

Console.WriteLine(d.ToBeautifulString());
//DonneesImportes.FromMsgPack(File.ReadAllBytes("C:\\Users\\Claude\\Downloads\\log (3).data"));
var elt=FileService.GetDataFromFile(new FileStream("C:\\Users\\Claude\\OneDrive\\Documents\\data.csv", FileMode.Open,FileAccess.Read), "data.csv");


elt.NomData= elt.Donnees.First().Name;
elt.ActualData.IncludeTrunkatedDistributions = false;
//var toto = elt.ActualData.GetAllDistributions();
elt.ActualData.IsDiscreteDistribution = false;

var toto = elt.ActualData.GetAllDistributions();
var toto33 = DonneesImportes.FromMsgPack(elt.ToMsgPack());
elt.ActualData.CalibratedDistribution=toto.First(a=>a.Distribution.Type==Stochastique.Enums.TypeDistribution.Beta).Distribution;
elt.ActualData.GetQQPlot();
elt.ActualData?.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
elt.ActualData.GetQQPlot();
elt.ActualData.AddMonteCarloTest();
var toto2 = elt.ActualData.CurrentDistribution.CarloQuantileTest.Alpha;
elt.ActualData.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Manuelle);
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
