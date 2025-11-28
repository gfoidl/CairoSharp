// (c) gfoidl, all rights reserved

//#define SHOW_BOX_MARKERS

extern alias CairoSharp;

using Cairo.Extensions.Colors;
using Cairo.Extensions.Colors.ColorMaps;
using Cairo.Extensions.Fonts;
using Cairo.Extensions.Pixels;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Fonts;
using CairoSharp::Cairo.Surfaces;
using CairoSharp::Cairo.Surfaces.Images;
using Gtk4.Extensions;

namespace Gtk4FunctionPlotDemo;

internal static class Plotter
{
    public static void CreateFunctionSurface<TColorMap>(ImageSurface surface, double[][] funcData, double funcMin, double funcMax)
        where TColorMap : ColorMap, new()
    {
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
    }
    //-------------------------------------------------------------------------
    public static void DrawCrosshairs(CairoContext cr, MousePosition mousePosition, int width, int height)
    {
        using (cr.Save())
        {
            cr.Color     = Color.Default;
            cr.LineWidth = 1;
            cr.SetDash(10, 5, 2, 5);    // 10 long, 5 off, 2 long, 5 off and repeat

            cr.Translate(mousePosition.X, mousePosition.Y);

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
    public static void DrawCurrentValue(
        CairoContext  cr,
        int           width,
        int           height,
        MousePosition mousePosition,
        double        funcX,
        double        funcY,
        double        funcZ)
    {
#if DEBUG
        Console.WriteLine($"mouse at ({mousePosition.X:N3}, {mousePosition.Y:N3})\tf({funcX:N3}, {funcY:N3}) = {funcZ:N3}");
#endif
        //cr.SelectFontFace("@cairo:monospace", weight: FontWeight.Bold);
        cr.FontFace = DefaultFonts.MonoSpace;
        cr.SetFontSize(16);

        string textForExtents = $"_(x, y) = ({funcX:N2}, {funcY:N2})";  // _ as space doesn't count in TextExtents
        string text0          = textForExtents.Replace('_', ' ');
        string text1          = $"f(x, y) = {funcZ:N2}";

        cr.TextExtents(textForExtents, out TextExtents textExtents);

        const double Padding =  5;
        const double Offset  = 20;

        using (cr.Save())
        {
            cr.Translate(mousePosition.X, mousePosition.Y);

            double annotationWidth  = textExtents.Width;
            double annotationHeight = textExtents.Height * 2;   // 2 lines

            double tx = mousePosition.X + Offset + annotationWidth < width
                ? Offset
                : -(Offset + annotationWidth);

            double ty = mousePosition.Y + Offset + annotationHeight < height
                ? Offset
                : -(Offset + annotationHeight);

            cr.Translate(tx, ty);

            double boxWidth  = annotationWidth + 2 * Padding;
            double boxHeight = annotationHeight + 2 * Padding;

            cr.MoveTo(-Padding, -Padding);
            cr.RelLineTo( boxWidth, 0);
            cr.RelLineTo(        0, boxHeight);
            cr.RelLineTo(-boxWidth, 0);
            cr.ClosePath();

            cr.Color = KnownColors.White;
            cr.FillPreserve();
            cr.Color = Color.Default;
            cr.Stroke();

            cr.MoveTo(0, 1 * textExtents.Height);
            cr.ShowText(text0);
            cr.MoveTo(0, 2 * textExtents.Height);
            cr.ShowText(text1);

#if SHOW_BOX_MARKERS
            cr.Color = KnownColors.Red;
            cr.Arc(0, 0, Padding, 0, Math.Tau);
            cr.Fill();

            cr.Color = KnownColors.Blue;
            cr.Arc(textExtents.Width, 0, Padding, 0, Math.Tau);
            cr.Fill();

            cr.Color = KnownColors.Green;
            cr.Arc(0, 2 * textExtents.Height, Padding, 0, Math.Tau);
            cr.Fill();

            cr.Color = KnownColors.Yellow;
            cr.Arc(textExtents.Width, 2 * textExtents.Height, Padding, 0, Math.Tau);
            cr.Fill();
#endif
        }
    }
}
