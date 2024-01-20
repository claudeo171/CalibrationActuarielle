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
        [Key(4)]
        public string? NomDataConjointe1 { get; set; }

        [Key(5)]
        public DonneesAAnalyser? ActualDataConjointe1 => Donnees?.FirstOrDefault(a => a.Name == NomDataConjointe1);
        [Key(6)]
        public string? NomDataConjointe2 { get; set; }

        [Key(7)]
        public DonneesAAnalyser? ActualDataConjointe2 => Donnees?.FirstOrDefault(a => a.Name == NomDataConjointe2);
        [IgnoreMember]
        public Point[] ScatterPlot => ActualDataConjointe1.Values.Select((a, i) => new Point() { X = a, Y = ActualDataConjointe2.Values[i] }).ToArray();
        [IgnoreMember]
        public Point[] ScatterPlotRank
        {
            get
            {
                var xValueOrdered = ActualDataConjointe1.Values.Select<double,(double valeur, int indice)>((a, i) => new ( a, i )).OrderBy(a => a.indice).ToArray();
                var yValueOrdered = ActualDataConjointe2.Values.Select<double, (double valeur, int indice)>((a, i) => new(a, i)).OrderBy(a => a.indice).ToArray();
                Point[] rst = new Point[xValueOrdered.Length];
                for (int i = 0; i < ActualDataConjointe1.Values.Length; i++)
                {
                    if(rst[xValueOrdered[i].indice]== null)
                    {
                        rst[xValueOrdered[i].indice] = new Point();
                    }
                    if (rst[yValueOrdered[i].indice] == null)
                    {
                        rst[yValueOrdered[i].indice] = new Point();
                    }
                    rst[xValueOrdered[i].indice].X = (double)i;
                    rst[yValueOrdered[i].indice].Y = (double)i;
                }
                return rst;
            }
        }

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
