using Accord.Statistics;
using Stochastique.Autre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique
{
    public static class Helper
    {
        public static double Divide(this double a, double b,double c)
        {
            if (b == 0)
            {
                return c;
            }
            return a / b;
        }
        public static double[] CentrerReduire(this double[] value)
        {
            var mean = value.Mean();
            var sd = value.StandardDeviation() * Math.Sqrt( 1.0 * (value.Length - 1)/ value.Length);
            return value.Select(a => (a - mean) / sd).ToArray();
        }
        public static List<int> Rang(this List<double> valeurs)
        {
            return valeurs.SortIndex().SortIndex();
        }

        private static List<int> SortIndex(this List<double> tab)
        {
            List<ValIndice> l = new List<ValIndice>();
            for (int i = 0; i < tab.Count; i++)
            {
                l.Add(new ValIndice(i, tab[i]));
            }

            l.Sort(ValIndice.comparaison);
            List<int> sortIndex = new List<int>();
            for (int i = 0; i < tab.Count; i++)
            {
                sortIndex.Add(l[i].indice);
            }

            return sortIndex;
        }

        private static List<int> SortIndex(this List<int> tab)
        {
            List<double> d = new List<double>();
            foreach (int i in tab)
            {
                d.Add(i);
            }

            return d.SortIndex();
        }
        public static void ReordonnerSimulations(this List<double> valeurs, List<int> indices)
        {
            double[] ValeursTemp = new double[valeurs.Count];
            for (int i = 0; i < valeurs.Count; i++)
            {
                ValeursTemp[i]=valeurs[indices[i]];
            }
            for (int i = 0; i < valeurs.Count; i++)
            {
                valeurs[i]=ValeursTemp[i];
            }
        }
        public static List<double> UniformeEmpirique(this List<double> valeurs)
        {
            List<double> u = new List<double>();
            List<int> r = Rang(valeurs);
            double invNbSimulations = 1.0 / (valeurs.Count + 1);
            for (int i = 0; i < valeurs.Count; i++)
            {
                u.Add(invNbSimulations * (r[i] + 1));
            }

            return u;
        }
        public static string ToBeautifulString(this double d, bool isPercent = false)
        {
            string format = isPercent ? "P" : "N";
            if (d == 0)
            {
                return "0";
            }
            if (Math.Abs(d) < 0.001 && !isPercent)
            {
                return d.ToString("E6");
            }
            else if (Math.Abs(d) < 0.00001 && isPercent)
            {
                return d.ToString("E6");
            }
            else if (isPercent && d < 1)
            {
                return d.ToString(format + "4");
            }
            else if (Math.Abs(d) < 1)
            {
                return d.ToString(format + "6");
            }
            else if (Math.Abs(d) < 10)
            {
                return d.ToString(format + "5");
            }
            else if (Math.Abs(d) < 100)
            {
                return d.ToString(format + "4");
            }
            else if (Math.Abs(d) < 1000)
            {
                return d.ToString(format + "3");
            }
            else if (Math.Abs(d) > 10000000000 && !isPercent || Math.Abs(d) > 100000000 && isPercent)
            {
                return d.ToString("E6");
            }
            else
            {
                return d.ToString(format + "2");
            }
        }
    }
}
