// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Drawing.Text;
using Cairo.Fonts;
using Cairo.Surfaces;
using Cairo.Surfaces.Recording;

namespace Cairo.Extensions.Colors.ColorMaps;

/// <summary>
/// Extension methods for color map analysis.
/// </summary>
public static class ColorMapAnalysisExtensions
{
    extension(ColorMap colorMap)
    {
        /// <summary>
        /// Creates a plot with the lightness characteristics for the <see cref="ColorMap"/>.
        /// </summary>
        /// <param name="chartWidthInPoints">the width of the chart area, defaults to 256</param>
        /// <param name="chartHeightInPoints">the height of the chart area, defaults to 256</param>
        /// <returns>
        /// A <see cref="RecordingSurface"/> containing the plot, which can be replayed on other <see cref="Surface"/>s
        /// </returns>
        /// <remarks>
        /// The size of the chart area is <paramref name="chartWidthInPoints"/> x <paramref name="chartHeightInPoints"/>.
        /// Note: the overall chart is bigger, as title, axis, etc. need to be accounted. Thus you can use
        /// <see cref="RecordingSurface.GetInkExtents"/> to get actual size information.
        /// Usage could look like:
        /// <code>
        /// using RecordingSurface recordingSurface = colorMap.PlotLightnessCharacteristics();
        /// Rectangle inkExtents                    = recordingSurface.GetInkExtents();
        /// using ImageSurface finalSurface         = new(Format.Argb32, (int)inkExtents.Width, (int)inkExtents.Height);
        /// using CairoContext cr                   = new (finalSurface);
        ///
        /// // White background
        /// cr.Color = KnownColors.White;
        /// cr.Paint();
        /// 
        /// // Need to set at (-x, -y)
        /// cr.SetSourceSurface(recordingSurface, -inkExtents.X, -inkExtents.Y);
        /// cr.Paint();
        ///
        /// finalSurface.WriteToPng($"{colorMap.Name}_lightness.png");
        /// </code>
        /// <para>
        /// As example for such a plot see <a href="https://github.com/gfoidl/CairoSharp/blob/main/images/colors/colormaps/gallery/lightness/Turbo_lightness.png">L* plot for Turbo</a>.
        /// </para>
        /// </remarks>
        public RecordingSurface PlotLightnessCharacteristics(int chartWidthInPoints = 256, int chartHeightInPoints = 256)
        {
            string name              = colorMap.Name;
            RecordingSurface surface = new(Content.ColorAlpha);
            using CairoContext cr    = new(surface);

            SetAxisTitleFont(cr);
            const string YAxisTitle = "Lightness";
            // Will be printed 90° rotated
            PointD yAxisTitlePoint = cr.TextAlignCenter(YAxisTitle, chartHeightInPoints, chartWidthInPoints, out TextExtents yAxisTitleExtents);
            double yAxisTitleWidth = yAxisTitleExtents.Height * 1.5;

            SetAxisTickFont(cr);
            cr.TextExtents("100", out TextExtents yAxisTickExtents);    // just the longest value
            double yAxisTickWidth = yAxisTickExtents.Width * 1.2;

            cr.TextExtents("1.0", out TextExtents xAxisTickExtents);    // just the longest value
            double xAxisTickHeight = xAxisTickExtents.Height * 1.2;

            cr.Translate(yAxisTitleWidth + yAxisTickWidth, 0);

            DrawTitle(out TextExtents titleExtents);

            cr.Translate(0, titleExtents.Height * 1.5);

            // Chart area
            cr.LineWidth = 1d;
            cr.Rectangle(0, 0, chartWidthInPoints, chartHeightInPoints);
            cr.Stroke();

            DrawAxisWithLabels();

            // Move coordinate system to bottom left
            cr.Translate(0, chartHeightInPoints);
            // Invert y axis to have natural chart coordinate system
            cr.Scale(1, -1);

            using (cr.Save())
            {
                DrawData();
            }

            return surface;
            //-----------------------------------------------------------------
            static void SetAxisTitleFont(CairoContext cr)
            {
                cr.SelectFontFace("Sans", weight: FontWeight.Bold);
                cr.SetFontSize(12);
            }
            //-----------------------------------------------------------------
            static void SetAxisTickFont(CairoContext cr)
            {
                cr.SelectFontFace("Sans");
                cr.SetFontSize(10);
            }
            //-----------------------------------------------------------------
            void DrawTitle(out TextExtents titleExtents)
            {
                cr.SelectFontFace("Sans", weight: FontWeight.Bold);
                cr.SetFontSize(14);

                string title      = $"L* plot for {name}";
                PointD titlePoint = cr.TextAlignCenter(title, chartWidthInPoints, chartHeightInPoints, out titleExtents);
                cr.MoveTo(titlePoint.X, titleExtents.Height);
                cr.ShowText(title);
            }
            //-----------------------------------------------------------------
            void DrawAxisWithLabels()
            {
                SetAxisTitleFont(cr);

                // x axis title
                using (cr.Save())
                {
                    const string XAxisTitle = "color map value";
                    PointD xAxisTitlePoint  = cr.TextAlignCenter(XAxisTitle, chartWidthInPoints, chartHeightInPoints, out TextExtents xAxisTitleExtents);
                    cr.MoveTo(xAxisTitlePoint.X, chartHeightInPoints + xAxisTickHeight + xAxisTitleExtents.Height);
                    cr.ShowText(XAxisTitle);
                }

                // y axis title
                using (cr.Save())
                {
                    cr.Translate(-yAxisTickWidth, chartHeightInPoints);
                    cr.Rotate(-90.DegreesToRadians());
                    cr.MoveTo(yAxisTitlePoint.X, 0);
                    cr.ShowText(YAxisTitle);
                }

                SetAxisTickFont(cr);

                // x ticks
                using (cr.Save())
                {
                    cr.Translate(0, chartHeightInPoints + xAxisTickHeight);

                    double scale = chartWidthInPoints / 100d;

                    for (int i = 0; i <= 100; i += 10)
                    {
                        using (cr.Save())
                        {
                            cr.Translate(i * scale, 0);

                            double xTick = i * 0.01;
                            string label = xTick.ToString("G1");
                            cr.TextExtents(label, out TextExtents labelExtents);
                            cr.MoveTo(-labelExtents.Width / 2, 0);
                            cr.ShowText(label);

                            // Tick line
                            cr.Translate(0, -xAxisTickExtents.Height * 1.2);
                            cr.MoveTo(0,  0);
                            cr.LineTo(0, -5);
                            cr.Stroke();

                            cr.MoveTo(0, -chartHeightInPoints);
                            cr.RelLineTo(0, 5);
                            cr.Stroke();

                            // Grid line
                            SetGridLineStyle(cr);
                            cr.MoveTo(0, 0);
                            cr.RelLineTo(0, -chartHeightInPoints);
                            cr.Stroke();
                        }
                    }
                }

                // y ticks
                using (cr.Save())
                {
                    double xOffset = yAxisTickWidth - yAxisTickExtents.Width;
                    cr.Translate(-xOffset, chartHeightInPoints);

                    double scale = chartHeightInPoints / 100d;

                    for (int i = 0; i <= 100; i += 10)
                    {
                        using (cr.Save())
                        {
                            cr.Translate(0, -i * scale);

                            string label = i.ToString();
                            cr.TextExtents(label, out TextExtents labelExtents);
                            cr.MoveTo(-labelExtents.Width, -labelExtents.Height / 2 - labelExtents.YBearing);
                            cr.ShowText(label);

                            // Tick line
                            cr.MoveTo(xOffset, 0);
                            cr.RelLineTo(5, 0);
                            cr.Stroke();

                            cr.MoveTo(xOffset + chartWidthInPoints, 0);
                            cr.RelLineTo(-5, 0);
                            cr.Stroke();

                            // Grid line
                            SetGridLineStyle(cr);
                            cr.MoveTo(xOffset, 0);
                            cr.RelLineTo(chartWidthInPoints, 0);
                            cr.Stroke();
                        }
                    }
                }

                static void SetGridLineStyle(CairoContext cr)
                {
                    cr.LineWidth = 0.25;
                    cr.Color     = KnownColors.Gray;

                    cr.SetDash(1, 1);
                }
            }
            //-----------------------------------------------------------------
            void DrawData()
            {
                const int Steps          = 256;
                const double PointRadius = 2;

                double xScale      = chartWidthInPoints  / (double)Steps;
                double yScale      = chartHeightInPoints / 100d;      // 100 = max value of CieLab L*

                for (int i = 0; i <= Steps; ++i)
                {
                    double value = i / (double)Steps;

                    Color color             = colorMap.GetColor(value);
                    CieLabColor cieLabColor = color.ToCieLab();
                    Debug.Assert(cieLabColor.L is >= 0 and <= 100);

                    cr.Color = color;
                    cr.Arc(i * xScale, cieLabColor.L * yScale, PointRadius, 0, Math.Tau);
                    cr.Fill();
                }
            }
        }
    }
}
