// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Reflection;
using Cairo;
using Cairo.Extensions;
using Cairo.Extensions.Shapes;
using Cairo.Fonts;
using Cairo.Surfaces.PDF;
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

    cr.Antialias = Antialias.Best;

    cr.Rectangle(0, 0, Width, Height);
    cr.Stroke();

    // Use shapes explicit
    using (cr.Save())
    {
        using Circle circle   = new(cr, 100);
        using Square square   = new(cr, 100);
        using Hexagon hexagon = new(cr, 100);

        circle .Fill(cr, 150, 150, KnownColors.Coral);
        square .Draw(cr, 400, 150, KnownColors.Pink);
        hexagon.Draw(cr, 650, 150, KnownColors.Brown);

        circle .Fill(cr, 900, 150, KnownColors.Coral);
        square .Draw(cr, 900, 150, KnownColors.Pink);
        hexagon.Draw(cr, 900, 150, KnownColors.Brown);

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
    }

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
    cr.FontSize  = 10;
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
