// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;
using Cairo;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Pango;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Tee;
using IOPath = System.IO.Path;

if (OperatingSystem.IsWindows())
{
    PangoNative.DllImportResolver = static (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
    {
        string? path = libraryName switch
        {
            PangoNative.LibPangoName      => IOPath.Combine(@"C:\Program Files\msys64\ucrt64\bin", "libpango-1.0-0.dll"),
            PangoNative.LibPangoCairoName => IOPath.Combine(@"C:\Program Files\msys64\ucrt64\bin", "libpangocairo-1.0-0.dll"),
            PangoNative.LibGObjectName    => IOPath.Combine(@"C:\Program Files\msys64\ucrt64\bin", "libgobject-2.0-0.dll"),
            _                             => null
        };

        if (path is not null && NativeLibrary.TryLoad(path, out nint handle))
        {
            return handle;
        }

        return default;
    };
}

if (Directory.Exists("output")) Directory.Delete("output", true);
Directory.CreateDirectory("output");
Environment.CurrentDirectory = IOPath.Combine(Environment.CurrentDirectory, "output");

DemoComparisonWithCairoText();
DemoFromPangoDocsWithTextAroundCircle();
DemoWithMarkup();
//-----------------------------------------------------------------------------
static void DemoComparisonWithCairoText()
{
    const int Width   = 600;
    const int Height  = 250;
    const string Text = "Hello from CairoSharp, w/o any AV and VAST";

    using SvgSurface svg  = new("cairo_comparison.svg", Width, Height);
    using PdfSurface pdf  = new("cairo_comparison.pdf", Width, Height);
    using TeeSurface tee  = new(svg);
    using CairoContext cr = new(tee);
    tee.Add(pdf);

    cr.Color = KnownColors.White;
    cr.Paint();

    cr.LineWidth = 4;
    cr.Color     = Color.Default;

    cr.Rectangle(0, 0, Width, Height);
    cr.Stroke();

    cr.LineWidth = 1;

    cr.MoveTo(10, 30);
    cr.ShowText("Demo for Kerning");

    cr.SelectFontFace("Times New Roman");
    cr.SetFontSize(28);
    cr.MoveTo(0, 100);
    cr.RelLineTo(Width, 0);
    cr.Stroke();
    cr.MoveTo(10, 100);
    cr.ShowText(Text);

    cr.MoveTo(0, 150);
    cr.RelLineTo(Width, 0);
    cr.Stroke();
    cr.MoveTo(10, 150);
    using (PangoLayout pangoLayout = new(cr))
    {
        pangoLayout.SetText(Text);
        pangoLayout.SetFontDescriptionFromString("Times New Roman, Normal 22");     // note the terminating , here at the font family
        pangoLayout.ShowLayout();
    }

    tee.WriteToPng("cairo_comparison.png");
}
//-----------------------------------------------------------------------------
static void DemoFromPangoDocsWithTextAroundCircle()
{
    // Demo from https://docs.gtk.org/PangoCairo/pango_cairo.html

    const int Radius  = 150;
    const int Words   = 10;
    const string Font = "Sans Bold 27";

    const int Width  = 2 * Radius;
    const int Height = 2 * Radius;

    using SvgSurface svg  = new("sample_from_docs.svg", Width, Height);
    using PdfSurface pdf  = new("sample_from_docs.pdf", Width, Height);
    using TeeSurface tee  = new(svg);
    using CairoContext cr = new(tee);
    tee.Add(pdf);

    using (cr.Save())
    {
        cr.Color = KnownColors.White;
        cr.Paint();

        cr.Color     = Color.Default;
        cr.LineWidth = 6;
        cr.Rectangle(0, 0, Width, Height);
        cr.Stroke();
    }

    cr.Translate(Radius, Radius);

    using PangoLayout pangoLayout = new(cr);
    pangoLayout.SetText("Text"u8);
    pangoLayout.SetFontDescriptionFromString(Font);

    for (int i = 0; i < Words; ++i)
    {
        double angle = (360d * i) / Words;

        using (cr.Save())
        {
            double red = (1 + Math.Cos((angle - 60) * Math.PI / 180d)) / 2d;
            cr.SetSourceRgb(red, 0, 1d - red);

            cr.Rotate(angle.DegreesToRadians());
            pangoLayout.UpdateLayout();

            pangoLayout.GetSize(out int width, out int height);
            cr.MoveTo(-((double)width / Pango.Scale) / 2, -Radius);

            pangoLayout.ShowLayout();
        }
    }

    tee.WriteToPng("sample_from_docs.png");
}
//-----------------------------------------------------------------------------
static void DemoWithMarkup()
{
    const int Width  = 600;
    const int Height = 250;

    using SvgSurface svg  = new("markup.svg", Width, Height);
    using PdfSurface pdf  = new("markup.pdf", Width, Height);
    using TeeSurface tee  = new(svg);
    using CairoContext cr = new(tee);
    tee.Add(pdf);

    cr.Color = KnownColors.White;
    cr.Paint();

    cr.LineWidth = 4;
    cr.Color     = Color.Default;

    cr.Rectangle(0, 0, Width, Height);
    cr.Stroke();

    using (PangoLayout pangoLayout = new(cr))
    {
        cr.MoveTo(10, 10);
        pangoLayout.SetMarkup("""
            <span foreground="blue" size="x-large">Blue text</span> is <i>cool</i>!
            """);
        pangoLayout.ShowLayout();

        cr.MoveTo(10, 40);
        pangoLayout.SetFontDescriptionFromString("Times New Roman, Normal 22");     // note the terminating , here at the font family
        pangoLayout.ShowLayout();

        cr.MoveTo(10, 80);
        pangoLayout.SetMarkup("""
            <span foreground="coral">This is quite a bit of text</span>, just to showcase what happens when the text is longer than the surface.
            """);
        pangoLayout.ShowLayout();
    }

    tee.WriteToPng("markup.png");
}
