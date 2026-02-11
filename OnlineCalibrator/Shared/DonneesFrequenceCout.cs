using MathNet.Numerics.Statistics;
using Stochastique;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class DonneesFrequenceCout
    {
        [MemoryPack.MemoryPackOrder(0)]
        public DonneesAAnalyser Nombre { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public DonneesAAnalyser CoutMoyenParClassification { get; set; }
        [MemoryPack.MemoryPackOrder(2)]
        public DonneesAAnalyser Cout { get; set; }
        [MemoryPack.MemoryPackOrder(3)]
        public DonneesAAnalyser Classification { get; set; }
        [MemoryPack.MemoryPackOrder(4)]
        public DonneesPourAnalyseConjointe DonnneesCopules { get; set; }
        [MemoryPack.MemoryPackOrder(5)]
        public bool UtiliserCopule { get;set; }

        public DonneesFrequenceCout(DonneesAAnalyser cout, DonneesAAnalyser classification)
        {
            Cout = cout;
            Classification = classification;
            Nombre = new DonneesAAnalyser() { IsDiscreteDistribution=true, Name=$"Nombre {cout.Name} par {Classification.Name}", Values = cout.Values.Select((x, indice) => (x, indice)).GroupBy((a) => Classification.Values[a.indice]).Select(a => (double)a.Count()).ToArray() };
            CoutMoyenParClassification = new DonneesAAnalyser() { Name = $"Nombre {cout.Name} par {Classification.Name}", Values = cout.Values.Select((x, indice) => (x, indice)).GroupBy((a) => Classification.Values[a.indice]).Select(a => a.Sum(b=>b.x)/ a.Count()).ToArray() };
            DonnneesCopules = new DonneesPourAnalyseConjointe() { DonneesAAnalyser1 = Nombre, DonneesAAnalyser2 = CoutMoyenParClassification };
        }

        [MemoryPack.MemoryPackIgnore]
        public Point[]? PointsKDE => GenerationGraphique.GetDensity(Sample(1000).Select(a=>a.Sum()).ToArray(), 100);
        [MemoryPack.MemoryPackIgnore]
        public Point[]? PointsCDF => GenerationGraphique.GetCDF(Sample(1000).Select(a => a.Sum()).ToArray());

        public double[][] Sample(int nbSim)
        {
            var random = new Random();
            double[][] resultat = new double[nbSim][];

            var nombreSinistres = Nombre.CalibratedDistribution.Simulate(random, nbSim);
            if(!UtiliserCopule)
            { 
                for (int i = 0; i < nbSim; i++)
                {
                    resultat[i] = new double[(int)nombreSinistres[i]];
                    for (int j = 0; j < nombreSinistres[i]; j++)
                    {
                        resultat[i][j] = Cout.CalibratedDistribution.Simulate(random);
                    }
                }
            }
            else
            {
                double maxSininstre = nombreSinistres.Max();
                double[][] resultatTemp = new double[nbSim][];
                for (int i = 0; i < nbSim; i++)
                {
                    resultatTemp[i] = new double[(int)maxSininstre];
                    for (int j = 0; j < maxSininstre; j++)
                    {
                        resultatTemp[i][j] = Cout.CalibratedDistribution.Simulate(random);
                    }
                }
                var tabMean = resultatTemp.Select(a => a.Mean()).ToArray();
                DonnneesCopules.CalibratedCopule.RendreDependant(random,nombreSinistres, tabMean);

                resultat = new double[nbSim][];
                for (int i = 0; i < nbSim; i++)
                {
                    resultat[i] = SampleFromList(random, resultatTemp.First(a=>a.Mean()== tabMean[i]), nombreSinistres[i]);
                }
            }
            return resultat;
        }

        /// <summary>
        /// Ressort les nbelt les plus proches de la moyenne du tableau
        /// </summary>
        /// <param name="valeurs"></param>
        /// <param name="nbelt"></param>
        /// <returns></returns>
        private double[] SampleFromList(Random r,double[] valeurs, double nbelt)
        {
            var mean = valeurs.Sum() / valeurs.Length;
            var ecarType = valeurs.StandardDeviation();
            double[] rst = new double[(int)nbelt];
            if (valeurs.Length == nbelt)
            {
                return valeurs;
            }
            for (int i = 0; i < nbelt; i++)
            {
                var nombre = r.Next(0, valeurs.Length - 1);
                rst[i] = valeurs[nombre];
                var valtemp = new double[valeurs.Length - 1];
                for (int j = 0; j < valeurs.Length; j++)
                {
                    if (j < nombre)
                    {
                        valtemp[j] = valeurs[j];
                    }
                    else if (j > nombre)
                    {
                        valtemp[j - 1] = valeurs[j];
                    }
                }
                valeurs = valtemp;
            }
            var meanrst = rst.Sum() / nbelt;
            var ecarTypeRst = rst.StandardDeviation();
            var additif = mean - meanrst * ecarType / ecarTypeRst;
            var multiplicatif = (mean - additif) / meanrst;
            for (int i = 0; i < rst.Length; i++)
            {
                rst[i] = rst[i] * multiplicatif + additif;
            }
            meanrst = rst.Sum() / nbelt;
            ecarTypeRst = rst.StandardDeviation();
            return rst;
        }
    }
}
