using MessagePack;
using Newtonsoft.Json;
using Stochastique;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public partial class DonneesImportes
    {
        [Key(0)]
        public List<DonneesAAnalyser>? Donnees { get; set; }
        [Key(1)]
        public string? Nom { get; set; }

        [Key(2)]
        public string? NomData { get; set; }
        [Key(3)]
        public DonneesAAnalyser? ActualData => Donnees?.FirstOrDefault(a => a.Name == NomData);

        [Key(4)]
        private List<DonneesPourAnalyseConjointe> DonneesPourAnalyseConjointes { get; set; } = new List<DonneesPourAnalyseConjointe>();

        [Key(5)]
        public string? NomDataConjointe1 { get; set; }
        [Key(6)]
        public string? NomDataConjointe2 { get; set; }

        [Key(7)]
        public DonneesPourAnalyseConjointe? ActualDonneesPourAnalyseConjointe
        {
            get
            {
                if(NomDataConjointe1==null || NomDataConjointe2==null)
                {
                    return null;
                }
                else
                {
                    if(DonneesPourAnalyseConjointes.Any(a=>a.DonneesAAnalyser1.Name==NomDataConjointe1 && a.DonneesAAnalyser2.Name == NomDataConjointe2))
                    {
                        return DonneesPourAnalyseConjointes.First(a => a.DonneesAAnalyser1.Name == NomDataConjointe1 && a.DonneesAAnalyser2.Name == NomDataConjointe2);
                    }
                    else
                    {
                        var rst = new DonneesPourAnalyseConjointe { DonneesAAnalyser1 = Donnees.First(a => a.Name == NomDataConjointe1), DonneesAAnalyser2 = Donnees.First(a => a.Name == NomDataConjointe2) };
                        DonneesPourAnalyseConjointes.Add(rst);
                        return rst;
                    }
                }
            }
        }



        public byte[] ToMsgPack()
        {
            var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
            return MessagePack.MessagePackSerializer.Serialize(this, lz4Options);
        }

        public static DonneesImportes? FromMsgPack(byte[] json)
        {
            try
            {
                var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
                return MessagePack.MessagePackSerializer.Deserialize<DonneesImportes?>(json, lz4Options);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
