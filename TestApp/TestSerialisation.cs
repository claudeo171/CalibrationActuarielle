using OnlineCalibrator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public static class TestSerialisation
    {
        public static byte[] ToMsgPack<T>(this T obj)
        {
            return MemoryPack.MemoryPackSerializer.Serialize(obj);
        }

        public static T? FromMsgPack<T>(this byte[] json) where T:class
        {
            try
            {
                return MemoryPack.MemoryPackSerializer.Deserialize<T?>(json);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
