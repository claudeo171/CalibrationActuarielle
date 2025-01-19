using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Painting;
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

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public class ChartViewModelLine
    {
        public ChartViewModelLine(Point[] valeurs)
        {
            Series = new ISeries[0];
            AddSerie(valeurs, null, new SolidColorPaint(SKColors.CornflowerBlue), 0, 0, null, false);
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
            AddRectangularSection();
            return this;
        }

        public void AddRectangularSection()
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
        public ChartViewModelScatter() { }
        public ChartViewModelScatter(Point[] valeurs)
        {
            Series = new ISeries[0];
            AddSerie(valeurs, null, new SolidColorPaint(SKColors.CornflowerBlue));
        }
        public ChartViewModelScatter(List<Point[]> valeurs, List<Paint> stroke, List<Paint> fill)
        {
            Series = new ISeries[0];
            for (int i = 0; i < valeurs.Count; i++)
            {
                AddSerie(valeurs[i], stroke[i], fill[i]);
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
