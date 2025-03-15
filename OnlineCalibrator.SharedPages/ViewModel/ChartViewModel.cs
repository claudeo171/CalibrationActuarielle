using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using MathNet.Numerics;
using MessagePack;
using SkiaSharp;
using Stochastique;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pax.BlazorChartJs;

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public class ChartViewModelLine
    {
        public ChartJsConfig ChartJsConfig { get; set; }
        public ChartViewModelLine(Point[] valeurs)
        {
            Series = new ISeries[0];
            AddSerie(valeurs, null, new SolidColorPaint(SKColors.CornflowerBlue), 0, 0, null, false);
            ChartJsConfig = new ChartJsConfig
            {
                Type = ChartType.scatter,
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
                    }
                },
                Data = new ChartJsData
                {
                    Datasets = new List<ChartJsDataset>
                    {
                        new ScatterDataset() {
                            ShowLine=true,
                            Data= valeurs.Select(a=>new DataPoint{ X=a.X, Y=a.Y }).OfType<object>().ToList()
                            
                        }
                    }
                }
            };
        }
        public ChartViewModelLine(List<Point[]> valeurs, List<SolidColorPaint> stroke, List<SolidColorPaint> fill, List<double> size, List<SolidColorPaint> color, bool differentAxes = false, bool rectangularSections = false)
        {
            Series = new ISeries[0];
            if (differentAxes)
            {
                YAxes = new Axis[valeurs.Count];
            }
            for (int i = 0; i < valeurs.Count; i++)
            {
                AddSerie(valeurs[i], stroke[i], fill[i], i, size[i], color[i], differentAxes);
                if (differentAxes)
                {
                    YAxes[i] = new Axis();
                }
            }

            ChartJsConfig = new ChartJsConfig
            {
                Type = ChartType.scatter,
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
                    }
                },

                Data = new ChartJsData
                {
                    Datasets = valeurs.Select((a, i) =>

                        new ScatterDataset()
                        {
                            ShowLine = true,

                            Data = a.Select(b => new DataPoint { X = b.X, Y = b.Y }).OfType<object>().ToList()

                        }
                    ).OfType<ChartJsDataset>().ToList()
                },


            };
            if(differentAxes)
            {
                ChartJsConfig.Options.Scales = !differentAxes ? null : new ChartJsOptionsScales
                {
                    Y = new CartesianAxis
                    {
                        Type = "linear",
                        Display = true,
                        Position = "left",
                        Ticks=new ChartJsAxisTick { Color = "blue" }
                    },
                    Y1 = new CartesianAxis
                    {
                        Type = "linear",
                        Display = true,
                        Position = "right",
                        Grid = new ChartJsGrid { DrawOnChartArea = false },
                        Ticks = new ChartJsAxisTick
                        {
                            Color = "red"
                        },
                        Max= valeurs[1].Select(a=>a.Y).Max()*1.1,
                    },

                };
                for(int i=0;i< ChartJsConfig.Data.Datasets.Count;i++)
                {
                    if (i == 0)
                    {
                        (ChartJsConfig.Data.Datasets[i] as ScatterDataset).YAxisID = "y";
                    }
                    else
                    {
                        (ChartJsConfig.Data.Datasets[i] as ScatterDataset).YAxisID = "y1";
                    }
                }
            }
        }

        public void Update(List<Point[]> valeurs, List<SolidColorPaint> stroke, List<SolidColorPaint> fill, List<double> size, List<SolidColorPaint> color, bool differentAxes = false, bool rectangularSections = false)
        {
            Series = new ISeries[0];
            if (differentAxes)
            {
                YAxes = new Axis[valeurs.Count];
            }
            for (int i = 0; i < valeurs.Count; i++)
            {
                AddSerie(valeurs[i], stroke[i], fill[i], i, size[i], color[i], differentAxes);
                if (differentAxes)
                {
                    YAxes[i] = new Axis();
                }
            }

            ChartJsConfig.Data.Datasets = valeurs.Select((a, i) =>

                        new ScatterDataset()
                        {
                            ShowLine = true,

                            Data = a.Select(b => new DataPoint { X = b.X, Y = b.Y }).OfType<object>().ToList()

                        }
                    ).OfType<ChartJsDataset>().ToList();
        }
        public ChartViewModelLine()
        {

        }
        public ChartViewModelLine InitialiseQuantile(Point[] valeurs)
        {
            Series = new ISeries[]{
            new LineSeries<ObservablePoint>
            {
                Values = valeurs.Select(a => new ObservablePoint { X = a.X, Y = a.Y }).ToList(),
                Name = "Quantile",
                GeometryFill= new SolidColorPaint(SKColors.Green),
                GeometryStroke = new SolidColorPaint(SKColors.Black),
                GeometrySize = 7,
                Fill=null,
                Stroke=null
            }.OnPointMeasured(point =>
            {
                if (point.Visual is null) return;

                if(point.Model is null) return;

                if(point.Model.Y<0.005 || point.Model.Y > 0.995)
                {
                    point.Visual.Stroke = new SolidColorPaint(SKColors.Black);
                    point.Visual.Fill = new SolidColorPaint(SKColors.DarkRed);
                }
                else if (point.Model.Y < 0.025 || point.Model.Y > 0.975)
                {
                    point.Visual.Stroke = new SolidColorPaint(SKColors.Black);
                    point.Visual.Fill = new SolidColorPaint(SKColors.DarkOrange);
                }
                else if (point.Model.Y < 0.05 || point.Model.Y > 0.95)
                {
                    point.Visual.Stroke = new SolidColorPaint(SKColors.Black);
                    point.Visual.Fill = new SolidColorPaint(SKColors.Yellow);
                }
            })
            };
            ChartJsConfig = new ChartJsConfig
            {
                Type = ChartType.scatter,
                Options = new ChartJsOptions
                {
                    Responsive = true,
                    MaintainAspectRatio=false,
                    Plugins = new Plugins
                    {
                        Legend = new Legend
                        {
                            Display = false
                        }
                    }
                },
                Data = new ChartJsData
                {
                    Datasets = new List<ChartJsDataset>
                    {
                        new ScatterDataset() {
                            Data= valeurs.Select(a=>new DataPoint{ X=a.X, Y=a.Y }).OfType<object>().ToList(),
                            PointBackgroundColor=  valeurs.Select(a=>a.Y<0.005 || a.Y>0.995?"red":(a.Y<0.025 || a.Y>0.975?"orange":(a.Y<0.05 || a.Y>0.95?"yellow":"green"))).OfType<string>().ToList()

                        }
                    }
                }
            };
            AddRectangularSection(valeurs);
            return this;
        }

        public void UpdateQuantile(Point[] valeurs)
        {
            Series = new ISeries[]{
            new LineSeries<ObservablePoint>
            {
                Values = valeurs.Select(a => new ObservablePoint { X = a.X, Y = a.Y }).ToList(),
                Name = "Quantile",
                GeometryFill= new SolidColorPaint(SKColors.Green),
                GeometryStroke = new SolidColorPaint(SKColors.Black),
                GeometrySize = 7,
                Fill=null,
                Stroke=null
            }.OnPointMeasured(point =>
            {
                if (point.Visual is null) return;

                if(point.Model is null) return;

                if(point.Model.Y<0.005 || point.Model.Y > 0.995)
                {
                    point.Visual.Stroke = new SolidColorPaint(SKColors.Black);
                    point.Visual.Fill = new SolidColorPaint(SKColors.DarkRed);
                }
                else if (point.Model.Y < 0.025 || point.Model.Y > 0.975)
                {
                    point.Visual.Stroke = new SolidColorPaint(SKColors.Black);
                    point.Visual.Fill = new SolidColorPaint(SKColors.DarkOrange);
                }
                else if (point.Model.Y < 0.05 || point.Model.Y > 0.95)
                {
                    point.Visual.Stroke = new SolidColorPaint(SKColors.Black);
                    point.Visual.Fill = new SolidColorPaint(SKColors.Yellow);
                }
            })
            };
            ChartJsConfig.Data.Datasets[0] = new ScatterDataset()
            {
                Data = valeurs.Select(a => new DataPoint { X = a.X, Y = a.Y }).OfType<object>().ToList(),
                PointBackgroundColor = valeurs.Select(a => a.Y < 0.005 || a.Y > 0.995 ? "red" : (a.Y < 0.025 || a.Y > 0.975 ? "orange" : (a.Y < 0.05 || a.Y > 0.95 ? "yellow" : "green"))).OfType<string>().ToList()

            };
        }
        public void AddRectangularSection(Point[] points)
        {
            RectangularSection = new RectangularSection[]{
                new RectangularSection
                {
                    Yj = 0.005,
                    Yi = 0,
                    Fill = new SolidColorPaint(SKColors.Red.WithAlpha(85))
                },
                new RectangularSection
                {
                    Yj = 0.025,
                    Yi = 0.005,
                    Fill = new SolidColorPaint(SKColors.Orange.WithAlpha(85))
                },
                new RectangularSection
                {
                    Yj = 0.05,
                    Yi = 0.025,
                    Fill = new SolidColorPaint(SKColors.Yellow.WithAlpha(85))
                },
                new RectangularSection
                {
                    Yj = 1,
                    Yi = 0.995,
                    Fill = new SolidColorPaint(SKColors.Red.WithAlpha(85))
                },
                new RectangularSection
                {
                    Yj = 0.995,
                    Yi = 0.975,
                    Fill = new SolidColorPaint(SKColors.Orange.WithAlpha(85))
                },
                new RectangularSection
                {
                    Yj = 0.975,
                    Yi = 0.95,
                    Fill = new SolidColorPaint(SKColors.Yellow.WithAlpha(85))
                },
            };

            ChartJsConfig.Options.Plugins.Annotation = new AnnotationsData
            {
                Annotations = new List<Annotation>
                {
                    new Annotation {  yMax=1, yMin=0.995, backgroundColor="rgba(255, 0, 0, 0.5)",borderColor="rgb(255, 0, 0)", borderWidth=1,drawTime="beforeDatasetsDraw", type="box"},
                    new Annotation {  yMax=0.005, yMin=0, backgroundColor="rgba(255, 0, 0, 0.5)",borderColor="rgb(255, 0, 0)", borderWidth=1,drawTime="beforeDatasetsDraw", type="box"},
                    new Annotation {  yMax=0.025, yMin=0.005, backgroundColor="rgba(255, 165, 0, 0.5)",borderColor="rgb(255, 165, 0)", borderWidth=1,drawTime="beforeDatasetsDraw", type="box"},
                    new Annotation { yMax=0.995, yMin=0.975, backgroundColor="rgba(255, 165, 0, 0.5)",borderColor="rgb(255, 165, 0)", borderWidth=1,drawTime="beforeDatasetsDraw", type="box"},
                    new Annotation { yMax=0.05, yMin=0.025, backgroundColor="rgba(255, 255, 0, 0.5)",borderColor="rgb(255, 255, 0)", borderWidth=1,drawTime="beforeDatasetsDraw", type="box"},
                    new Annotation {  yMax=0.975, yMin=0.95, backgroundColor="rgba(255, 255, 0, 0.5)",borderColor="rgb(255, 255, 0)", borderWidth=1,drawTime="beforeDatasetsDraw", type="box"},
                }
            };  
        }
        public void AddSerie(Point[] valeurs, SolidColorPaint? stroke, SolidColorPaint? fill, int indice, double size, SolidColorPaint color, bool differentAxes)
        {
            var serieAsList = Series.ToList();
            serieAsList.Add(new LineSeries<ObservablePoint>
            {
                Values = valeurs.Select(a => new ObservablePoint { X = a.X, Y = a.Y }).ToList(),
                Fill = fill, // mark
                Stroke = stroke,
                GeometryFill = color == null ? null : new SolidColorPaint(color.Color),
                GeometryStroke = color == null ? null : new SolidColorPaint(color.Color) { StrokeThickness = 2 },
                GeometrySize = size

            });
            if (differentAxes)
            {
                (serieAsList.Last() as LineSeries<ObservablePoint>).ScalesYAt = indice;
            }
            Series = serieAsList.ToArray();
        }

        [Key(0)]
        public ISeries[] Series { get; set; }
        [Key(3)]
        public RectangularSection[] RectangularSection { get; set; }

        // Creates a gray background and border in the draw margin.
        [Key(1)]
        public DrawMarginFrame DrawMarginFrame => new()
        {

            Fill = new SolidColorPaint(new SKColor(220, 220, 220)),
            Stroke = new SolidColorPaint(new SKColor(180, 180, 180), 1)
        };

        [Key(2)]
        public Axis[] YAxes { get; set; }

    }


    [MessagePackObject]
    public class ChartViewModelScatter
    {
        public ChartJsConfig ChartJsConfig { get; set; }

        public ChartViewModelScatter() { }
        public ChartViewModelScatter(Point[] valeurs)
        {
            Series = new ISeries[0];
            AddSerie(valeurs, null, new SolidColorPaint(SKColors.CornflowerBlue));
            ChartJsConfig = new ChartJsConfig
            {
                Type = ChartType.scatter,
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
                    }
                },
                Data = new ChartJsData
                {
                    Datasets = new List<ChartJsDataset>
                    {
                        new ScatterDataset() {

                            Data= valeurs.Select(a=>new DataPoint{ X=a.X, Y=a.Y }).OfType<object>().ToList()
                        }
                    }
                }
            };
        }
        public ChartViewModelScatter(List<Point[]> valeurs, List<Paint> stroke, List<Paint> fill)
        {
            Update(valeurs, stroke, fill);
        }

        public void Update(List<Point[]> valeurs, List<Paint> stroke, List<Paint> fill)
        {
            Series = new ISeries[0];
            for (int i = 0; i < valeurs.Count; i++)
            {
                AddSerie(valeurs[i], stroke[i], fill[i]);
            }
            if (ChartJsConfig == null)
            {
                ChartJsConfig = new ChartJsConfig
                {
                    Type = ChartType.scatter,
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
                        }
                    },
                    Data = new ChartJsData
                    {
                        Datasets = valeurs.Select(a =>
                            new ScatterDataset()
                            {

                                Data = a.Select(b => new DataPoint { X = b.X, Y = b.Y }).OfType<object>().ToList()
                            }).OfType<ChartJsDataset>().ToList()
                    }
                };
            }
            else
            {
                ChartJsConfig.Data = new ChartJsData
                {
                    Datasets = valeurs.Select(a =>
                        new ScatterDataset()
                        {

                            Data = a.Select(b => new DataPoint { X = b.X, Y = b.Y }).OfType<object>().ToList()
                        }).OfType<ChartJsDataset>().ToList()
                };
            }
        }

        public void AddSerie(Point[] valeurs, Paint? stroke, Paint? fill)
        {
            var serieAsList = Series.ToList();
            serieAsList.Add(new LineSeries<ObservablePoint>
            {
                Values = valeurs.Select(a => new ObservablePoint { X = a.X, Y = a.Y }).ToList(),
                GeometryFill = fill,
                GeometryStroke = stroke,
                GeometrySize = 10,
                Fill = null,
                Stroke = null,
            });
            Series = serieAsList.ToArray();
        }

        [Key(0)]
        public ISeries[] Series { get; set; }

        // Creates a gray background and border in the draw margin.
        [Key(1)]
        public DrawMarginFrame DrawMarginFrame => new()
        {

            Fill = new SolidColorPaint(new SKColor(220, 220, 220)),
            Stroke = new SolidColorPaint(new SKColor(180, 180, 180), 1)
        };
    }
}
