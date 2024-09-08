using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    public class DoubleBuffer<T>
    {
        public T[] Buff0 { get; set; }
        public T[] Buff1 { get; set; }

        public bool IsBuff0SRC { get; set; }

        public T[] get_src => IsBuff0SRC ? Buff0 : Buff1;
        public T[] get_dest => IsBuff0SRC ? Buff1 : Buff0;
        public void flip()
        {
            IsBuff0SRC = !IsBuff0SRC;
        }
        public DoubleBuffer(int n, T value)
        {
            Buff0 = new T[n];
            Buff1 = new T[n];
            for (int i = 0;i<n;i++)
            {
                Buff0[i] = value;
                Buff1[i] = value;
            }
            IsBuff0SRC = true;
        }
    }
}
