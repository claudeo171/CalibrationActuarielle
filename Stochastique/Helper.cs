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
