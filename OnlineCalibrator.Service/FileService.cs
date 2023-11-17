

using OnlineCalibrator.Shared;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OnlineCalibrator.Service
{
    public class FileService
    {
        private static string[] SplitCSV(string input)
        {
            return input.Split(';');
        }
        public static DonneesImportes GetDataFromFile(Stream singleFile, string name)
        {
            var donneesImportes = new DonneesImportes();
            Regex regex = new Regex(".+\\.csv", RegexOptions.Compiled);
            if (!regex.IsMatch(name))
            {
                regex = new Regex(".+\\.data", RegexOptions.Compiled);
                if (regex.IsMatch(name))
                {

                    donneesImportes = DonneesImportes.FromMsgPack(UseStreamDotReadMethod(singleFile));

                    donneesImportes.NomData = donneesImportes.Donnees.FirstOrDefault()?.Name;
                }
                else
                {
                    throw new Exception("Le fichier n'est pas au bon format");
                }
            }
            else
            {
                donneesImportes = new DonneesImportes { Donnees = new List<DonneesAAnalyser>(), Nom = Path.GetFileNameWithoutExtension(name) };
                var outputFileString = System.Text.Encoding.UTF8.GetString(UseStreamDotReadMethod(singleFile));
                int index = 0;
                var lignes = outputFileString.Split(Environment.NewLine);
                try
                {
                    foreach (var item in lignes)
                    {
                        if (item != string.Empty)
                        {
                            if (index == 0)
                            {
                                foreach (var header in SplitCSV(item.ToString()))
                                {
                                    donneesImportes.Donnees.Add(new DonneesAAnalyser { Name = header, Values = new double[lignes.Count(a => a != string.Empty) - 1] });
                                }
                            }
                            else
                            {
                                var colonnes = SplitCSV(item.ToString());
                                for (int i = 0; i < colonnes.Length; i++)
                                {
                                    donneesImportes.Donnees[i].Values[index - 1] = colonnes[i].Contains(',')? double.Parse(colonnes[i], new CultureInfo("fr-FR")): double.Parse(colonnes[i], new CultureInfo("en-US"));
                                }
                            }
                        }
                        index++;
                    }
                    foreach (var v in donneesImportes.Donnees)
                    {
                        v.Initialize();
                    }
                    donneesImportes.NomData = donneesImportes.Donnees.FirstOrDefault()?.Name;

                }
                catch (Exception ex)
                {

                    throw new Exception("Le fichier csv n'est pas au bon format. Il faut que la première ligne soit au format texte et que tout le reste soit des nombres.");
                }

            }
            return donneesImportes;
        }

        public static byte[] UseStreamDotReadMethod(Stream stream)
        {
            if(stream is MemoryStream ms)
            {
                return ms.ToArray();
            }
            byte[] bytes;
            List<byte> totalStream = new();
            byte[] buffer = new byte[32];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalStream.AddRange(buffer.Take(read));
            }
            bytes = totalStream.ToArray();
            return bytes;
        }
    }
}
