using MathNet.Numerics.Statistics;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using Stochastique.Distributions.Discrete;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Batch
{
    public class ErreurModeleRetraitement
    {
        public int[][] TriangleNombre { get; set; }
        public double[] Exposition { get; set; }
        public List<Sinistre> Sinistres { get; set; }
        public List<SinistreTouteSurvenance> SinistresMiseEnForme { get; set; }
        public double[][] NombresSimules { get; set; }
        public double[][] CoutSimules { get; set; }
        public List<Distribution> DistributionsNombre { get; set; }
        public List<List<(Distribution dist, double proba)>> DistributionsNombreErreurCalibration { get; set; }
        public Distribution[] DistributionsCoutInf5M { get; set; }
        public List<(Distribution dist, double proba)>[] DistributionsCoutInf5MErreurCalibration { get; set; }
        public Distribution[] DistributionsCoutSup5M { get; set; }
        public List<(Distribution dist, double proba)>[] DistributionsCoutSup5MErreurCalibration { get; set; }
        public Random r { get; set; }

        public void Import()
        {
            r = new Random(45564);
            var striangle = File.ReadAllLines("./TriangleNombreSinistre.csv");
            TriangleNombre = new int[striangle.Length][];
            for (int i = 0; i < striangle.Length; i++)
            {
                TriangleNombre[i] = striangle[i].Split(';').Select(a => a == "" ? 0 : Convert.ToInt32(a)).ToArray();
            }
            var sin = File.ReadAllLines("./ValeurCoutSto.csv");
            Sinistres = new List<Sinistre>();
            for (int i = 0; i < sin.Length; i++)
            {
                var temp = sin[i].Split(";");
                Sinistres.Add(new Sinistre
                {
                    ID = temp[0],
                    Devpt = Convert.ToInt32(temp[2]),
                    Surv = Convert.ToInt32(temp[1]),
                    Cout = Convert.ToDouble(temp[3]),
                    RetraitementInflation = Convert.ToDouble(temp[4]),
                });
            }
            Exposition = File.ReadAllLines("./Retraitements.csv").Select(a => Convert.ToDouble(a)).ToArray();

        }

        public void Calculer(int nbSim)
        {
            int historique = 5;
            double[][] ratioTriangleNombre = new double[TriangleNombre.Length - 1][];
            double[][] ratioTriangleWeight = new double[TriangleNombre.Length - 1][];
            for (int i = 0; i < TriangleNombre.Length - 1; i++)
            {
                ratioTriangleWeight[i] = new double[5];
            }
            for (int i = 0; i < TriangleNombre.Length - 1; i++)
            {
                ratioTriangleNombre[i] = new double[TriangleNombre.Length - 1];
                for (int j = 0; j < TriangleNombre.Length - 1 - i; j++)
                {
                    ratioTriangleNombre[i][j] = 1.0 * TriangleNombre[i][j + 1] / TriangleNombre[i][j];
                    if (Math.Min(0, j - TriangleNombre.Length + historique + 1) + i >= 0)
                    {
                        ratioTriangleWeight[j][Math.Min(0, j - TriangleNombre.Length + historique + 1) + i] = TriangleNombre[i][j];
                    }
                }

            }

            for (int i = 0; i < TriangleNombre.Length - 1; i++)
            {
                for (int j = 1; j < historique; j++)
                {
                    ratioTriangleWeight[i][j] = ratioTriangleWeight[i][j - 1] + ratioTriangleWeight[i][j];
                }
            }

            for (int i = 0; i < TriangleNombre.Length - 1; i++)
            {
                for (int j = 0; j < historique; j++)
                {
                    ratioTriangleWeight[i][j] /= ratioTriangleWeight[i][historique - 1];
                }
            }
            NombresSimules = new double[nbSim][];
            for (int i = 0; i < nbSim; i++)
            {
                NombresSimules[i] = new double[TriangleNombre.Length];
                for (int j = 0; j < TriangleNombre.Length; j++)
                {
                    double temp = TriangleNombre[j][TriangleNombre.Length - j - 1];
                    for (int k = TriangleNombre.Length - 1 - j; k < TriangleNombre.Length - 1; k++)
                    {
                        var alea = r.NextDouble();
                        int indice = ratioTriangleWeight[k].ToList().IndexOf(ratioTriangleWeight[k].First(a => a > alea));
                        temp *= ratioTriangleNombre[indice + Math.Max(0, ratioTriangleNombre.Length - k - 5)][k];
                    }
                    temp = temp / Exposition[j] * Exposition.Last();
                    NombresSimules[i][j] = (int)Math.Round(temp);
                }
            }

            List<(double min, double max)> seuils = new List<(double min, double max)>{
                (0,1000000),
                (1000000,2000000),
                (2000000,4000000),
                (4000000,8000000),
                (8000000,double.MaxValue)
            };
            int survmin = Sinistres.Min(a => a.Surv);
            int survmax = Sinistres.Max(a => a.Surv);
            Dictionary<(double min, double max), List<(double ratio, double proba)>>[] ratioCoutParTranche = new Dictionary<(double min, double max), List<(double ratio, double proba)>>[survmax - survmin];
            List<(double ratio, double proba)>[] ratiosCout = new List<(double ratio, double proba)>[survmax - survmin];
            List<(double valeur, double ratio)> ratiorrrr = new List<(double valeur, double ratio)>();
            for (int i = 0; i < survmax - survmin; i++)
            {
                ratiosCout[i] = new List<(double ratio, double proba)>();
                ratioCoutParTranche[i] = new Dictionary<(double min, double max), List<(double ratio, double proba)>>();
            }
            var sinistreparID = Sinistres.GroupBy(a => a.ID);
            foreach (var sinistre in sinistreparID)
            {
                var temp = sinistre.OrderBy(a => a.Devpt).ToList();
                for (int i = 0; i < temp.Count - 1; i++)
                {
                    if (temp[i].Cout >= 500000 && temp[i + 1].Cout != 0)
                    {
                        ratiosCout[temp[i].Surv - survmin].Add((temp[i + 1].Cout / temp[i].Cout, temp[i].Cout));
                    }
                }
            }
            for (int i = 0; i < ratiosCout.Length; i++)
            {
                foreach (var seuil in seuils)
                {
                    var list = ratiosCout[i].Where(a => a.proba >= seuil.min && a.proba <= seuil.max).ToList();

                    if (list.Any())
                    {
                        var total = list.Sum(a => a.proba);
                        list[0] = (list[0].ratio, list[0].proba / total);
                        for (int j = 1; j < list.Count; j++)
                        {
                            list[j] = (list[j].ratio, list[j - 1].proba + list[j].proba / total);
                        }
                    }
                    else
                    {
                        list.Add((1, 1));
                    }
                    ratioCoutParTranche[i].Add(seuil, list);
                }

            }
            SinistresMiseEnForme = sinistreparID.Select(a => new SinistreTouteSurvenance
            {
                Cout = a.First(a => a.Devpt + a.Surv == survmax).Cout,
                RetraitementInflation = a.Sum(a => a.RetraitementInflation),
                Sinistres = a,
                Survenance = a.First().Surv
            }).ToList();
            CoutSimules = new double[nbSim][];
            for (int i = 0; i < nbSim; i++)
            {
                CoutSimules[i] = new double[sinistreparID.Count()];
                int j = 0;
                foreach (var sinistre in SinistresMiseEnForme)
                {
                    var cout = sinistre.Cout;
                    for (int k = survmax - sinistre.Survenance; k < survmax - survmin; k++)
                    {
                        var seuil = seuils.First(a => a.min <= cout && a.max >= cout);
                        var alea = r.NextDouble();
                        int indice = ratioCoutParTranche[k][seuil].IndexOf(ratioCoutParTranche[k][seuil].First(a => a.proba > alea));
                        cout *= ratioCoutParTranche[k][seuil][indice].ratio;
                    }
                    cout += sinistre.RetraitementInflation;
                    CoutSimules[i][j] = cout;
                    j++;
                }
            }
            var exartMoyenne = SinistresMiseEnForme.Count*1981000.0 - SinistresMiseEnForme.Sum(a => a.Cout);
            var ecartGlobal = CoutSimules.Sum(a => a.Sum())/ CoutSimules.Length - SinistresMiseEnForme.Sum(a => a.Cout) ;
            for(int i=0;i<CoutSimules.Length;i++)
            {
                for(int j = 0; j < CoutSimules[i].Length;j++)
                {
                    CoutSimules[i][j] = SinistresMiseEnForme[j].Cout + (CoutSimules[i][j] - SinistresMiseEnForme[j].Cout) * exartMoyenne / ecartGlobal;
                }
            }
            var moyenne = CoutSimules.Sum(a => a.Sum(b => b >= 500000 ? b:0)) / CoutSimules.Sum(a => a.Count(b=> b >= 500000));
            var max = CoutSimules.Max(a => a.Max());

        }
        public void CalibrateLoiNombre()
        {
            DistributionsNombre = new List<Distribution>();
            DistributionsNombreErreurCalibration = new List<List<(Distribution dist, double proba)>>();
            for (int i = 0; i < NombresSimules.Length; i++)
            {
                DonneesImportes donnees = new DonneesImportes
                {
                    Donnees = new List<DonneesAAnalyser>
                    {
                        new DonneesAAnalyser
                        {
                            Name="a",
                            Values=NombresSimules[i]
                        }
                    }
                };
                donnees.NomData = "a";
                donnees.ActualData.IsDiscreteDistribution = true;
                donnees.ActualData.GetAllDistributions();
                donnees.ActualData.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
                DistributionsNombre.Add(donnees.ActualData.CalibratedDistribution);
                DistributionsNombreErreurCalibration.Add(donnees.ActualData.GetDistributionAvecErreurDeModele());
            }
        }
        public void CalibrateLoiCout()
        {
            DistributionsCoutInf5M = new Distribution[NombresSimules.Length];
            DistributionsCoutSup5M = new Distribution[NombresSimules.Length];
            DistributionsCoutInf5MErreurCalibration = new List<(Distribution dist, double proba)>[NombresSimules.Length];
            DistributionsCoutSup5MErreurCalibration = new List<(Distribution dist, double proba)>[NombresSimules.Length];
            Parallel.For(0, NombresSimules.Length, new ParallelOptions {  MaxDegreeOfParallelism = Environment.ProcessorCount}, i =>
            {
                DonneesImportes donneesInf = new DonneesImportes
                {
                    Donnees = new List<DonneesAAnalyser>
                    {
                        new DonneesAAnalyser
                        {
                            Name="a",
                            Values= CoutSimules[i].Where(a=>a<5000000 && a>=500000).ToArray()
                        }
                    }
                };
                donneesInf.NomData = "a";
                donneesInf.ActualData.IncludeTruncatedDistributions = true;
                donneesInf.ActualData.GetAllDistributions();
                donneesInf.ActualData.ValeurMinTrukated = 500000;
                donneesInf.ActualData.ValeurMaxTrukated = 5000000;
                donneesInf.ActualData.MajCalibrationTronque();
                donneesInf.ActualData.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
                DistributionsCoutInf5M[i] = donneesInf.ActualData.CalibratedDistribution;
                DistributionsCoutInf5MErreurCalibration[i] = donneesInf.ActualData.GetDistributionAvecErreurDeModele();
                DonneesImportes donneesSup = new DonneesImportes
                {
                    Donnees = new List<DonneesAAnalyser>
                    {
                        new DonneesAAnalyser
                        {
                            Name="a",
                            Values=CoutSimules[i].Where(a=>a>5000000).ToArray()
                        }
                    }
                };
                donneesSup.NomData = "a";
                donneesSup.ActualData.IncludeTruncatedDistributions = true;
                donneesSup.ActualData.GetAllDistributions();
                donneesSup.ActualData.ValeurMinTrukated = 5000000;
                donneesSup.ActualData.ValeurMaxTrukated = 5e15;
                donneesSup.ActualData.MajCalibrationTronque();
                donneesSup.ActualData.ChangeSelectionMethod(Stochastique.Enums.MethodeCalibrationRetenue.Vraisemblance);
                DistributionsCoutSup5M[i] = donneesSup.ActualData.CalibratedDistribution;
                DistributionsCoutSup5MErreurCalibration[i] = donneesSup.ActualData.GetDistributionAvecErreurDeModele();
            });
        }

        public void Export()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < CoutSimules.Length; i++)
            {
                foreach (var v in CoutSimules[i].Select((a, i) => (a, SinistresMiseEnForme[i].Survenance)).GroupBy(a => a.Survenance).OrderBy(a => a.Key))
                {
                    stringBuilder.Append(v.Sum(a => a.a) / v.Count() + ";");
                }
                stringBuilder.Append(Environment.NewLine);
            }
            File.WriteAllText("./coutSimules.csv", stringBuilder.ToString());
            stringBuilder = new StringBuilder();
            for (int i = 0; i < CoutSimules.Length; i++)
            {
                foreach (var v in CoutSimules[i])
                {
                    stringBuilder.Append(v + ";");
                }
                stringBuilder.Append(Environment.NewLine);
            }
            File.WriteAllText("./coutSimulesDetail.csv", stringBuilder.ToString());
            stringBuilder = new StringBuilder();

            for (int i = 0; i < NombresSimules.Length; i++)
            {
                for (int j = 0; j < NombresSimules[0].Length; j++)
                {
                    stringBuilder.Append(NombresSimules[i][j] + ";");
                }
                stringBuilder.Append(Environment.NewLine);
            }
            File.WriteAllText("./nombreSimule.csv", stringBuilder.ToString());
            stringBuilder = new StringBuilder();
            EcrireExportLoi(stringBuilder, DistributionsNombre);
            File.WriteAllText("./loiNombre.csv", stringBuilder.ToString());
            stringBuilder = new StringBuilder();
            EcrireExportLoi(stringBuilder, DistributionsCoutInf5M);
            File.WriteAllText("./loiCoutInf.csv", stringBuilder.ToString());
            stringBuilder = new StringBuilder();
            EcrireExportLoi(stringBuilder, DistributionsCoutSup5M);
            File.WriteAllText("./loiCoutSup.csv", stringBuilder.ToString());
            stringBuilder = new StringBuilder();
            EcrireExportLoiCout(stringBuilder, DistributionsCoutInf5M, DistributionsCoutSup5M);
            File.WriteAllText("./loiCoutMisEnForme.csv", stringBuilder.ToString(), Encoding.UTF8);
            stringBuilder = new StringBuilder();
            EcrireExportLoiNombre(stringBuilder, DistributionsNombreErreurCalibration, "Nombre");
            EcrireExportLoiCout(stringBuilder, DistributionsCoutInf5MErreurCalibration, DistributionsCoutSup5MErreurCalibration, "Cout");
            File.WriteAllText("./loiAvecErreur.csv", stringBuilder.ToString(), Encoding.UTF8);

        }
        public void EcrireExportLoi(StringBuilder sb, IEnumerable<Distribution> valeurs)
        {
            var val=valeurs.ToList();
            for (int i = 0; i < val.Count; i++)
            {
                var dist = val[i];
                if (dist is TruncatedDistribution trunk)
                {
                    sb.Append($"Loi Tronquée;{trunk.QuantileDown};{trunk.QuantileUp};{trunk.ValeurMin};{trunk.ValeurMax};");
                    dist = trunk.BaseDistribution;
                }
                sb.Append($"{dist.Type};");
                foreach (var param in dist.AllParameters())
                {
                    sb.Append(param.Value + ";");
                }
                sb.AppendLine();
            }
        }
        public void EcrireExportLoiCout(StringBuilder sb, IEnumerable<Distribution> valeursinf, IEnumerable<Distribution> valeurssup)
        {
            var valinf = valeursinf.ToList();
            var valsup = valeurssup.ToList();
            for (int i = 0; i < valinf.Count; i++)
            {
                var distinf = valinf[i];
                var distsup = valsup[i];
                sb.Append($"Loi Composée;{CoutSimules[i].Count(a => a <= 5000000 && a>=500000)*1.0 / CoutSimules[i].Count(a=>a>500000)};");
                if (distinf is TruncatedDistribution trunk)
                {
                    sb.Append($"Loi Tronquée;{trunk.QuantileDown};{trunk.QuantileUp};{trunk.ValeurMin};{trunk.ValeurMax};");
                    distinf = trunk.BaseDistribution;
                }
                sb.Append($"{distinf.Type};");
                foreach (var param in distinf.AllParameters())
                {
                    sb.Append(param.Value + ";");
                }
                if (distsup is TruncatedDistribution trunksup)
                {
                    sb.Append($"Loi Tronquée;{trunksup.QuantileDown};{trunksup.QuantileUp};{trunksup.ValeurMin};{trunksup.ValeurMax};");
                    distsup = trunksup.BaseDistribution;
                }
                sb.Append($"{distsup.Type};");
                foreach (var param in distsup.AllParameters())
                {
                    sb.Append(param.Value + ";");
                }
                sb.AppendLine();
            }
        }
        public void EcrireExportLoiNombre(StringBuilder sb, List<List<(Distribution dist, double proba)>> valeurs, string nom)
        {
            for (int i = 0; i < valeurs.Count; i++)
            {
                sb.Append($"{nom};{i+1};");
                for (int j = 0; j < valeurs[i].Count; j++)
                {
                    
                    if (j != valeurs[i].Count - 1)
                    {
                        if (j == 0)
                        {
                            sb.Append($"Loi Composée;{valeurs[i][j].proba};");
                        }
                        else
                        {
                            var proba = (valeurs[i][j].proba - valeurs[i][j - 1].proba) / (1 - valeurs[i][j-1].proba);
                            sb.Append($"Loi Composée;{proba};");
                        }
                    }
                    var dist = valeurs[i][j].dist;
                    if (dist is TruncatedDistribution trunk)
                    {
                        sb.Append($"Loi Tronquée;{trunk.QuantileDown};{trunk.QuantileUp};{trunk.ValeurMin};{trunk.ValeurMax};");
                        dist = trunk.BaseDistribution;
                    }
                    sb.Append($"{dist.Type};");
                    var parameters = dist.AllParameters();
                    if (dist is CauchyDistribution || dist is ParetoDistribution || dist is GammaDistribution || dist is UniformDistribution || dist is BinomialDistribution || dist is NegativeBinomialDistribution || dist is PascalDistribution)
                    {
                        foreach (var param in dist.AllParameters().Reverse())
                        {
                            sb.Append(param.Value + ";");
                        }
                    }
                    else
                    {
                        foreach (var param in dist.AllParameters())
                        {
                            sb.Append(param.Value + ";");
                        }
                    }
                }
                sb.AppendLine();
            }
        }
        public void EcrireExportLoiCout(StringBuilder sb, List<(Distribution dist, double proba)>[] valeursInf, List<(Distribution dist, double proba)>[] valeursSup, string nom)
        {
            
            for (int i = 0; i < valeursInf.Length; i++)
            {
                double probaRestante = 1;
                var proportion = CoutSimules[i].Count(a => a <= 5000000 && a >= 500000) * 1.0 / CoutSimules[i].Count(a => a > 500000);
                sb.Append($"{nom};{i+1};");
                for (int j = 0; j < valeursInf[i].Count; j++)
                {
                    

                    if (j == 0)
                    {
                        sb.Append($"Loi Composée;{valeursInf[i][j].proba * proportion};");
                        probaRestante -= valeursInf[i][j].proba * proportion;
                    }
                    else
                    {
                        var proba = (valeursInf[i][j].proba - valeursInf[i][j - 1].proba) * proportion / probaRestante;
                        probaRestante -= (valeursInf[i][j].proba - valeursInf[i][j - 1].proba) * proportion;
                        sb.Append($"Loi Composée;{proba};");
                    }
                    var dist = valeursInf[i][j].dist;
                    if (dist is TruncatedDistribution trunk)
                    {
                        sb.Append($"Loi Tronquée;{trunk.QuantileDown};{trunk.QuantileUp};{trunk.ValeurMin};{trunk.ValeurMax};");
                        dist = trunk.BaseDistribution;
                    }
                    else
                    {
                        sb.Append($"Loi Tronquée;{dist.CDF(500000)};{dist.CDF(5000000)};{500000};{5000000};");
                    }
                    sb.Append($"{dist.Type};");
                    var parameters = dist.AllParameters();
                    if (dist is CauchyDistribution || dist is ParetoDistribution || dist is GammaDistribution || dist is UniformDistribution || dist is BinomialDistribution || dist is NegativeBinomialDistribution || dist is PascalDistribution)
                    {
                        foreach (var param in dist.AllParameters().Reverse())
                        {
                            sb.Append(param.Value + ";");
                        }
                    }
                    else
                    {
                        foreach (var param in dist.AllParameters())
                        {
                            sb.Append(param.Value + ";");
                        }
                    }
                }
                for (int j = 0; j < valeursSup[i].Count; j++)
                {

                    if (j != valeursSup[i].Count - 1)
                    {
                        if (j == 0)
                        {
                            sb.Append($"Loi Composée;{valeursSup[i][j].proba *(1-proportion)/ probaRestante };");
                            probaRestante -= valeursSup[i][j].proba * (1 - proportion);
                        }
                        else
                        {
                            var proba = (valeursSup[i][j].proba - valeursSup[i][j - 1].proba) * (1 - proportion) / probaRestante;
                            probaRestante -= (valeursSup[i][j].proba - valeursSup[i][j - 1].proba) * (1 - proportion);
                            sb.Append($"Loi Composée;{proba};");
                        }
                    }
                    var dist = valeursSup[i][j].dist;
                    if (dist is TruncatedDistribution trunk)
                    {
                        sb.Append($"Loi Tronquée;{trunk.QuantileDown};{trunk.QuantileUp};{trunk.ValeurMin};{trunk.ValeurMax};");
                        dist = trunk.BaseDistribution;
                    }
                    sb.Append($"{dist.Type};");
                    if (dist is CauchyDistribution || dist is ParetoDistribution || this is GammaDistribution || this is UniformDistribution || this is BinomialDistribution || this is NegativeBinomialDistribution || this is PascalDistribution)
                    {
                        foreach (var param in dist.AllParameters().Reverse())
                        {
                            sb.Append(param.Value + ";");
                        }
                    }
                    else
                    {
                        foreach (var param in dist.AllParameters())
                        {
                            sb.Append(param.Value + ";");
                        }
                    }
                }
                sb.AppendLine();
            }
        }
        public class SinistreTouteSurvenance
        {
            public IGrouping<string, Sinistre> Sinistres { get; set; }
            public double Cout { get; set; }
            public double RetraitementInflation { get; set; }
            public int Survenance { get; set; }
        }
        public class Sinistre
        {
            public string ID { get; set; }
            public int Surv { get; set; }
            public int Devpt { get; set; }
            public double Cout { get; set; }
            public double RetraitementInflation { get; set; }
        }
    }
}
