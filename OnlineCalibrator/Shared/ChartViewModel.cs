using GenerationImageDistribution;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public class ChartViewModelLine
    {
        public ChartViewModelLine(Point[] valeurs)
        {
            Series = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = valeurs.Select(a=>new ObservablePoint{ X = a.X, Y=a.Y}),
                    Fill = new SolidColorPaint(SKColors.CornflowerBlue), // mark
                    Stroke = null,
                    GeometryFill = null,
                    GeometryStroke = null
                }
            };
        }
        public ISeries[] Series { get; set; } 

        // Creates a gray background and border in the draw margin.
        public DrawMarginFrame DrawMarginFrame => new()
        {
            
            Fill = new SolidColorPaint(new SKColor(220, 220, 220)),
            Stroke = new SolidColorPaint(new SKColor(180, 180, 180), 1)
        };
    }
}
