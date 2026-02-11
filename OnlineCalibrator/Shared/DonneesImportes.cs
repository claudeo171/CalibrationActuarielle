using Newtonsoft.Json;
using Stochastique;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Explicit)]
    public partial class DonneesImportes
    {
        [MemoryPack.MemoryPackOrder(0)]
        public List<DonneesAAnalyser>? Donnees { get; set; }
        [MemoryPack.MemoryPackOrder(1)]
        public string? Nom { get; set; }

        [MemoryPack.MemoryPackOrder(2)]
        public string? NomData { get; set; }
        [MemoryPack.MemoryPackOrder(3)]
        public DonneesAAnalyser? ActualData => Donnees?.FirstOrDefault(a => a.Name == NomData);

        [MemoryPack.MemoryPackOrder(4)]
        private List<DonneesPourAnalyseConjointe> DonneesPourAnalyseConjointes { get; set; } = new List<DonneesPourAnalyseConjointe>();

        [MemoryPack.MemoryPackOrder(5)]
        public string? NomDataConjointe1 { get; set; }
        [MemoryPack.MemoryPackOrder(6)]
        public string? NomDataConjointe2 { get; set; }

        [MemoryPack.MemoryPackOrder(7)]
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

        [MemoryPack.MemoryPackOrder(8)]
        private List<DonneesFrequenceCout> DonneesPourAnalyseFrequenceCout { get; set; } = new List<DonneesFrequenceCout>();

        [MemoryPack.MemoryPackOrder(9)]
        public string? NomDataCout { get; set; }
        [MemoryPack.MemoryPackOrder(10)]
        public string? NomDataCategorie { get; set; }

        [MemoryPack.MemoryPackOrder(11)]
        public DonneesFrequenceCout? ActualDonneesPourAnalyseFrequenceCout
        {
            get
            {
                if (NomDataCout == null || NomDataCategorie == null)
                {
                    return null;
                }
                else
                {
                    if (DonneesPourAnalyseFrequenceCout.Any(a => a.Cout.Name == NomDataCout && a.Classification.Name == NomDataCategorie))
                    {
                        return DonneesPourAnalyseFrequenceCout.First(a => a.Cout.Name == NomDataCout && a.Classification.Name == NomDataCategorie);
                    }
                    else
                    {
                        var rst = new DonneesFrequenceCout ( Donnees.First(a => a.Name == NomDataCout), Donnees.First(a => a.Name == NomDataCategorie));
                        DonneesPourAnalyseFrequenceCout.Add(rst);
                        return rst;
                    }
                }
            }
        }



        public byte[] ToMsgPack()
        {
            using var compressor = new MemoryPack.Compression.BrotliCompressor();
            MemoryPack.MemoryPackSerializer.Serialize(compressor,this);
            return compressor.ToArray();
        }

        public static DonneesImportes? FromMsgPack(byte[] json)
        {
            try
            {
                using var decompressor = new MemoryPack.Compression.BrotliDecompressor();
                var decompressedBuffer = decompressor.Decompress(json);
                return MemoryPack.MemoryPackSerializer.Deserialize<DonneesImportes?>(decompressedBuffer);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
