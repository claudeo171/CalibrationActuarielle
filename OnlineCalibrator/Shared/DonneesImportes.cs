using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public class DonneesImportes
    {
        [Key(0)]
        public List<DonneesAAnalyser>? Donnees { get; set; }
        [Key(1)]
        public string? Nom { get; set; }

        [Key(2)]
        public string? NomData { get; set; }

        [Key(3)]
        public DonneesAAnalyser? ActualData => Donnees?.FirstOrDefault(a => a.Name == NomData);

        public byte[] ToMsgPack()
        {
            return MessagePack.MessagePackSerializer.Serialize(this);
        }

        public static DonneesImportes? FromMsgPack(byte[] json)
        {
            try
            {
                return MessagePack.MessagePackSerializer.Deserialize<DonneesImportes?>(json);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
