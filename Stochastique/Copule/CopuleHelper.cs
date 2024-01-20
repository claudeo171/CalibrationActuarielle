using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Copule
{
    public static class CopuleHelper
    {
        public static List<int> Rang(this List<double> data)
        {
            List<int> rang = new int[data.Count].ToList();
            List<(int indice, double valeur)> dataCopy = data.Select<double, (int indice, double valeur)>((a,i)=>new( i, a)).ToList();
            dataCopy=dataCopy.OrderBy(a=>a.valeur).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                rang[dataCopy[i].indice]=i;
            }
            return rang;
        }
        public static List<int> SortIndex(this List<double> data)
        {
            List<int> sortIndex = new int[data.Count].ToList();
            List<(int indice, double valeur)> dataCopy = data.Select<double, (int indice, double valeur)>((a,i)=>new( i, a)).ToList();
            dataCopy=dataCopy.OrderBy(a=>a.valeur).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                sortIndex[i] = dataCopy[i].indice;
            }
            return sortIndex;
        }
        public static void ReordonnerSimulations(this List<double> valeur, List<int> indices)
        {
            List<double> ValeursTemp = new List<double>();
            for (int i = 0; i < valeur.Count; i++)
            {
                ValeursTemp.Add(valeur[indices[i]]);
            }

            valeur = ValeursTemp;
        }

        public static double NegativeProd(int value)
        {
            double rst = 1;
            for(int i= 1; i <= value; i++)
            {
                rst *= -i;
            }
            return rst;
        }

    }
}
