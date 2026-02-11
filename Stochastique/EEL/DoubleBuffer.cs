using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.EEL
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class DoubleBuffer<T>
    {
        [MemoryPack.MemoryPackOrder(0)]
        public T[] Buff0 { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public T[] Buff1 { get; set; }
        [MemoryPack.MemoryPackOrder(2)]
        public bool IsBuff0SRC { get; set; }
        [MemoryPack.MemoryPackIgnore]
        public T[] get_src => IsBuff0SRC ? Buff0 : Buff1;
        [MemoryPack.MemoryPackIgnore]
        public T[] get_dest => IsBuff0SRC ? Buff1 : Buff0;
        public void flip()
        {
            IsBuff0SRC = !IsBuff0SRC;
        }
        [MemoryPack.MemoryPackConstructor]
        public DoubleBuffer()
        {

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
