using DocumentFormat.OpenXml.Drawing;
using Microsoft.ML.Data;
using OnlineCalibrator.Shared;
using pax.BlazorChartJs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.SharedPages.ViewModel
{
    public class ChartMatrixViewModel
    {
        public ChartJsConfig ChartJsConfig { get; set; }
        public ChartMatrixViewModel(ConfusionMatrix confusion, List<string> titres)
        {
            List<MatrixPoint> points = new List<MatrixPoint>();
            for (int i = 0; i < confusion.NumberOfClasses; i++)
            {
                var row = new List<MatrixPoint>();
                for (int j = 0; j < confusion.NumberOfClasses; j++)
                {
                    row.Add(new MatrixPoint { X = titres[i], Y = titres[j], V = confusion.GetCountForClassPair(i, j) });
                }
                var sum = row.Sum(a => a.V);
                row.ForEach(a => a.V /= sum);
                points.AddRange(row);
            }
            ChartJsConfig = new ChartJsConfig
            {
                Type = ChartType.matrix,
                Data = new ChartJsData
                {
                    Datasets = new List<ChartJsDataset>
                    {
                        new MatrixDataset{
                            Data = points.OfType<object>().ToList(),
                            BackgroundColor=points.Select(a=>a.X==a.Y?$"rgba(0,255,0,{(a.V/2).ToString(new CultureInfo("en-GB"))})":$"rgba(255,0,0,{a.V.ToString(new CultureInfo("en-GB"))}").ToList()
                        }
                    }
                },
                Options = new ChartJsOptions
                {
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Plugins = new Plugins
                    {
                        Legend = new Legend
                        {
                            Display = false
                        }
                    },
                    Scales = new ChartJsOptionsScales
                    {
                        Y = new ChartJSCategoryAxis
                        {
                            Type = "category",
                            Labels = titres.ToArray(),
                            Offset = true,
                            Ticks = new ChartJsAxisTick
                            {
                                Display = true
                            },
                            Grid = new ChartJsGrid
                            {
                                Display = false
                            }

                        },
                        X = new ChartJSCategoryAxis
                        {
                            Type = "category",
                            Labels = titres.Reverse<string>().ToArray(),
                            Offset = true,
                            Ticks = new ChartJsAxisTick
                            {
                                Display = true
                            },
                            Grid = new ChartJsGrid
                            {
                                Display = false
                            }

                        }
                    }
                }

            };
        }
    }
}
