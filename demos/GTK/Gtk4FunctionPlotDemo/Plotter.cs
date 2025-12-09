// (c) gfoidl, all rights reserved

#define USE_BYTE_SPANS_FOR_TEXT
#define SHOW_BOX_MARKERS

extern alias CairoSharp;

using System.Diagnostics;
using System.Text;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Colors.ColorMaps;
using Cairo.Extensions.Fonts;
using Cairo.Extensions.Pixels;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Drawing;
using CairoSharp::Cairo.Fonts;
using CairoSharp::Cairo.Surfaces;
using CairoSharp::Cairo.Surfaces.Images;
using Gtk4.Extensions;

namespace Gtk4FunctionPlotDemo;

internal static class Plotter
{
    private const double AnnotationPadding =  5;
    private const double AnnotationOffset  = 20;
    //-------------------------------------------------------------------------
    public static void CreateFunctionSurface<TColorMap>(ImageSurface surface, double[][] funcData, double funcMin, double funcMax)
        where TColorMap : ColorMap, new()
    {
        ColorMap colorMap = new TColorMap();
        double invScale   = 1d / (funcMax - funcMin);

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

                Color color         = colorMap.GetColor(value);
                pixelRowAccessor[x] = color;

#if DEBUG
                Color actual = pixelRowAccessor[x];
                Debug.Assert(Math.Abs(actual.Red   - color.Red)   < 1e-2);
                Debug.Assert(Math.Abs(actual.Green - color.Green) < 1e-2);
                Debug.Assert(Math.Abs(actual.Blue  - color.Blue)  < 1e-2);
                Debug.Assert(Math.Abs(actual.Alpha - color.Alpha) < 1e-2);
#endif
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
        double        funcZ,
        bool          darkColorScheme,
        bool          useMonospaceFont)
    {
#if DEBUG
        Console.WriteLine($"mouse at ({mousePosition.X:N3}, {mousePosition.Y:N3})\tf({funcX:N3}, {funcY:N3}) = {funcZ:N3}");
#endif
        //cr.SelectFontFace("@cairo:monospace", weight: FontWeight.Bold);
        cr.FontFace = useMonospaceFont ? DefaultFonts.MonoSpace : DefaultFonts.SansSerif;
        cr.SetFontSize(16);

#if !USE_BYTE_SPANS_FOR_TEXT
        string textForExtents = $"_(x, y) = ({funcX:N2}, {funcY:N2})";                  // _ as space doesn't count in TextExtents
#else
        Span<char> textBuffer     = stackalloc char[64];
        Span<byte> textForExtents = stackalloc byte[128];
        Span<byte> text0          = textForExtents;
        Span<byte> text1          = textForExtents.Slice(64, 64);

        textBuffer.TryWrite($"_(x, y) = ({funcX:N2}, {funcY:N2})", out int written);    // _ as space doesn't count in TextExtents
        written                       = Encoding.UTF8.GetBytes(textBuffer[..written], textForExtents);
        textForExtents                = textForExtents[..written];
#endif

        cr.FontExtents(out FontExtents fontExtents);
        cr.TextExtents(textForExtents, out TextExtents textExtents);

        using (cr.Save())
        {
            cr.Translate(mousePosition.X, mousePosition.Y);

            // See ReadMe.md in project root for a figure for the values.
            double yBearing         = Math.Abs(textExtents.YBearing);   // YBearing is negative, as defined by cairo's coordinate system
            double annotationWidth  = textExtents.Width;
            double annotationHeight = yBearing + fontExtents.Height + (textExtents.Height - yBearing);

            double tx = mousePosition.X + AnnotationOffset + annotationWidth + AnnotationPadding < width
                ? AnnotationOffset
                : -(AnnotationOffset + annotationWidth);

            double ty = mousePosition.Y + AnnotationOffset + annotationHeight + AnnotationPadding < height
                ? AnnotationOffset
                : -(AnnotationOffset + annotationHeight);

            cr.Translate(tx, ty);

            DrawBoundingBox(cr, annotationWidth, annotationHeight, tx, ty, darkColorScheme);

#if !USE_BYTE_SPANS_FOR_TEXT
            string text0 = textForExtents.Replace('_', ' ');
            string text1 = $"f(x, y) = {funcZ:N2}";
#else
            text0[0] = (byte)' ';

            textBuffer.TryWrite($"f(x, y) = {funcZ:N2}", out written);
            written                       = Encoding.UTF8.GetBytes(textBuffer[..written], text1);
            text1                         = text1[..written];
#endif

            cr.Color = !darkColorScheme ? Color.Default : KnownColors.White;
            cr.MoveTo(0, yBearing);
            cr.ShowText(text0);
            cr.MoveTo(0, yBearing + fontExtents.Height);
            cr.ShowText(text1);
        }
    }
    //-------------------------------------------------------------------------
    private static void DrawBoundingBox(
        CairoContext cr,
        double       annotationWidth,
        double       annotationHeight,
        double       tx,
        double       ty,
        bool         darkColorScheme)
    {
        double boxWidth  = annotationWidth  + 2 * AnnotationPadding;
        double boxHeight = annotationHeight + 2 * AnnotationPadding;

        const double ArrowWidth = 3 * AnnotationPadding;

        if (tx < 0 && ty > 0)       // left, below
        {
            // Manually drawing the path which is filled and stroked
            using (cr.Save())
            {
                cr.Translate(-tx, -ty);     // mouse position
                cr.MoveTo(0, 0);
                cr.LineTo(-AnnotationOffset + AnnotationPadding - ArrowWidth, AnnotationOffset - AnnotationPadding);
                cr.RelLineTo(-(boxWidth - ArrowWidth), 0);
                cr.RelLineTo(0, boxHeight);
                cr.RelLineTo(boxWidth, 0);
                cr.RelLineTo(0, -(boxHeight - ArrowWidth));
                cr.ClosePath();
            }
        }
        else if (tx > 0 && ty > 0)  // right, below
        {
            // Using cairo's compositing operators.
            // We need an "isolated" drawing environment for this, thus use PushGroup.
            cr.PushGroup();
            {
                cr.Rectangle(-AnnotationPadding, -AnnotationPadding, boxWidth, boxHeight);
                Draw(cr, darkColorScheme);

                cr.PushGroup();                 // for the arrow
                {
                    cr.MoveTo(-tx, -ty);        // mouse position
                    cr.LineTo(-AnnotationPadding, -AnnotationPadding + ArrowWidth);
                    cr.LineTo(-AnnotationPadding + ArrowWidth, -AnnotationPadding);
                    cr.ClosePath();
                    Draw(cr, darkColorScheme);
                }
                cr.PopGroupToSource();

                // Cf. https://www.cairographics.org/operators/
                // Both variants result here in the same image, but the chosen one seems more
                // correct to me.
                //cr.Operator = !darkColorScheme ? Operator.Add : Operator.Multiply;
                cr.Operator = !darkColorScheme ? Operator.Lighten : Operator.Darken;
                cr.Paint();
            }
            cr.PopGroupToSource();
            cr.Paint();

            goto Exit;
        }
        else
        {
            cr.Rectangle(-AnnotationPadding, -AnnotationPadding, boxWidth, boxHeight);
        }

        Draw(cr, darkColorScheme);

        static void Draw(CairoContext cr, bool darkColorScheme)
        {
            Color background, foreground;

            if (!darkColorScheme)
            {
                background = KnownColors.White;
                foreground = Color.Default;
            }
            else
            {
                background = Color.Default;
                foreground = KnownColors.White;
            }

            cr.Color = background;
            cr.FillPreserve();

            cr.Color = foreground;
            cr.Stroke();
        }

    Exit:
#if SHOW_BOX_MARKERS
        const double BoxMarkerRadius = 5;

        cr.Color = KnownColors.Red;
        cr.Arc(0, 0, BoxMarkerRadius, 0, Math.Tau);
        cr.Fill();

        cr.Color = KnownColors.Blue;
        cr.Arc(annotationWidth, 0, BoxMarkerRadius, 0, Math.Tau);
        cr.Fill();

        cr.Color = KnownColors.Green;
        cr.Arc(0, annotationHeight, BoxMarkerRadius, 0, Math.Tau);
        cr.Fill();

        cr.Color = KnownColors.Yellow;
        cr.Arc(annotationWidth, annotationHeight, BoxMarkerRadius, 0, Math.Tau);
        cr.Fill();
#endif
        return;     // just to make the compiler happy when SHOW_BOX_MARKERS isn't defined
    }
}
