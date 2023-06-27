using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public class DonneesImportes
    {
        public List<DonneesAAnalyser>? Donnees { get; set; }
        public string? Nom { get; set; }

        public string? NomData { get; set; }

        public DonneesAAnalyser? ActualData => Donnees?.FirstOrDefault(a => a.Name == NomData);

        public string ToJson()
        {
            try
            {
                var jsonParam = new JsonSerializerSettings();
                jsonParam.Error += (s, a) => a.ErrorContext.Handled = true;
                jsonParam.TypeNameHandling = TypeNameHandling.Auto;
                jsonParam.SerializationBinder=new CustomSerializationBinder();
                var rst = JsonConvert.SerializeObject(this, jsonParam);
                return rst;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static DonneesImportes? FromJson(string json)
        {
            try
            {
                var jsonParam = new JsonSerializerSettings();
                jsonParam.Error += (s, a) => a.ErrorContext.Handled = true;
                jsonParam.TypeNameHandling = TypeNameHandling.Auto;
                jsonParam.SerializationBinder = new CustomSerializationBinder();
                var rst = JsonConvert.DeserializeObject<DonneesImportes>(json, jsonParam);
                return rst;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
