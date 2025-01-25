using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using MathNet.Numerics.Statistics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stochastique;

namespace OnlineCalibrator.Shared
{
    public class GenerationGraphique
    {

        public static Point[] GetDensity(double[] valeurs, int nbStep, double? min=null,double? max = null)
        {
            Point[] result = new Point[nbStep];


            double MAX = Double.MinValue, MIN = Double.MaxValue;
            int N = valeurs.Length; // number of data points
            double racineN = Math.Sqrt(N);
            var valOrdered = valeurs.OrderBy(a => a).ToArray();
            if(min==null)
            {
                min = valOrdered[0] - (valOrdered[valOrdered.Length - 1] - valOrdered[0]) / nbStep;
            }
            if(max==null)
            {
                max = valOrdered[valOrdered.Length - 1] + (valOrdered[valOrdered.Length - 1] - valOrdered[0]) / nbStep;
            }


            for (int i = 0; i < nbStep; i++)
            {
                result[i] = new Point { X = (min + (max - min) * i / nbStep).Value, Y = 0 };
            }

            // kernel density estimation
            double bandwidth = Math.Pow((4 * Math.Pow(Statistics.StandardDeviation(valOrdered), 5) / (3 * valOrdered.Length)), 0.2);
            int indDeb = 0;

            for (int i = 0; i < nbStep; i++)
            {
                int j = indDeb;
                bool continuer = true;
                while (j < valOrdered.Length && continuer)
                {
                    var kernel = GaussianKernel((result[i].X - valOrdered[j]) / bandwidth);
                    if (j == indDeb && kernel < 1e-8)
                    {
                        indDeb++;
                    }
                    else if (j != indDeb && kernel < 1e-8)
                    {
                        continuer = false;
                    }
                    else
                    {
                        result[i].Y += kernel / racineN;
                    }
                    j++;
                }

            }
            return result;
        }
        public static Point[] GetRank(List<List<double>> valeurs)
        {
            Point[] result = new Point[valeurs[0].Count];
            for (int i = 0; i < valeurs[0].Count; i++)
            {
                result[i] = new Point { X = valeurs[0][i], Y = valeurs[1][i] };
            }
            return result;
        }

        public static Point[] GetCDF(double[] valeurs)
        {
            Point[] result = new Point[valeurs.Length];
            var valOrdered = valeurs.OrderBy(a => a).ToArray();
            for (int i = 0; i < valOrdered.Length; i++)
            {
                result[i] = new Point { X = valOrdered[i], Y = (double)(i + 1) / valOrdered.Length };
            }
            return result;
        }



        public static double GaussianKernel(double x)
        {
            return 1.0 / Math.Sqrt(2 * Math.PI) * Math.Exp(-x * x / 2);
        }

        public static void SaveChartImage(Point[] points, string name, int width = 2000, int height = 1500, bool showAxis = false)
        {

            var skChart = new SKCartesianChart()
            {
                Series = new List<ISeries>
                {
                    new LineSeries<ObservablePoint>
                    {
                        Values = points.Select(a => new ObservablePoint(a.X, a.Y)).ToList(),
                        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4, Color= SKColors.Blue  },

                        GeometrySize = 0
                    }
                },
                Width = width,
                Height = height,
            };
            if (!showAxis)
            {
                skChart.XAxes.First().ShowSeparatorLines = false;
                skChart.YAxes.First().ShowSeparatorLines = false;
                skChart.XAxes.First().Labeler = a => "";
                skChart.YAxes.First().Labeler = a => "";
            }
            skChart.SaveImage($"{name}.png");
        }
        public static void SaveChartImage(List<Point[]> points, List<SolidColorPaint> fill, List<SolidColorPaint> content, List<SolidColorPaint> geometryFill, List<int> geometrySize, string name, int width = 2000, int height = 1500, bool showAxis = false)
        {

            var skChart = new SKCartesianChart()
            {
                Series = points.Select((a, i) =>
                    new LineSeries<ObservablePoint>
                    {
                        Values = a.Select(x => new ObservablePoint(x.X, x.Y)).ToList(),
                        Stroke = content[i],
                        Fill = fill[i],
                        GeometrySize = geometrySize[i],
                        GeometryFill = geometryFill[i],
                        LineSmoothness = 0,
                    }
                ),
                Width = width,
                Height = height,

            };
            if (!showAxis)
            {
                skChart.XAxes.First().ShowSeparatorLines = false;
                skChart.YAxes.First().ShowSeparatorLines = false;
                skChart.XAxes.First().Labeler = a => "";
                skChart.YAxes.First().Labeler = a => "";
            }
            else
            {
                skChart.DrawMarginFrame = new DrawMarginFrame
                {

                    Fill = new SolidColorPaint(new SKColor(220, 220, 220)),
                    Stroke = new SolidColorPaint(new SKColor(180, 180, 180), 1)
                };
            }
            skChart.SaveImage($"{name}.png");

        }

        public static void SaveChartImage(List<Point[]> pointsArray, string name, int width = 2000, int height = 1500, bool showAxis = false)
        {
            var series = new List<ISeries>();
            int i = 0;
            foreach (var points in pointsArray)
            {
                var skcol = SKColors.Blue;
                switch (i % 4)
                {
                    case 0:
                        skcol = SKColors.Blue;
                        break;
                    case 1:
                        skcol = SKColors.Red;
                        break;
                    case 2:
                        skcol = SKColors.Green;
                        break;
                    case 3:
                        skcol = SKColors.Orange;
                        break;
                }

                series.Add(new LineSeries<ObservablePoint>
                {
                    Values = points.Select(a => new ObservablePoint(a.X, a.Y)).ToList(),
                    Stroke = new SolidColorPaint(skcol) { StrokeThickness = 2, Color = skcol},
                    Fill = null,
                    GeometrySize = 0
                });
                i++;

            }
            var skChart = new SKCartesianChart()
            {
                Series = series,
                Width = width,
                Height = height,

            };

            if (!showAxis)
            {
                skChart.XAxes.First().ShowSeparatorLines = false;
                skChart.YAxes.First().ShowSeparatorLines = false;
                skChart.XAxes.First().Labeler = a => "";
                skChart.YAxes.First().Labeler = a => "";
            }
            skChart.SaveImage($"{name}.png");
        }

    }
}
