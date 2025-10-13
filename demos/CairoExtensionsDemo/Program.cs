// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Reflection;
using Cairo;
using Cairo.Drawing.Patterns;
using Cairo.Extensions;
using Cairo.Extensions.Arrows;
using Cairo.Extensions.Shapes;
using Cairo.Fonts;
using Cairo.Surfaces;
using Cairo.Surfaces.Images;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.PostScript;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Tee;
using IOPath = System.IO.Path;

AppContext.SetSwitch("Cairo.DebugDispose", true);

if (Directory.Exists("output")) Directory.Delete("output", true);
Directory.CreateDirectory("output");
Environment.CurrentDirectory = IOPath.Combine(Environment.CurrentDirectory, "output");

try
{
    PrintCairoInfo();

    Shapes();
    KnownColorsView();
    Arrows();
    PaintAfter();       // "layered painting"
}
catch (Exception ex) when (!Debugger.IsAttached)
{
    Console.WriteLine(ex);
}

if (Debugger.IsAttached)
{
    Console.WriteLine("\nEnd.");
    Console.ReadKey();
}
//-----------------------------------------------------------------------------
static void PrintCairoInfo()
{
    Console.WriteLine($"Cairo version: {CairoAPI.VersionString}");
    Console.WriteLine();
}
//-----------------------------------------------------------------------------
static void Shapes()
{
    const int Width  = 1050;
    const int Height = 600;

    using SvgSurface svg     = new("shapes.svg", Width, Height);
    using PdfSurface pdf     = new("shapes.pdf", Width, Height);
    using TeeSurface surface = new(svg);

    surface.Add(pdf);
    using CairoContext cr = new(surface);

    // Is only relevant to PNG
    cr.Antialias = Antialias.Best;

    cr.Rectangle(0, 0, Width, Height);
    cr.Stroke();

    // Use shapes explicit
    using (cr.Save())
    {
        using Circle circle   = new(cr, 100);
        using Square square   = new(cr, 100);
        using Hexagon hexagon = new(cr, 100);

        circle .Fill(150, 150, KnownColors.Coral);
        square .Draw(400, 150, KnownColors.Pink);
        hexagon.Draw(650, 150, KnownColors.Brown);

        circle .Fill(900, 150, KnownColors.Coral);
        square .Draw(900, 150, KnownColors.Pink);
        hexagon.Draw(900, 150, KnownColors.Brown);

        cr.LineWidth = 1;
        cr.SetDash(10, 2, 2, 2);
        cr.Color = KnownColors.Blue;
        cr.Arc(900, 150, hexagon.Circumradius, 0, Math.Tau);
        cr.Stroke();
    }

    // Use shapes via extension method -- one-off consumption
    using (cr.Save())
    {
        cr.Circle(150, 400, 100);
        cr.Color = KnownColors.Coral;
        cr.Fill();

        cr.Square(400, 400, 100);
        cr.Color = KnownColors.Pink;
        cr.Stroke();

        cr.Hexagon(650, 400, 100);
        cr.Color = KnownColors.Brown;
        cr.Stroke();

        cr.Hexagon(900, 400, 100, peakOnTop: false);
        cr.Stroke();
    }

    // PNG can also be created this way
    surface.WriteToPng("shapes.png");
}
//-----------------------------------------------------------------------------
static void KnownColorsView()
{
    const int RectWidth  = 200;
    const int RectHeight = 20;
    const int Space      = 5;

    // sorted per HSV colorspace
    List<(Color Color, string Name)> colors = [.. GetKnownColors()
        .Select (c => (Color: c, HSV: c.Color.ToHSV()))
        .OrderBy(cc => cc.HSV.Hue)
        .ThenBy (cc => cc.HSV.Saturation)
        .ThenBy (cc => cc.HSV.Value)
        .Select (cc => cc.Color)
    ];

    const int SurfaceWidth = RectWidth + 2 * Space;
    int surfaceHeight      = Space + (RectHeight + Space) * colors.Count;

    using SvgSurface svg     = new("known_colors.svg", SurfaceWidth, surfaceHeight);
    using PdfSurface pdf     = new("known_colors.pdf", SurfaceWidth, surfaceHeight);
    using TeeSurface surface = new(svg);

    surface.Add(pdf);
    using CairoContext cr = new(surface);

    using (cr.Save())
    {
        cr.Hairline = true;
        cr.Rectangle(0, 0, SurfaceWidth, surfaceHeight);
        cr.Stroke();
    }

    cr.LineWidth = 0.35;
    cr.SetFontSize(10);     // is the default anyway
    cr.SelectFontFace("Arial");

    for (int i = 0; i < colors.Count; ++i)
    {
        int y = Space + i * (RectHeight + Space);

        cr.Rectangle(Space, y, RectWidth, RectHeight);
        cr.Color = colors[i].Color;
        cr.FillPreserve();

        cr.Color = Color.Default;
        cr.Stroke();

        string name = colors[i].Name;

        PointD textStart = cr.TextAlignCenter(name, SurfaceWidth, RectHeight);
        cr.MoveTo(textStart.X, y + textStart.Y);

        cr.Color = colors[i].Color.GetInverseColor();
        cr.ShowText(name);
    }

    surface.WriteToPng("known_colors.png");

    static IEnumerable<(Color Color, string Name)> GetKnownColors()
    {
        IEnumerable<FieldInfo> fields = typeof(KnownColors)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .OrderBy(f => f.Name);

        foreach (FieldInfo field in fields)
        {
            Color color = (Color)field.GetValue(null)!;
            yield return (color, field.Name);
        }
    }
}
//-----------------------------------------------------------------------------
static void Arrows()
{
    using SvgSurface svg       = new("arrows.svg", 300, 300);
    using PdfSurface pdf       = new("arrows.pdf", 300, 300);
    using PostScriptSurface ps = new("arrows.ps" , 300, 300);
    using TeeSurface surface   = new(svg);

    surface.Add(pdf);
    surface.Add(ps);
    using CairoContext cr = new(surface);

    // Is only relevant to PNG
    cr.Antialias = Antialias.Best;

    cr.Scale(300, 300);

    // adjust the line width due scaling
    double ux = 1, uy = 1;
    cr.DeviceToUserDistance(ref ux, ref uy);
    cr.LineWidth = Math.Max(ux, uy);

    cr.MoveTo(0, 0.1);
    cr.LineTo(1, 0.1);
    cr.MoveTo(0, 0.9);
    cr.LineTo(1, 0.9);
    cr.Stroke();

    Arrow arrow = new(cr);
    cr.Color    = KnownColors.Blue;
    arrow.DrawArrow (0.1, 0.1, 0.2, 0.9);
    arrow.DrawVector(0.2, 0.1, 0.3, 0.9);

    arrow    = new OpenArrow(cr);
    cr.Color = KnownColors.Green;
    arrow.DrawArrow (0.3, 0.1, 0.4, 0.9);
    arrow.DrawVector(0.4, 0.1, 0.5, 0.9);

    arrow    = new CircleArrow(cr, radius: 0.01);   // keep scale in mind
    cr.Color = KnownColors.Red;
    arrow.DrawArrow (0.5, 0.1, 0.6, 0.9);
    arrow.DrawVector(0.6, 0.1, 0.7, 0.9);

    svg.WriteToPng("arrows.png");
}
//-----------------------------------------------------------------------------
static void PaintAfter()
{
    Pattern? pattern = null;

    try
    {
        using (SvgSurface svg  = new("test.svg", 300, 300))
        using (CairoContext cr = new(svg))
        {
            // Draw to group
            cr.PushGroup();
            {
                cr.Hexagon(150, 150, 125);
                cr.SetSourceRgb(0.8, 0.8, 0.8);
                cr.Fill();
            }
            // Get the group
            pattern = cr.PopGroup();

            cr.SetSource(pattern);

            // Draw (withou mask, hence everything that's in the in the source)
            cr.Paint();
        }

        using (PdfSurface pdf  = new("test1.pdf", 300, 300))
        using (SvgSurface svg  = new("test1.svg", 300, 300))
        using (CairoContext cr = new(svg))
        {
            cr.PushGroup();
            {
                cr.SetSource(pattern);
                cr.Paint();

                cr.SetSourceRgb(0, 0, 1);
                cr.LineWidth = 0.1;

                cr.MoveTo(  0, 150);
                cr.LineTo(300, 150);
                cr.MoveTo(150,   0);
                cr.LineTo(150, 300);
                cr.Stroke();

                cr.SetSourceRgb(0, 0, 0);
                cr.SelectFontFace("Georgia");   // or "Rockwell", or "Arial", or ...
                cr.SetFontSize(16);
                string text = "Hexagon with coordinate-axis";

                // Determine the width of the text
                double textWidth = cr.GetTextWidth(text);

                using (cr.Save())
                {
                    cr.Translate((300 - textWidth) / 2, 16);
                    cr.ShowText(text);
                }

                cr.MoveTo(  0,   0);
                cr.LineTo(300, 300);
                cr.MoveTo(  0, 300);
                cr.LineTo(300,   0);
                cr.Stroke();

                using (cr.Save())
                {
#if !VARIANT
                    cr.Translate(16, 300);      // see below in the #ifdef, there's 0 --> result is the same.
                    cr.Rotate(-Math.PI / 2);
                    cr.Translate((300 - textWidth) / 2, 0);
#else
                    cr.Translate(0, 300);
                    cr.Rotate(-Math.PI / 2);
                    cr.Translate((300 - textWidth) / 2, 16);
#endif
                    cr.ShowText(text);
                }

                // Draw scale
                using (cr.Save())
                {
                    cr.Translate(270, 20);

                    using LinearGradient linpat = new(0, 0, 0, 260);
                    linpat.AddColorStopRgb(0  , 0, 1, 0);
                    linpat.AddColorStopRgb(0.5, 0, 0, 1);
                    linpat.AddColorStopRgb(1  , 1, 0, 0);

                    cr.Rectangle(0, 0, 20, 260);
                    cr.SetSource(linpat);
                    cr.FillPreserve();

                    cr.Color = Color.Default;
                    cr.LineWidth = 1;
                    cr.Stroke();
                }
            }
            pattern = cr.PopGroup();

            cr.SetSource(pattern);
            cr.Paint();

            using CairoContext cr1 = new(pdf);
            cr1.Color = new Color(1, 1, 1, 0);
            cr1.Rectangle(0, 0, 300, 300);
            cr1.Fill();

            cr1.SetSource(pattern);
            cr1.Paint();

            pdf.WriteToPng("test1.png");
        }
    }
    finally
    {
        pattern?.Dispose();
    }
}
