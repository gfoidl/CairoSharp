// (c) gfoidl, all rights reserved

extern alias CairoSharp;

using Cairo.Extensions.Colors;
using Cairo.Extensions.Colors.ColorMaps;
using Cairo.Extensions.Pixels;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Fonts;
using CairoSharp::Cairo.Surfaces;
using CairoSharp::Cairo.Surfaces.Images;
using Gtk4.Extensions;

namespace Gtk4FunctionPlotDemo;

internal static class Plotter
{
    private const int Size = MainWindow.Size;
    //-------------------------------------------------------------------------
    public static ImageSurface CreateFunctionSurface<TColorMap>(double[][] funcData, double funcMin, double funcMax)
        where TColorMap : ColorMap, new()
    {
        ImageSurface surface = new(Format.Argb32, Size, Size);
        ColorMap colorMap    = new TColorMap();
        double invScale      = 1d / (funcMax - funcMin);

        surface.Flush();

        Parallel.For(0, funcData.Length, y =>
        {
            double[] data_y                   = funcData[y];
            PixelRowAccessor pixelRowAccessor = surface.GetPixelRowAccessor(y);

            for (int x = 0; x < data_y.Length; ++x)
            {
                double value = data_y[x];
                value       -= funcMin;
                value       *= invScale;

                pixelRowAccessor[x] = colorMap.GetColor(value);
            }
        });

        surface.MarkDirty();

        return surface;
    }
    //-------------------------------------------------------------------------
    public static void DrawCrosshairs(CairoContext cr, DevicePosition devicePosition, int width, int height)
    {
        using (cr.Save())
        {
            cr.Color     = Color.Default;
            cr.LineWidth = 1;
            cr.SetDash(10, 5, 2, 5);    // 10 long, 5 off, 2 long, 5 off and repeat

            cr.Translate(devicePosition.X, devicePosition.Y);

            cr.MoveTo(0, 0);
            cr.LineTo(width, 0);

            cr.MoveTo(0, 0);
            cr.LineTo(-width, 0);

            cr.MoveTo(0, 0);
            cr.LineTo(0, height);

            cr.MoveTo(0, 0);
            cr.LineTo(0, -height);

            cr.Stroke();
        }
    }
    //-------------------------------------------------------------------------
    public static void DrawCurrentValue(CairoContext cr, int width, int height, DevicePosition devicePosition, double funcX, double funcY, double funcZ)
    {
#if DEBUG
        Console.WriteLine($"mouse at ({devicePosition.X:N3}, {devicePosition.Y:N3})\tf({funcX:N3}, {funcY:N3}) = {funcZ:N3}");
#endif

        cr.SelectFontFace("Helvetica");
        cr.SetFontSize(16);

        string text0 = $"(x, y) = ({funcX:N3}, {funcY:N3})";
        string text1 = $"f(x, y) = {funcZ:N3}";

        cr.TextExtents(text0, out TextExtents textExtents);

        const double Padding = 5;
        const double Offset  = 20;

        using (cr.Save())
        {
            cr.Translate(devicePosition.X, devicePosition.Y);
            cr.Translate(Offset, Offset);

            cr.MoveTo(-Padding, -Padding);
            cr.RelLineTo( textExtents.Width + 2 * Padding, 0);
            cr.RelLineTo(                               0, 2 * textExtents.Height + 2 * Padding);
            cr.RelLineTo(-textExtents.Width - 2 * Padding, 0);
            cr.ClosePath();

            cr.Color = Color.Default;
            cr.Fill();

            cr.Color = KnownColors.White;

            cr.MoveTo(0, 1 * textExtents.Height);
            cr.ShowText(text0);
            cr.MoveTo(0, 2 * textExtents.Height);
            cr.ShowText(text1);
        }
    }
}
