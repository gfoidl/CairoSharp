// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Globalization;
using Cairo;
using Cairo.Drawing.Text;
using Cairo.Extensions;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Colors.ColorMaps;
using Cairo.Fonts;
using Cairo.Surfaces;
using Cairo.Surfaces.Images;
using Cairo.Surfaces.Recording;
using IOPath = System.IO.Path;

if (Directory.Exists("lightness")) Directory.Delete("lightness", true);
Directory.CreateDirectory("lightness");
Environment.CurrentDirectory = IOPath.Combine(Environment.CurrentDirectory, "lightness");

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

foreach (ColorMap colorMap in GetColorMaps())
{
    PlotLightnessOfColorMap(colorMap);
}
//-----------------------------------------------------------------------------
static IEnumerable<ColorMap> GetColorMaps()
{
    foreach (Type type in typeof(ColorMap).Assembly.GetTypes())
    {
        if (type.IsClass && type.IsSubclassOf(typeof(ColorMap)))
        {
            yield return (Activator.CreateInstance(type) as ColorMap)!;
        }
    }
}
//-----------------------------------------------------------------------------
static void PlotLightnessOfColorMap(ColorMap colorMap)
{
    (string name, RecordingSurface recordingSurface) = CreatePlotCore(colorMap);

    try
    {
        Rectangle inkExtents = recordingSurface.GetInkExtents();

        using ImageSurface finalSurface = new(Format.Argb32, 2 * (int)inkExtents.Width, 2 * (int)inkExtents.Height);
        using CairoContext cr           = new(finalSurface);

        cr.Scale(2, 2);
        cr.Antialias = Antialias.Best;

        // White background
        cr.Color = KnownColors.White;
        cr.Paint();

        // Need to set at (-x, -y)
        cr.SetSourceSurface(recordingSurface, -inkExtents.X, -inkExtents.Y);
        cr.Paint();

        finalSurface.WriteToPng($"{name}_lightness.png");
    }
    finally
    {
        recordingSurface.Dispose();
    }
}
//-----------------------------------------------------------------------------
static (string Name, RecordingSurface Surface) CreatePlotCore(ColorMap colorMap)
{
    const int ChartWidthInPoints  = 256;
    const int ChartHeightInPoints = 256;

    string name              = colorMap.GetType().Name.Replace("ColorMap", "");
    RecordingSurface surface = new(Content.ColorAlpha);
    using CairoContext cr    = new(surface);

    SetAxisTitleFont(cr);
    const string YAxisTitle = "Lightness";
    // Will be printed 90° rotated
    PointD yAxisTitlePoint = cr.TextAlignCenter(YAxisTitle, ChartHeightInPoints, ChartWidthInPoints, out TextExtents yAxisTitleExtents);
    double yAxisTitleWidth = yAxisTitleExtents.Height * 1.5;

    SetAxisTickFont(cr);
    string yAxisTick = "100";   // just the longest value
    cr.TextExtents(yAxisTick, out TextExtents yAxisTickExtents);
    double yAxisTickWidth = yAxisTickExtents.Width * 1.2;

    string xAxisTick = "1.0";   // just the longest value
    cr.TextExtents(xAxisTick, out TextExtents xAxisTickExtents);
    double xAxisTickHeight = xAxisTickExtents.Height * 1.2;

    cr.Translate(yAxisTitleWidth + yAxisTickWidth, 0);

    DrawTitle(out TextExtents titleExtents);

    cr.Translate(0, titleExtents.Height * 1.5);

    // Chart area
    cr.LineWidth = 1d;
    cr.Rectangle(0, 0, ChartWidthInPoints, ChartHeightInPoints);
    cr.Stroke();

    DrawAxisWithLabels();

    // Move coordinate system to bottom left
    cr.Translate(0, ChartHeightInPoints);
    // Invert y axis to have natural chart coordinate system
    cr.Scale(1, -1);

    using (cr.Save())
    {
        DrawData();
    }

    return (name, surface);
    //-------------------------------------------------------------------------
    static void SetAxisTitleFont(CairoContext cr)
    {
        cr.SelectFontFace("Sans", weight: FontWeight.Bold);
        cr.SetFontSize(12);
    }
    //-------------------------------------------------------------------------
    static void SetAxisTickFont(CairoContext cr)
    {
        cr.SelectFontFace("Sans");
        cr.SetFontSize(10);
    }
    //-------------------------------------------------------------------------
    void DrawTitle(out TextExtents titleExtents)
    {
        cr.SelectFontFace("Sans", weight: FontWeight.Bold);
        cr.SetFontSize(14);
        string title      = $"L* plot for {name}";
        PointD titlePoint = cr.TextAlignCenter(title, ChartWidthInPoints, ChartHeightInPoints, out titleExtents);
        cr.MoveTo(titlePoint.X, titleExtents.Height);
        cr.ShowText(title);
    }
    //-------------------------------------------------------------------------
    void DrawAxisWithLabels()
    {
        SetAxisTitleFont(cr);

        // x axis title
        using (cr.Save())
        {
            const string XAxisTitle = "color map value";
            PointD xAxisTitlePoint  = cr.TextAlignCenter(XAxisTitle, ChartWidthInPoints, ChartHeightInPoints, out TextExtents xAxisTitleExtents);
            cr.Translate(0, ChartHeightInPoints + xAxisTickHeight + xAxisTitleExtents.Height);
            cr.MoveTo(xAxisTitlePoint.X, 0);
            cr.ShowText(XAxisTitle);
        }

        // y axis title
        using (cr.Save())
        {
            cr.Translate(-yAxisTickWidth, ChartHeightInPoints);
            cr.Rotate(-90.DegreesToRadians());
            cr.MoveTo(yAxisTitlePoint.X, 0);
            cr.ShowText(YAxisTitle);
        }

        SetAxisTickFont(cr);

        // x ticks
        using (cr.Save())
        {
            cr.Translate(0, ChartHeightInPoints + xAxisTickHeight);

            const double Scale = ChartWidthInPoints / 100d;

            for (int i = 0; i <= 100; i += 10)
            {
                using (cr.Save())
                {
                    cr.Translate(i * Scale, 0);

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

                    cr.MoveTo(0, -ChartHeightInPoints);
                    cr.RelLineTo(0, 5);
                    cr.Stroke();

                    // Grid line
                    SetGridLineStyle(cr);
                    cr.MoveTo(0, 0);
                    cr.RelLineTo(0, -ChartHeightInPoints);
                    cr.Stroke();
                }
            }
        }

        // y ticks
        using (cr.Save())
        {
            double xOffset = yAxisTickWidth - yAxisTickExtents.Width;
            cr.Translate(-xOffset, ChartHeightInPoints);

            const double Scale = ChartHeightInPoints / 100d;

            for (int i = 0; i <= 100; i += 10)
            {
                using (cr.Save())
                {
                    cr.Translate(0, -i * Scale);

                    string label = i.ToString();
                    cr.TextExtents(label, out TextExtents labelExtents);
                    cr.MoveTo(-labelExtents.Width, -labelExtents.Height / 2 - labelExtents.YBearing);
                    cr.ShowText(label);

                    // Tick line
                    cr.MoveTo(xOffset, 0);
                    cr.RelLineTo(5, 0);
                    cr.Stroke();

                    cr.MoveTo(xOffset + ChartWidthInPoints, 0);
                    cr.RelLineTo(-5, 0);
                    cr.Stroke();

                    // Grid line
                    SetGridLineStyle(cr);
                    cr.MoveTo(xOffset, 0);
                    cr.RelLineTo(ChartWidthInPoints, 0);
                    cr.Stroke();
                }
            }
        }

        static void SetGridLineStyle(CairoContext cr)
        {
            cr.LineWidth = 0.25;
            cr.Color = KnownColors.Gray;
            cr.SetDash(1, 1);
        }
    }
    //-------------------------------------------------------------------------
    void DrawData()
    {
        const int Steps          = 256;
        const double PointRadius = 2;
        const double XScale      = ChartWidthInPoints  / (double)Steps;
        const double YScale      = ChartHeightInPoints / 100d;      // 100 = max value of CieLab L*

        for (int i = 0; i <= Steps; ++i)
        {
            double value = i / (double)Steps;

            Color color             = colorMap.GetColor(value);
            CieLabColor cieLabColor = color.ToCieLab();
            Debug.Assert(cieLabColor.L is >= 0 and <= 100);

            cr.Color = color;
            cr.Arc(i * XScale, cieLabColor.L * YScale, PointRadius, 0, Math.Tau);
            cr.Fill();
        }
    }
}
