using GenerationImageDistribution;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MathNet.Numerics;
using MessagePack;
using SkiaSharp;
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
            AddSerie(valeurs, null, new SolidColorPaint(SKColors.CornflowerBlue));
        }
        public ChartViewModelLine(List<Point[]> valeurs, List<Paint> stroke,List<Paint> fill)
        {
            Series = new ISeries[0];
            for (int i=0;i<valeurs.Count;i++)
            {
                AddSerie(valeurs[i], stroke[i], fill[i]);
            }
        }

        public void AddSerie(Point[] valeurs, Paint? stroke, Paint? fill)
        {
            var serieAsList=Series.ToList();
            serieAsList.Add(new LineSeries<ObservablePoint>
            {
                Values = valeurs.Select(a => new ObservablePoint { X = a.X, Y = a.Y }),
                Fill = fill, // mark
                Stroke = stroke,
                GeometryFill = null,
                GeometryStroke = null
            });
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
    }
}
