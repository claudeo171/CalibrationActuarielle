using LiveChartsCore;
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

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public class ChartViewModelLine
    {
        public ChartViewModelLine(Point[] valeurs)
        {
            Series = new ISeries[0];
            AddSerie(valeurs, null, new SolidColorPaint(SKColors.CornflowerBlue),0,0,null,false);
        }
        public ChartViewModelLine(List<Point[]> valeurs, List<Paint> stroke,List<Paint> fill,List<double> size,List<Paint> color,bool differentAxes = false)
        {
            Series = new ISeries[0];
            if(differentAxes)
            {
                YAxes = new Axis[valeurs.Count];
            }
            for (int i=0;i<valeurs.Count;i++)
            {
                AddSerie(valeurs[i], stroke[i], fill[i], i, size[i], color[i],differentAxes);
                if (differentAxes)
                {
                    YAxes[i] = new Axis();
                }
            }
        }

        public void AddSerie(Point[] valeurs, Paint? stroke, Paint? fill, int indice, double size, Paint color,bool differentAxes)
        {
            var serieAsList=Series.ToList();
            serieAsList.Add(new LineSeries<ObservablePoint>
            {
                Values = valeurs.Select(a => new ObservablePoint { X = a.X, Y = a.Y }),
                Fill = fill, // mark
                Stroke = stroke,
                GeometryFill = color,
                GeometryStroke = color,
                GeometrySize = size
               
            });
            if (differentAxes )
            {
                (serieAsList.Last() as LineSeries<ObservablePoint>).ScalesYAt = indice;
            }
            Series= serieAsList.ToArray();
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

        [Key(2)]
        public Axis[] YAxes { get; set; }

    }

    [MessagePackObject]
    public class ChartViewModelScatter
    {
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
                Values = valeurs.Select(a => new ObservablePoint { X = a.X, Y = a.Y }),
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
