using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    [MessagePack.MessagePackObject]
    public class ResultatIntervalles
    {
        [MessagePack.Key(0)]
        public double[] BorneInf { get; set; }
        [MessagePack.Key(1)]
        public double[] BorneSup { get; set; }
        [MessagePack.Key(2)]
        public double[] Esperance { get; set; }
        [MessagePack.Key(3)]
        public double Alpha { get; set; }
        [MessagePack.Key(4)]
        public double Eta { get; set; }
        [MessagePack.Key(5)]
        public int N { get; set; }
    }
}
