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
using Stochastique.SpecialFunction;
using System.Text;
using TestApp;
WeibullDistribution fd = new WeibullDistribution(2, 4);
GammaDistribution GD = new GammaDistribution(100, 12);
Khi2Distribution k2 = new Khi2Distribution(100);
var mmmmm= fd.Simulate(new Random(13465), 100000).Mean();
var vvvvvv = fd.Simulate(new Random(134665), 100000).Variance();
var SSSSS = fd.Simulate(new Random(134665), 100000).Skewness();
var KKKK = fd.Simulate(new Random(13465), 100000).Kurtosis();
var loiNormale = new NormalDistribution(0, 1);
List<List<double[][]>> rstConfusionMatrix = new List<List<double[][]>>();
foreach (var taille in new List<int> { 20,50,100})
{
    var rzzzd = new Random(13465);
    var sample = loiNormale.Simulate(rzzzd, taille);
    DonneesImportes data = new DonneesImportes();
    data.Donnees = new List<DonneesAAnalyser> { new DonneesAAnalyser { Values = sample, Name="a" }};
    data.NomData = "a";
    data.ActualData.GetAllDistributions();
    data.ActualData.CalibrerMLI();
    rstConfusionMatrix.Add(new List<double[][]> { data.ActualData.ConfusionMatrixML, data.ActualData.ConfusionMatrixMaximumVraissemblance });

}



#region Test Confusion Matrix MLD
var testMatrice = FileService.GetDataFromFile(new FileStream("./test.csv", FileMode.Open, FileAccess.Read), "tes.csv");


testMatrice.NomData = testMatrice.Donnees.First().Name;

testMatrice.ActualData.IsDiscreteDistribution = false;

testMatrice.ActualData.GetAllDistributions();
testMatrice.ActualData.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
testMatrice.ActualData.ComputeMLEConfusionMatrix();
#endregion

#region test copule
var testCopule = FileService.GetDataFromFile(new FileStream("C:\\users\\parent.claude\\Documents\\Classeur1.csv", FileMode.Open, FileAccess.Read), "NormaleCorrelle.csv");


testCopule.NomData = testCopule.Donnees.First().Name;   
testCopule.ActualData.IncludeTrunkatedDistributions = false;
//var toto = elt.ActualData.GetAllDistributions();
testCopule.ActualData.IsDiscreteDistribution = false;
testCopule.NomDataConjointe1 = "A";
testCopule.NomDataConjointe2 = "D";
var resultatTestCopule = testCopule.ActualDonneesPourAnalyseConjointe.GetAllCopula();
testCopule.ActualDonneesPourAnalyseConjointe?.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
testCopule.ActualDonneesPourAnalyseConjointe?.GetCopuleCopulePlot(new Random(123));
//testCopule.DonneesPourAnalyseConjointes = null;
var trololo4864=testCopule.ToMsgPack();
//var totogggg= MessagePack.MessagePackSerializer.Serialize(testCopule.ActualDonneesPourAnalyseConjointe.Copules[2]);
DonneesImportes.FromMsgPack(trololo4864);
#endregion

#region Graphique Puissance test

loiNormale = new NormalDistribution(0, 1);
var loiBeta = new LoiBeta(0.5, 0.5);
var loiBeta2 = new GammaDistribution(5, 5);
int nbSim = 1000;
double[] statNormale = new double[nbSim];
double[] statBeta = new double[nbSim];
double[] statBeta2 = new double[nbSim];
Random randdd= new Random(154365458);
for(int i = 0; i < nbSim; i++)
{
    statNormale[i] = new ShapiroTest(loiNormale.Simulate(randdd, 50)).Statistic;
    statBeta[i] = new ShapiroTest(loiBeta.Simulate(randdd, 50)).Statistic;
    statBeta2[i] = new ShapiroTest(loiBeta2.Simulate(randdd, 50)).Statistic;
}

GenerationGraphique.SaveChartImage(new List<Point[]> { GenerationGraphique.GetDensity(statNormale, 100), GenerationGraphique.GetDensity(statBeta, 100), GenerationGraphique.GetDensity(statBeta2, 100) },

    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue.WithAlpha(0)), new SolidColorPaint(SKColors.DarkRed.WithAlpha(0)), new SolidColorPaint(SKColors.Indigo.WithAlpha(0)) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue) { StrokeThickness = 0 }, new SolidColorPaint(SKColors.DarkRed) { StrokeThickness = 0 }, new SolidColorPaint(SKColors.Indigo) { StrokeThickness = 0 } },
     new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue) { StrokeThickness = 0 }, new SolidColorPaint(SKColors.DarkRed) { StrokeThickness = 0 }, new SolidColorPaint(SKColors.Indigo) { StrokeThickness = 0 } },
    new List<int> { 0,0,0 },

    "distributionTestShapiro",800, 400, true);

#endregion


#region Test Tronqué
var testTronque = FileService.GetDataFromFile(new FileStream("./TestTronque.csv", FileMode.Open, FileAccess.Read), "NormaleCorrelle.csv");


testTronque.NomData = testTronque.Donnees.First().Name;
testTronque.ActualData.IncludeTrunkatedDistributions = true;
//var toto = elt.ActualData.GetAllDistributions();
testTronque.ActualData.IsDiscreteDistribution = false;

testTronque.ActualData.GetAllDistributions();
testTronque.ActualData.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
#endregion

var rst =Debye.gsl_sf_debye_1_e(0.01);
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
GenerationGraphique.SaveChartImage(new List<Point[]> { GenerationGraphique.GetDensity(logNormal.Simulate(rand, 1000).ToArray(), 100) },

    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue.WithAlpha(0)) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue) { StrokeThickness=0 } },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue) { StrokeThickness = 0 } },
    new List<int> { 0 },

    "pfgml");
GenerationGraphique.SaveChartImage(new List<Point[]> { ln3.GetDensity() },

    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue.WithAlpha(0)) },
    new List<SolidColorPaint> { null },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue) { StrokeThickness = 5 } },
    new List<int> { 10 },

    "rangML");
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, normal.PDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, truncated.PDF(-5 + (i) / 10.0))).ToArray() },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "truncatedpdf", 600, 400, true

    );
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, normal.CDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, truncated.CDF(-5 + (i) / 10.0))).ToArray() },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "truncatedCdf", 600, 400, true

    );
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, student.PDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, afine.PDF(-5 + (i) / 10.0))).ToArray() },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "afinepdf", 600, 400, true

    );
GenerationGraphique.SaveChartImage(
    new List<Point[]> { Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, student.CDF(-5.0 + (i) / 10.0))).ToArray(), Enumerable.Repeat(0.0, 100).Select((a, i) => new Point(-5.0 + (i) / 10.0, afine.CDF(-5 + (i) / 10.0))).ToArray() },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue.WithAlpha(50)), new SolidColorPaint(SKColors.Red.WithAlpha(50)) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<SolidColorPaint> { new SolidColorPaint(SKColors.Blue), new SolidColorPaint(SKColors.Red) },
    new List<int> { 0, 0 },
    "afineCdf", 600, 400, true

    );
var d = Double.MinValue;

Console.WriteLine(d.ToBeautifulString());
//DonneesImportes.FromMsgPack(File.ReadAllBytes("C:\\Users\\Claude\\Downloads\\log (3).data"));
var elt=FileService.GetDataFromFile(new FileStream("C:\\users\\claude\\Documents\\NormaleCorrelle.csv", FileMode.Open,FileAccess.Read), "NormaleCorrelle.csv");


elt.NomData= elt.Donnees.First().Name;
elt.ActualData.IncludeTrunkatedDistributions = false;
//var toto = elt.ActualData.GetAllDistributions();
elt.ActualData.IsDiscreteDistribution = false;
elt.NomDataConjointe1 = elt.Donnees.First().Name;
elt.NomDataConjointe2 = elt.Donnees.Last().Name;
var ttttt=elt.ActualDonneesPourAnalyseConjointe.GetAllCopula();
var toto = elt.ActualData.GetAllDistributions();
elt.ActualData.CalibratedDistribution = elt.ActualData.Distributions.First(a => a.Distribution is GammaDistribution).Distribution;
elt.ActualData.GetQQPlot();

elt.ActualData.Distributions.First().ToMsgPack().FromMsgPack<DistributionWithDatas>();

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
