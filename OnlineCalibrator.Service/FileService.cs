

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using OnlineCalibrator.Shared;
using Stochastique.Enums;
using System.Globalization;
using System.Text.RegularExpressions;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using A = DocumentFormat.OpenXml.Drawing;
using System.Reflection;

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
                donneesImportes = new DonneesImportes { Donnees = new List<DonneesAAnalyser>(), Nom = System.IO.Path.GetFileNameWithoutExtension(name) };
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
                                    donneesImportes.Donnees[i].Values[index - 1] = colonnes[i].Contains(',') ? double.Parse(colonnes[i], new CultureInfo("fr-FR")) : double.Parse(colonnes[i], new CultureInfo("en-US"));
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
            if (stream is MemoryStream ms)
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
        public byte[] ExportFileDocx(DonneesImportes donnees)
        {
            byte[] bytes;
            using MemoryStream stream = new MemoryStream();

            using (WordprocessingDocument doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                // Add a main document part.
                doc.AddMainDocumentPart();
                var mainPart = doc.MainDocumentPart;
                var temp=new GeneratedCode.GeneratedClass();
                temp.CreateMainDocumentPart(mainPart);
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                
                #region Create Puces
                // Création d'une liste à puces
                #endregion
                foreach (var v in donnees.Donnees)
                {
                    body.AddParagraph($"Calibration de la variable: {v.Name}", "Titre1");
                    
                    if (v.VisisbleData == null || v.VisisbleData.Count == 0)
                    {
                        body.AddParagraph("Aucune analyse n'a été réalisé sur cette variable");
                    }
                    else
                    {
                        body.AddParagraph($"Description de la donnée", "Titre2");
                        body.AddParagraph($"Valeurs numériques", "Titre3");

                        body.AddParagraph($"Les caractéristiques empiriques de la distribution sont les suivantes :");
                        body.AddPuceParagraphe($"Moyenne : {v.Moyenne}");
                        body.AddPuceParagraphe($"Variance empirique : {v.Moyenne}");
                        body.AddPuceParagraphe($"Skewness empirique : {v.Skewness}");
                        body.AddPuceParagraphe($"Kurtosis empirique : {v.Kurtosis}");
                        body.AddParagraph($"Graphiques", "Titre3");
                        body.AddParagraph($"Les éléments graphiques sont les suivants :");
                        var date = DateTime.Now.Ticks;
                        body.AddPuceParagraphe($"Estimateur à noyau de la densité :");
                        GenerationGraphique.SaveChartImage(v.PointsKDE, $"./KDE{date}",1000,500,true);
                        body.AddImageParagraphe(mainPart, $"./KDE{date}.png", 20, 10);
                        File.Delete($"./KDE{date}.png");
                        body.AddPuceParagraphe($"Fonction de répartition empirique");
                        GenerationGraphique.SaveChartImage(v.PointsCDF, $"./CDF{date}", 1000, 500, true);
                        body.AddImageParagraphe(mainPart, $"./CDF{date}.png", 20, 10);
                        File.Delete($"./CDF{date}.png");

                        body.AddParagraph($"Calibration de la donnée", "Titre2");
                        body.AddParagraph($"Hypothèses de calibration", "Titre3");

                        body.AddParagraph($"La donnée {v.Name} a été calibré avec les hypothèses suivantes :");
                        if (v.IsDiscreteDistribution)
                        {
                            body.AddPuceParagraphe($"La variable {v.Name} est discrète.");
                        }
                        else
                        {
                            body.AddPuceParagraphe($"La variable {v.Name} est continue.");
                        }
                        if (v.IncludeTrunkatedDistributions)
                        {
                            body.AddPuceParagraphe($"Les distributions tronquées ont été utilisés pour la calibration.");
                        }
                        else
                        {
                            body.AddPuceParagraphe($"Les distributions tronquées n'ont pas été utilisés pour la calibration.");
                        }
                        switch (v.MethodeCalibration)
                        {
                            case MethodeCalibrationRetenue.Manuelle:
                                body.AddPuceParagraphe($"La méthode de selection manuelle a été utilisé pour la calibration.");
                                break;
                            case MethodeCalibrationRetenue.AIC:
                                body.AddPuceParagraphe($"Le AIC a été utilisé pour la sélection de distribution.");
                                break;
                            case MethodeCalibrationRetenue.BIC:
                                body.AddPuceParagraphe($"Le BIC a été utilisé pour la sélection de distribution.");
                                break;
                            case MethodeCalibrationRetenue.Vraisemblance:
                                body.AddPuceParagraphe($"La vraisemblance a été utilisé pour la sélection de distribution.");
                                break;
                            case MethodeCalibrationRetenue.MachineLearningImage:
                                body.AddPuceParagraphe($"Le score de la calibration par machine learning pour la reconnaissance d'image a été utilisé pour la sélection de distribution.");
                                break;
                        }
                        body.AddParagraph($"Loi retenue", "Titre3");
                        body.AddParagraph($"La loi qui a été calibrée est une {v.CalibratedDistribution.Type}. Les paramètres de la loi sont les suivants :");
                        foreach(var param in v.CalibratedDistribution.AllParameters())
                        {
                            body.AddPuceParagraphe($"{param.Name} : {param.Value}");
                        }
                        body.AddParagraph($"Eléments de calibration", "Titre3");
                        body.AddParagraph($"Les valeurs des métriques de calibration obtenues pour la {v.CalibratedDistribution.Type} sont :");
                        body.AddPuceParagraphe($"Log-Vraissemblance : {v.VisisbleData.First(a=>a.Distribution.Type== v.CalibratedTypeDistribution).LogLikelihood}");
                        body.AddPuceParagraphe($"AIC : {v.VisisbleData.First(a => a.Distribution.Type == v.CalibratedTypeDistribution).AIC}");
                        body.AddPuceParagraphe($"BIC : {v.VisisbleData.First(a => a.Distribution.Type == v.CalibratedTypeDistribution).BIC}");

                        body.AddParagraph($"Le qqplot est le suivant :");
                        GenerationGraphique.SaveChartImage(v.GetQQPlot(v.CalibratedTypeDistribution), $"./QQ{date}", 1000, 500, true);
                        body.AddImageParagraphe(mainPart, $"./QQ{date}.png", 20, 10);
                        File.Delete($"./QQ{date}.png");

                        body.AddParagraph($"Les écarts sur les premiers moments sont les suivants :");
                        body.AddTable("TableauGrille5Fonc-Accentuation1", GetTableValueForEstimation(v));

                        body.AddParagraph($"Résultats de la calibration pour les autres lois :", "Titre2");
                        body.AddParagraph($"Les valeurs des métriques pour de calibration obtenue pour la loi sont :");
                        body.AddTable("TableauGrille5Fonc-Accentuation1", GetTableValueForGoodnessOfFit(v)) ;
                        body.AddPageBreak();
                    }
                }
            }
            bytes = stream.ToArray();

            return bytes;
        }
        private List<List<string>> GetTableValueForGoodnessOfFit(DonneesAAnalyser datas)
        {
            var columnHeader = new List<string> { "Type", "LogVraissemblance", "AIC", "BIC" };
            var result = new List<List<string>>();
            result.Add(columnHeader);
            foreach(var v in datas.VisisbleData)
            {
                var temp = new List<string>();
                temp.Add(v.Distribution.Type.ToString());
                temp.Add(v.LogLikelihood.ToBeautifulString());
                temp.Add(v.AIC.ToBeautifulString());
                temp.Add(v.BIC.ToBeautifulString());
                result.Add(temp);
            }
            return result;
        }
        private List<List<string>> GetTableValueForEstimation(DonneesAAnalyser datas)
        {
            var result = new List<List<string>>();
            var ev = datas.CalibratedDistribution.ExpextedValue();
            var variance = datas.CalibratedDistribution.Variance();
            double skewness = datas.CalibratedDistribution.Skewness();
            double kurtosis = datas.CalibratedDistribution.Kurtosis();


            result.Add(new List<string> { "Moment", "Valeur Empirique", "Valeur Théorique", "Ecart" });
            result.Add(new List<string> { "Esperance", datas.Moyenne.ToBeautifulString(), ev.ToBeautifulString(), ((datas.Moyenne-ev)/ datas.Moyenne).ToString("P4") });
            result.Add(new List<string> { "Variance", datas.Variance.ToBeautifulString(), variance.ToBeautifulString(), ((datas.Variance - variance) / datas.Variance).ToString("P4") }); ;
            result.Add(new List<string> { "Skewness", datas.Skewness.ToBeautifulString(), skewness.ToBeautifulString(), ((datas.Skewness - skewness) / datas.Skewness).ToString("P4") });
            result.Add(new List<string> { "Kurtosis", datas.Kurtosis.ToBeautifulString(), kurtosis.ToBeautifulString(), ((datas.Kurtosis - kurtosis) / datas.Kurtosis).ToString("P4") });
            return result;

        }


    }


    public static class WordHelper
    {
        public static string ToBeautifulString(this double d)
        {
            if(d==0)
            {
                return "0";
            }
            if (Math.Abs(d) < 0.001)
            {
                return d.ToString("E6");
            }
            else if (Math.Abs(d) < 1)
            {
                return d.ToString("F6");
            }
            else if (Math.Abs(d) < 10)
            {
                return d.ToString("F5");
            }
            else if (Math.Abs(d) < 100)
            {
                return d.ToString("F4");
            }
            else if (Math.Abs(d) < 1000)
            {
                return d.ToString("F3");
            }
            else
            {
                return d.ToString("F2");
            }
        }
        public static void AddPageBreak(this Body body)
        {
            Paragraph para = new Paragraph();
            Run run = new Run();
            Break pageBreak = new Break() { Type = BreakValues.Page };
            run.Append(pageBreak);
            para.Append(run);
            body.Append(para);
        }
        public static void AddParagraph(this Body body, string text, string style = null)
        {
            body.AddParagraph(text, JustificationValues.Left, style);
        }
        public static void AddParagraph(this Body body, string text, JustificationValues justif, string style)
        {
            Paragraph paragraph = new Paragraph();
            Run run_paragraph = new Run();
            Text text_paragraph = new Text(text);

            var paragraphProps = new ParagraphProperties();
            if (style != null)
            {

                paragraphProps.ParagraphStyleId = new ParagraphStyleId { Val = style };

            }
            //paragraphProps.Justification = new Justification() { Val = justif };
            run_paragraph.Append(text_paragraph);
            paragraph.Append(paragraphProps);
            paragraph.Append(run_paragraph);
            body.Append(paragraph);
        }

        public static void AddPuceParagraphe(this Body body, string text)
        {
            Paragraph listItem = new Paragraph(
                new ParagraphProperties(
                    new NumberingProperties(
                        new NumberingLevelReference() { Val = 0 },
                        new NumberingId() { Val = 1 }
                    )
                ),
                new Run(new Text(text))
            );

            body.Append(listItem);
        }
        public static void AddTable(this Body body, string style, List<List<string>> table)
        {
            Table table1 = new Table();

            TableProperties tableProperties1 = new TableProperties();
            TableStyle tableStyle1 = new TableStyle() { Val = style };
            TableWidth tableWidth1 = new TableWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableLook tableLook1 = new TableLook() { Val = "04A0" };

            tableProperties1.Append(tableStyle1);
            tableProperties1.Append(tableWidth1);
            tableProperties1.Append(tableLook1);

            TableGrid tableGrid1 = new TableGrid();
            for(int i = 0; i < table[0].Count;i++)
            {
                tableGrid1.Append(new GridColumn() { Width = (100/ table[0].Count).ToString() }) ;
            }
            table1.Append(tableProperties1);
            table1.Append(tableGrid1);
            for (int i=0;i<table.Count;i++)
            {
                TableRow row = new TableRow();
                if(i==0)
                {
                    TableRowProperties tableRowProperties1 = new TableRowProperties();
                    ConditionalFormatStyle conditionalFormatStyle1 = new ConditionalFormatStyle() { Val = "100000000000" };
                    row.Append(conditionalFormatStyle1);
                }
                else
                {
                    TableRowProperties tableRowProperties1 = new TableRowProperties();
                    ConditionalFormatStyle conditionalFormatStyle1 = new ConditionalFormatStyle() { Val = "000000100000" };
                    row.Append(conditionalFormatStyle1);
                }
                for(int j = 0; j < table[i].Count;j++)
                {
                    TableCell tableCell1 = new TableCell();
                    Paragraph paragraph17 = new Paragraph();
                    if (j == 0)
                    {
                        TableCellProperties tableCellProperties1 = new TableCellProperties();
                        ConditionalFormatStyle conditionalFormatStyle2 = new ConditionalFormatStyle() { Val = "001000000000" };
                        //TableCellWidth tableCellWidth1 = new TableCellWidth() { Width = "1", Type = TableWidthUnitValues.Pct };

                        tableCellProperties1.Append(conditionalFormatStyle2);
                        //tableCellProperties1.Append(tableCellWidth1);
                        

                        tableCell1.Append(tableCellProperties1);
                    }
                    else
                    {
                        TableCellProperties tableCellProperties1 = new TableCellProperties();
                        ConditionalFormatStyle conditionalFormatStyle2 = new ConditionalFormatStyle() { Val = "000000100000" };
                        //TableCellWidth tableCellWidth1 = new TableCellWidth() { Width = "1", Type = TableWidthUnitValues.Pct };
                        tableCellProperties1.Append(conditionalFormatStyle2);
                        //tableCellProperties1.Append(tableCellWidth1);
                        
                        tableCell1.Append(tableCellProperties1);
                    }
                    tableCell1.Append(paragraph17);
                    
                    Paragraph paragraph = new Paragraph();
                    Run run = new Run();
                    Text text = new Text(table[i][j]);
                    run.Append(text);
                    paragraph.Append(run);
                    tableCell1.Append(paragraph);
                    row.Append(tableCell1);
                }
                table1.Append(row);
            }

            body.Append(table1);

        }

        public static void AddImageParagraphe(this Body body, MainDocumentPart mainPart, string path, int cx, int cy)
        {
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);
            long LCX = cx * 261257L;
            long LCY = cy * 261257L;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = LCX, Cy = LCY },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.png"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                       "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = mainPart.GetIdOfPart(imagePart),
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = LCX, Cy = LCY }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            body.AppendChild(new Paragraph(new Run(element)));
        }
    }
}
