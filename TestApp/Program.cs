// See https://aka.ms/new-console-template for more information
using LiveChartsCore.SkiaSharpView.Painting;
using MathNet.Numerics.Random;
using MathNet.Numerics.Statistics;
using OnlineCalibrator.Service;
using OnlineCalibrator.Shared;
using SkiaSharp;
using Stochastique;
using Stochastique.Copule;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using System.Text;

var normal = new NormalDistribution(0, 1);
var logNormal = new NormalDistribution(0, 1000000);
var rand = MersenneTwister.MTRandom.Create(139478);
List<List<double>> ln = new List<List<double>>();
ln.Add(logNormal.Simulate(rand, 1000).ToList());
ln.Add(logNormal.Simulate(rand, 1000).ToList());
List<List<double>> ln2 = new List<List<double>>();
for(int i=0; i<ln.Count; i++)
{
    ln2.Add(new List<double>());
    for (int  j=0; j < ln[i].Count; j++)
    {
        ln2[i].Add(ln[i][j]);
    }
}
CopuleGaussienne cs = new CopuleGaussienne(0.7);
cs.AppliquerCopule(rand, ln2);
List<List<double>> ln3 = new List<List<double>>();
for (int i = 0; i < ln.Count; i++)
{
    ln3.Add(new List<double>());
    for (int j = 0; j < ln[i].Count; j++)
    {
        ln3[i].Add(ln[i][j]);
    }
}
CopuleClayton cs2 = new CopuleClayton(2);
cs2.AppliquerCopule(rand, ln3);
var correl2 = Correlation.Spearman(ln2[0], ln2[1]);
var correl = Correlation.Spearman(ln3[0], ln3[1]);


StringBuilder sb = new StringBuilder();
sb.AppendLine("b1;b2;r;b1 CG0.7;b2 CG0.7;r CG0.7;b1 CJ5;b2 CJ5;r CJ5");
for(int i = 0; i < ln2[0].Count;i++)
{
    sb.AppendLine($"{ln[0][i]};{ln[1][i]};{ln[0][i]+ln[1][i]};{ln2[0][i]};{ln2[1][i]};{ln2[0][i]+ ln2[1][i]};{ln3[0][i]};{ln3[1][i]};{ln3[0][i] + ln3[1][i]}");
}
File.WriteAllText(@".\rstCorrel.csv",sb.ToString());

var truncated = new TrunkatedDistribution(normal, 0.1, 0.75);
var student = new StudentDistribution(3);
var afine = new LoiAfine(student, 2, 1);
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, normal.PDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, truncated.PDF(-5 + (i) / 10.0))).ToArray() },
    new List<Paint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "truncatedpdf", 600, 400, true

    );
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, normal.CDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, truncated.CDF(-5 + (i) / 10.0))).ToArray() },
    new List<Paint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "truncatedCdf", 600, 400, true

    );
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, student.PDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, afine.PDF(-5 + (i) / 10.0))).ToArray() },
    new List<Paint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "afinepdf", 600, 400, true

    );
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, student.CDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, afine.CDF(-5 + (i) / 10.0))).ToArray() },
    new List<Paint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<Paint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
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
var toto2 = elt.ActualData.CurrentDistribution.EELQuantileTest.Alpha;
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
