using MathNet.Numerics.Statistics;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
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
        public List<Distribution> DistributionsCoutInf5M { get; set; }
        public List<Distribution> DistributionsCoutSup5M { get; set; }
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
            double[][] ratioTriangleNombre = new double[TriangleNombre.Length - 1][];
            for (int i = 0; i < TriangleNombre.Length - 1; i++)
            {
                ratioTriangleNombre[i] = new double[TriangleNombre.Length - 1];
                for (int j = 0; j < TriangleNombre.Length - 1 - i; j++)
                {
                    ratioTriangleNombre[i][j] = 1.0 * TriangleNombre[i][j + 1] / TriangleNombre[i][j];
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
                        temp *= ratioTriangleNombre[(int)(r.NextDouble() *( ratioTriangleNombre.Length-k))][k];
                    }
                    temp = temp / Exposition[j] * Exposition.Last();
                    NombresSimules[i][j] = (int)Math.Round(temp);
                }
            }


            int survmin = Sinistres.Min(a => a.Surv);
            int survmax = Sinistres.Max(a => a.Surv);
            List<double>[] ratiosCout = new List<double>[survmax - survmin];
            for (int i = 0; i < survmax - survmin; i++)
            {
                ratiosCout[i] = new List<double>();
            }
            var sinistreparID = Sinistres.GroupBy(a => a.ID);
            foreach (var sinistre in sinistreparID)
            {
                var temp = sinistre.OrderBy(a => a.Devpt).ToList();
                for (int i = 0; i < temp.Count - 1; i++)
                {
                    if (temp[i].Cout>= 500000 && temp[i + 1].Cout != 0)
                    {
                        ratiosCout[temp[i].Surv - survmin].Add(temp[i + 1].Cout / temp[i].Cout);
                    }
                }
            }
            for(int i=0;i< ratiosCout.Length;i++)
            {
                var mean = ratiosCout[i].Mean();
                var variance = ratiosCout[i].StandardDeviation();
                ratiosCout[i] = ratiosCout[i].Where(a => a > mean - 5 * variance && a < mean + 5 * variance).ToList();
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
                        cout *= ratiosCout[k][(int)(r.NextDouble() * ratiosCout[k].Count)];
                    }
                    cout += sinistre.RetraitementInflation;
                    CoutSimules[i][j] = cout;
                    j++;
                }
            }

        }
        public void CalibrateLoiNombre()
        {
            DistributionsNombre = new List<Distribution>();
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
            }
        }

        public void Export()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < CoutSimules.Length; i++)
            {
                foreach(var v in CoutSimules[i].Select((a,i)=>(a,SinistresMiseEnForme[i].Survenance)).GroupBy(a=>a.Survenance).OrderBy(a=>a.Key))
                {
                    stringBuilder.Append(v.Sum(a=>a.a)/v.Count() + ";");
                }
                stringBuilder.Append(Environment.NewLine);
            }
            File.WriteAllText("./coutSimules.csv", stringBuilder.ToString());
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

            for (int i = 0; i < DistributionsNombre.Count; i++)
            {

                stringBuilder.Append(DistributionsNombre[i].Type + ";");
                foreach(var v in DistributionsNombre[i].AllParameters())
                {
                    stringBuilder.Append(v.Name + ";" + v.Value+";");
                }
                stringBuilder.Append(Environment.NewLine);
            }
            File.WriteAllText("./loiNombre.csv", stringBuilder.ToString());
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
