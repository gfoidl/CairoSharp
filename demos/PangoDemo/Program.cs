// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Cairo;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Pango;
using Cairo.Fonts;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.Recording;
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
            PangoNative.LibGLibName       => IOPath.Combine(@"C:\Program Files\msys64\ucrt64\bin", "libglib-2.0-0.dll"),
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

ComparisonWithCairoTextDemo();
FontSelectionDemo();
PangoDocsDemoWithTextAroundCircle();
PangoFeaturesDemo();
FontMapDemo();
RecordingSurfaceDemo();
FontListDemo();
//-----------------------------------------------------------------------------
static void ComparisonWithCairoTextDemo()
{
    const int Width   = 600;
    const int Height  = 150;
    const string Text = "Hello from CairoSharp, w/o any AV and VA";

    using SvgSurface svg  = new("cairo_comparison.svg", Width, Height);
    using PdfSurface pdf  = new("cairo_comparison.pdf", Width, Height);
    using TeeSurface tee  = new(svg);
    using CairoContext cr = new(tee);
    tee.Add(pdf);

    cr.Color = KnownColors.White;
    cr.Paint();
    cr.LineWidth = 6;
    cr.Color     = Color.Default;
    cr.Rectangle(0, 0, Width, Height);
    cr.Stroke();
    cr.LineWidth = 1;

    using (cr.Save())
    {
        cr.Color = KnownColors.Coral;
        cr.MoveTo(10, 30);
        cr.ShowText("Demo for Kerning");
    }

    FontExtents fontExtents;

    // Cairo text rendering
    {
        cr.SelectFontFace("Times New Roman");
        cr.SetFontSize(28);
        cr.MoveTo(0, 75);
        cr.RelLineTo(Width, 0);
        cr.Stroke();
        cr.MoveTo(10, 75);
        cr.ShowText(Text);
        cr.FontExtents(out fontExtents);
    }

    // Pango text rendering
    {
        cr.Translate(0, 75 + fontExtents.Descent + 5);
        cr.MoveTo(0, 0);
        cr.RelLineTo(Width, 0);
        cr.Stroke();
        cr.MoveTo(10, 0);
        using (PangoLayout pangoLayout = new(cr))
        {
            pangoLayout.SetText(Text);

            // note the terminating , here at the font family
            pangoLayout.SetFontDescriptionFromString("Times New Roman, Normal 28");

            pangoLayout.ShowLayout();
        }
    }

    tee.WriteToPng("cairo_comparison.png");
}
//-----------------------------------------------------------------------------
static void FontSelectionDemo()
{
    const int Width   = 800;
    const int Height  = 100;
    const string Text = "Hello from CairoSharp, w/o any AV and VA";

    using SvgSurface svg  = new("font-selection.svg", Width, Height);
    using PdfSurface pdf  = new("font-selection.pdf", Width, Height);
    using TeeSurface tee  = new(svg);
    using CairoContext cr = new(tee);
    tee.Add(pdf);

    cr.Color = KnownColors.White;
    cr.Paint();
    cr.LineWidth = 6;
    cr.Color     = Color.Default;
    cr.Rectangle(0, 0, Width, Height);
    cr.Stroke();
    cr.LineWidth = 1;

    double curY = 10;
    using PangoLayout pangoLayout = new(cr);

    cr.MoveTo(10, curY);
    pangoLayout.SetText($"{Text} (Pango font from string)");
    string? fontDescriptionAsString = pangoLayout.SetFontDescriptionFromString("DejaVu Serif, Normal 22");
    Console.WriteLine(fontDescriptionAsString);
    pangoLayout.ShowLayout();
    pangoLayout.GetSize(out double width, out double height);

    curY += height;
    cr.MoveTo(10, curY);
    using PangoFontMap fontMap       = PangoFontMap.CairoFontMapGetDefault();
    using PangoFontFamily fontFamily = fontMap.GetFamily("DejaVu Serif");
    using PangoFontFace? fontFace    = fontFamily.GetFace(null);
    Debug.Assert(fontFace is not null);
    pangoLayout.SetText($"{Text} (Pango font selected)");
    fontDescriptionAsString = pangoLayout.SetFontDescription(fontFace, size: 22);
    Console.WriteLine(fontDescriptionAsString);
    pangoLayout.ShowLayout();
    pangoLayout.GetSize(out width, out height);
    curY += height;

    cr.SelectFontFace("DejaVu Serif");
    cr.SetFontSize(22);
    cr.FontExtents(out FontExtents fontExtents);
    curY += fontExtents.Ascent;
    cr.MoveTo(10, curY);
    cr.ShowText($"{Text} (cairo)");

    tee.WriteToPng("font-selection.png");
}
//-----------------------------------------------------------------------------
static void PangoDocsDemoWithTextAroundCircle()
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
    pangoLayout.Resolution = Pango.DefaultResolution;

    for (int i = 0; i < Words; ++i)
    {
        double angle = (360d * i) / Words;

        using (cr.Save())
        {
            double red = (1 + Math.Cos((angle - 60) * Math.PI / 180d)) / 2d;
            cr.SetSourceRgb(red, 0, 1d - red);

            cr.Rotate(angle.DegreesToRadians());

            // Need to update the layout, as the current transformation got changed by Rotate
            pangoLayout.UpdateLayout();

            pangoLayout.GetSize(out double width, out double height);
            cr.MoveTo(-width / 2, -Radius);

            pangoLayout.ShowLayout();
        }
    }

    tee.WriteToPng("sample_from_docs.png");
}
//-----------------------------------------------------------------------------
static void PangoFeaturesDemo()
{
    const int Width  = 600;
    const int Height = 620;

    using SvgSurface svg  = new("features.svg", Width, Height);
    using PdfSurface pdf  = new("features.pdf", Width, Height);
    using TeeSurface tee  = new(svg);
    using CairoContext cr = new(tee);
    tee.Add(pdf);

    cr.Color = KnownColors.White;
    cr.Paint();
    cr.LineWidth = 6;
    cr.Color     = Color.Default;
    cr.Rectangle(0, 0, Width, Height);
    cr.Stroke();
    cr.LineWidth = 1;

    using (PangoLayout pangoLayout = new(cr))
    {
        double curY = 10;

        // Markup
        {
            cr.MoveTo(10, curY);
            pangoLayout.SetMarkup("""
                <span foreground="blue" size="x-large">Blue text</span> is <i>cool</i>!
                """);
            pangoLayout.ShowLayout();
        }
        pangoLayout.GetSize(out double width, out double height);
        curY += height;

        // Markup with selected font
        {
            cr.MoveTo(10, curY);
            pangoLayout.SetFontDescriptionFromString("Times New Roman, Normal 22");     // note the terminating , here at the font family
            pangoLayout.ShowLayout();
        }
        pangoLayout.GetSize(out width, out height);
        curY += height;

        cr.MoveTo(0, curY);
        cr.RelLineTo(Width, 0);
        cr.Stroke();

        // Wrapping by setting the width
        {
            Debug.Assert(pangoLayout.Width == -1);

            cr.MoveTo(10, curY);
            pangoLayout.SetMarkup("""
                <span foreground="coral">This is quite a bit of text</span>, just to showcase what happens when the text is longer than the surface.
                """);
            Console.WriteLine($"Is in wrap-mode: {pangoLayout.IsWrapped}");
            pangoLayout.ShowLayout();

            pangoLayout.GetSize(out width, out height);
            curY += height;

            cr.MoveTo(10, curY);
            pangoLayout.SetMarkup("""
                <span foreground="coral">This is quite a bit of text</span>, just to showcase what happens when the text is longer than the surface.
                """);
            Console.WriteLine($"Is in wrap-mode: {pangoLayout.IsWrapped}");
            pangoLayout.Width = Width - 2 * 10;
            Console.WriteLine($"Is in wrap-mode: {pangoLayout.IsWrapped}");
            pangoLayout.ShowLayout();
        }
        pangoLayout.GetSize(out width, out height);
        curY += height;

        cr.MoveTo(0, curY);
        cr.RelLineTo(Width, 0);
        cr.Stroke();

        // Alignment
        {
            pangoLayout.Width = Width - 2 * 10;

            cr.MoveTo(10, curY);
            pangoLayout.SetText("Left aligned");
            pangoLayout.Alignment = Alignment.Left;
            pangoLayout.ShowLayout();

            pangoLayout.GetSize(out width, out height);
            curY += height;

            cr.MoveTo(10, curY);
            pangoLayout.SetText("Center aligned");
            pangoLayout.Alignment = Alignment.Center;
            pangoLayout.ShowLayout();

            pangoLayout.GetSize(out width, out height);
            curY += height;

            cr.MoveTo(10, curY);
            pangoLayout.SetText("Right aligned");
            pangoLayout.Alignment = Alignment.Right;
            pangoLayout.ShowLayout();

            pangoLayout.Alignment = Alignment.Left;
        }
        pangoLayout.GetSize(out width, out height);
        curY += height;

        cr.MoveTo(0, curY);
        cr.RelLineTo(Width, 0);
        cr.Stroke();

        pangoLayout.SetFontDescriptionFromString("Serif Normal 22");

        // Justify
        {
            cr.Color = KnownColors.DeepSkyBlue;
            cr.MoveTo(10, curY);
            pangoLayout.SetMarkup("Another long text, to showcase how <tt>Justify</tt> works. And as we get on at least one more line, we can test next how <tt>JustifyLastLine</tt> works."u8);
            pangoLayout.ShowLayout();

            pangoLayout.GetSize(out width, out height);
            curY += height;

            cr.Color = KnownColors.Green;
            cr.MoveTo(10, curY);
            pangoLayout.Justify = true;
            pangoLayout.ShowLayout();

            pangoLayout.GetSize(out width, out height);
            curY += height;

            cr.Color = KnownColors.Blue;
            cr.MoveTo(10, curY);
            pangoLayout.Justify         = true;
            pangoLayout.JustifyLastLine = true;
            pangoLayout.ShowLayout();

            pangoLayout.Justify = false;
        }
        pangoLayout.GetSize(out width, out height);
        curY += height;

        cr.Color = Color.Default;
        cr.MoveTo(0, curY);
        cr.RelLineTo(Width, 0);
        cr.Stroke();

        // LayoutPath
        {
            cr.MoveTo(10, curY);
            pangoLayout.SetText("CairoSharp"u8);
            pangoLayout.LayoutPath();

            cr.Color = KnownColors.Cyan;
            cr.FillPreserve();
            cr.Color = KnownColors.Red;
            cr.Stroke();
        }
        pangoLayout.GetSize(out width, out height);
        curY += height;

        cr.Color = Color.Default;
        cr.MoveTo(0, curY);
        cr.RelLineTo(Width, 0);
        cr.Stroke();

        // Ident
        {
            cr.Color = KnownColors.Green;
            cr.MoveTo(10, curY);
            pangoLayout.SetText("Again, a longer text to get at least another line, in order to see how Ident behaves. But just one additional line is not enough, so we have more words to get another line.");
            pangoLayout.Ident = 20;
            pangoLayout.ShowLayout();

            pangoLayout.GetSize(out width, out height);
            curY += height;

            cr.Color = KnownColors.Blue;
            cr.MoveTo(10, curY);
            pangoLayout.Ident = -20;
            pangoLayout.ShowLayout();
        }
    }

    tee.WriteToPng("features.png");
}
//-----------------------------------------------------------------------------
static void FontMapDemo()
{
    Console.WriteLine();
    using StreamWriter sw = File.CreateText("font-families.csv");
    sw.WriteLine("Name;IsMonospace;IsVariable");

    using PangoFontMap fontMap = PangoFontMap.CairoFontMapGetDefault();
    fontMap.AddFontFile(IOPath.Combine(AppContext.BaseDirectory, "fonts", "SanRemo.ttf"));

    List<PangoFontFamily> families = [];
    foreach (PangoFontFamily fontFamily in fontMap.ListFamilies())
    {
        families.Add(fontFamily);
    }

    string fontFamilyName = "";
    foreach (PangoFontFamily fontFamily in families.OrderBy(f => f.Name))
    {
        fontFamilyName = fontFamily.Name;

        sw.WriteLine($"{fontFamilyName};{fontFamily.IsMonospace};{fontFamily.IsVariable}");
        Console.WriteLine(fontFamily);
        Console.WriteLine("Faces:");

        foreach (PangoFontFace fontFace in fontFamily.ListFaces())
        {
            Console.WriteLine($"\t{fontFace.Name}\tsynthesized: {fontFace.IsSynthesized}");
            Console.WriteLine("\tSizes:");

            foreach (double size in fontFace.ListSizes())
            {
                Console.WriteLine($"\t\t{size}");
            }
        }

        Console.WriteLine();
    }

    const int Width  = 600;
    const int Height = 120;

    using SvgSurface svg  = new("font-map.svg", Width, Height);
    using PdfSurface pdf  = new("font-map.pdf", Width, Height);
    using TeeSurface tee  = new(svg);
    using CairoContext cr = new(tee);
    tee.Add(pdf);

    using PangoLayout pangoLayout = new(cr);
    double curY                   = 10;

    cr.MoveTo(10, curY);
    pangoLayout.SetFontDescriptionFromString($"{fontFamilyName} Normal 22");
    pangoLayout.SetText($"Font: {fontFamilyName}");
    pangoLayout.ShowLayout();
    pangoLayout.GetSize(out double width, out double height);
    curY += height;

    cr.MoveTo(10, curY);
    pangoLayout.SetFontDescriptionFromString("Viner Hand ITC, Normal 22");
    pangoLayout.SetText("Font: Viner Hand ITC");
    pangoLayout.ShowLayout();
    pangoLayout.GetSize(out width, out height);
    curY += height;

    cr.MoveTo(10, curY);
    pangoLayout.SetFontDescriptionFromString("San Remo, Normal 22");
    pangoLayout.SetText("Font: San Remo");
    pangoLayout.ShowLayout();
    pangoLayout.GetSize(out width, out height);
    curY += height;

    tee.WriteToPng("font-map.png");

    families.ForEach(f => f.Dispose());
}
//-----------------------------------------------------------------------------
static void RecordingSurfaceDemo()
{
    using PangoFontMap fontMap = PangoFontMap.CairoFontMapGetDefault();
    fontMap.AddFontFile(IOPath.Combine(AppContext.BaseDirectory, "fonts", "SanRemo.ttf"));

    List<PangoFontFamily> families = [];
    foreach (PangoFontFamily fontFamily in fontMap.ListFamilies())
    {
        families.Add(fontFamily);
    }

    string fontFamilyName = "";
    foreach (PangoFontFamily fontFamily in families.OrderBy(f => f.Name))
    {
        fontFamilyName = fontFamily.Name;
    }

    using RecordingSurface surface = new();
    using (CairoContext cr         = new(surface))
    {
        using PangoLayout pangoLayout = new(cr);
        double curY                   = 10;

        cr.MoveTo(10, curY);
        pangoLayout.SetFontDescriptionFromString($"{fontFamilyName} Normal 22");
        pangoLayout.SetText($"Font: {fontFamilyName}");
        pangoLayout.ShowLayout();
        pangoLayout.GetSize(out double width, out double height);
        curY += height;

        cr.MoveTo(10, curY);
        pangoLayout.SetFontDescriptionFromString("Viner Hand ITC, Normal 22");
        pangoLayout.SetText("Font: Viner Hand ITC");
        pangoLayout.ShowLayout();
        pangoLayout.GetSize(out width, out height);
        curY += height;

        cr.MoveTo(10, curY);
        pangoLayout.SetFontDescriptionFromString("San Remo, Normal 22");
        pangoLayout.SetText("Font: San Remo");
        pangoLayout.ShowLayout();
        pangoLayout.GetSize(out width, out height);
        curY += height;
    }

    Rectangle recordingExtents = surface.GetInkExtents();
    const int Padding          = 10;

    using SvgSurface svg   = new("recording.svg", recordingExtents.Width + 2 * Padding, recordingExtents.Height + 2 * Padding);
    using PdfSurface pdf   = new("recording.pdf", recordingExtents.Width + 2 * Padding, recordingExtents.Height + 2 * Padding);
    using TeeSurface tee   = new(svg);
    using (CairoContext cr = new(tee))
    {
        tee.Add(pdf);

        cr.SetSourceSurface(surface, Padding - recordingExtents.X, Padding - recordingExtents.Y);
        cr.Paint();
    }

    tee.WriteToPng("recording.png");

    families.ForEach(f => f.Dispose());
}
//-----------------------------------------------------------------------------
static void FontListDemo()
{
    using PangoFontMap fontMap = PangoFontMap.CairoFontMapGetDefault();
    fontMap.AddFontFile(IOPath.Combine(AppContext.BaseDirectory, "fonts", "SanRemo.ttf"));

    List<PangoFontFamily> families = [];
    foreach (PangoFontFamily fontFamily in fontMap.ListFamilies())
    {
        families.Add(fontFamily);
    }
    families = [.. families
        .OrderBy(f => f.IsMonospace)
        .ThenBy (f => f.Name)
    ];

    using RecordingSurface surface = new();
    using (CairoContext cr         = new(surface))
    {
        using PangoLayout pangoLayout = new(cr);
        double curY                   = 10;

        foreach (PangoFontFamily fontFamily in families)
        {
            using PangoFontFace? fontFace = fontFamily.GetFace(null);

            if (fontFace is null)
            {
                continue;
            }

            cr.MoveTo(10, curY);
            pangoLayout.SetText($"Font: {fontFamily.Name} {fontFace.Name}; AV and VA to see kerning or not");
            pangoLayout.SetFontDescription(fontFace, size: 22);
            pangoLayout.ShowLayout();
            pangoLayout.GetSize(out double width, out double height);
            curY += height;
        }
    }

    Rectangle recordingExtents = surface.GetInkExtents();
    const int Padding          = 10;
    double surfaceWidth        = recordingExtents.Width  + 2 * Padding;
    double surfaceHeight       = recordingExtents.Height + 2 * Padding;

    using SvgSurface svg   = new("font-list.svg", surfaceWidth, surfaceHeight);
    using PdfSurface pdf   = new("font-list.pdf", surfaceWidth, surfaceHeight);
    using TeeSurface tee   = new(svg);
    using (CairoContext cr = new(tee))
    {
        tee.Add(pdf);

        cr.Color = KnownColors.White;
        cr.Paint();
        cr.Color = Color.Default;
        cr.LineWidth = 4;
        cr.Rectangle(0, 0, surfaceWidth, surfaceHeight);
        cr.Stroke();

        cr.SetSourceSurface(surface, Padding - recordingExtents.X, Padding - recordingExtents.Y);
        cr.Paint();
    }

    tee.WriteToPng("font-list.png");

    families.ForEach(f => f.Dispose());
}
