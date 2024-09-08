using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    public class ResultatIntervalles
    {
        public double[] BorneInf { get; set; }
        public double[] BorneSup { get; set; }
        public double[] Esperance { get; set; }
        public double Alpha { get; set; }
        public double Eta { get; set; }
        public int N { get; set; }
    }
}
