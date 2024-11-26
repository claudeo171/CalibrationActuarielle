using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.FSharp.Core.ByRefKinds;

namespace Stochastique.Copule
{
    public class Paire
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Paire(double x, double y)
        {
            X = x;
            Y = y;
        }
        public static int comparaisonX(Paire p1, Paire p2)
        {
            return Math.Sign(p1.X - p2.X);
        }

        public static int comparaisonY(Paire p1, Paire p2)
        {
            return Math.Sign(p1.Y - p2.Y);
        }

        public static int comparaisonXY(Paire p1, Paire p2)
        {
            if (p1.X == p2.X)
            {
                return comparaisonY(p1, p2);
            }
            else
            {
                return comparaisonX(p1, p2);
            }
        }

        public static bool operator !=(Paire p1, Paire p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }
        public static bool operator ==(Paire p1, Paire p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked
            {
                hashCode += 1000000007 * X.GetHashCode();
                hashCode += 1000000009 * Y.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            Paire other = obj as Paire;
            if (other == null)
                return false;
            return object.Equals(this.X, other.X) && object.Equals(this.Y, other.Y);
        }
    }
    public static class CopuleHelper
    {

        public static List<int> Rang(this IEnumerable<double> data)
        {
            int count = data.Count();
            List<int> rang = new int[count].ToList();
            List<(int indice, double valeur)> dataCopy = data.Select<double, (int indice, double valeur)>((a,i)=>new( i, a)).ToList();
            dataCopy=dataCopy.OrderBy(a=>a.valeur).ToList();
            for (int i = 0; i < count; i++)
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
            for (int i = 0; i < valeur.Count; i++)
            {
                valeur[i] = ValeursTemp[i];
            }
        }

        public static double NegativeProd(int value,double decalage=0)
        {
            double rst = 1;
            for(int i= 1; i <= value; i++)
            {
                rst *= -(i+decalage);
            }
            return rst;
        }

        public static List<List<T>> IntervertDimention<T>(this IEnumerable<IEnumerable<T>> values)
        {
            var valueAsList = values.Select(a => a.ToList()).ToList();
            List<List<T>> rst = new List<List<T>>();
            int y = valueAsList.Count;
            int x = valueAsList[0].Count;
            for (int i=0;i<x;i++)
            {
                rst.Add(new List<T>());
                for (int j = 0; j < y; j++)
                {
                    rst[i].Add(valueAsList[j][i]);
                }
            }
            return rst;
        }

        public static double Prod(this IEnumerable<double> values)
        {
            double rst = 1;
            foreach (var value in values)
            {
                rst *= value;
            }
            return rst;
        }
        public static double Prod<T>(this IEnumerable<T> values,Func<T,double> func)
        {
            double rst = 1;
            foreach (var value in values)
            {
                rst *= func(value);
            }
            return rst;
        }

        public static double TauKendall(this IEnumerable<double> elt1, IEnumerable<double> elt2)
        {
            var list1 = elt1.ToList();
            var list2 = elt2.ToList();
            //Création des paires
            int nbSim = elt1.Count();
            List<Paire> paires = new List<Paire>();
            for (int i = 0; i < nbSim; i++)
            {
                paires.Add(new Paire(list1[i], list2[i]));
            }

            //Comptage eX
            paires.Sort(Paire.comparaisonY);
            int eX = 0;
            for (int i = 0; i < nbSim - 1; i++)
            {
                if (paires[i].Y == paires[i + 1].Y && paires[i].X != paires[i + 1].X)
                {
                    eX++;
                }
            }

            //Comptage eY
            paires.Sort(Paire.comparaisonX);
            int eY = 0;
            for (int i = 0; i < nbSim - 1; i++)
            {
                if (paires[i].X == paires[i + 1].X && paires[i].Y != paires[i + 1].Y)
                {
                    eY++;
                }
            }

            //Comptage paires égales et élimination des doublons
            paires.Sort(Paire.comparaisonXY);
            List<Paire> pairesUniques = new List<Paire>();
            int spare = 0;
            for (int i = 0; i < nbSim - 1; i++)
            {
                if (paires[i] != paires[i + 1])
                {
                    pairesUniques.Add(paires[i]);
                }
                else
                {
                    spare++;
                }
            }

            //Calcul du taux de Kendall en O(n log(n)) sur le principe du tri par fusion
            int nbPaires = (int)Math.Round(0.5 * nbSim * (nbSim - 1));
            List<double> y = new List<double>();
            foreach (Paire p in pairesUniques)
            {
                y.Add(p.Y);
            }

            double s = 2 * NbSwapFusion(y);

            return (nbPaires - s) / Math.Sqrt((nbPaires - eX - spare) * (nbPaires - eY - spare));
        }
        private static int NbSwapFusion(List<double> tableau)
        {
            int nbSwap = 0;
            List<List<double>> tableauxIteration = new List<List<double>>();
            List<List<double>> tableauxIterationPrecedente = new List<List<double>>();
            for (int i = 0; i < tableau.Count; i++)
            {
                tableauxIteration.Add(new List<double> { tableau[i] });
            }

            while (tableauxIteration.Count > 1)
            {
                tableauxIterationPrecedente = tableauxIteration;
                tableauxIteration = new List<List<double>>();
                for (int i = 0; i < tableauxIterationPrecedente.Count / 2; i++)
                {
                    int lenT1 = tableauxIterationPrecedente[i * 2].Count;
                    int lenT2 = tableauxIterationPrecedente[i * 2 + 1].Count;
                    int indT1 = 0;
                    int indT2 = 0;
                    tableauxIteration.Add(new List<double>());
                    for (int j = 0; j < lenT1 + lenT2; j++)
                    {
                        if (indT1 == lenT1)
                        {
                            tableauxIteration[i].Add(tableauxIterationPrecedente[2 * i + 1][indT2]);
                            indT2++;
                        }
                        else if (indT2 == lenT2)
                        {
                            tableauxIteration[i].Add(tableauxIterationPrecedente[2 * i][indT1]);
                            indT1++;
                        }
                        else if (tableau[indT1] > tableau[indT2])
                        {
                            tableauxIteration[i].Add(tableauxIterationPrecedente[2 * i + 1][indT2]);
                            nbSwap += lenT1 - indT1 + 1;
                            indT2++;
                        }
                        else
                        {
                            tableauxIteration[i].Add(tableauxIterationPrecedente[2 * i][indT1]);
                            indT1++;
                        }
                    }
                }
            }

            return nbSwap;
        }

        public static List<double> GetCDF(this List<List<double>> values)
        {
            List<double> rst = new List<double>();
            foreach (var point in values)
            {
                var rstPoint = 0;
                foreach (var pointForComparaison in values)
                {
                    bool isUnder = true;
                    for (int i = 0; i < point.Count; i++)
                    {
                        isUnder = isUnder && pointForComparaison[i] <= point[i];
                    }
                    if (isUnder)
                        rstPoint += 1;
                }
                rst.Add(rstPoint / values.Count);
            }
            return rst;
        }

        public static double RechercheDichotomique(double min,double max, Func<double,double> method)
        {
            double x = (min + max) / 2;
            double y = method(x);
            double yMin = method(min);
            double yMax = method(max);
            if (yMin * yMax > 0)
            {
                return Math.Abs(yMin)> Math.Abs(yMax)? max:min;
            }    
            while (Math.Abs(y) > 0.0001)
            {
                if (y * yMin > 0)
                {
                    min = x;
                    yMin = y;
                }
                else
                {
                    max = x;
                    yMax = y;
                }
                x = (min + max) / 2;
                y = method(x);
            }
            return x;
        }

        public static double Divide(this double a, double b, double defaultValue)
        {
            if (b == 0)
                return defaultValue;
            return a / b;
        }

    }
}
