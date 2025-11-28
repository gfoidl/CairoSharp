// (c) gfoidl, all rights reserved

//#define USE_TEXT_PATH

using System.Diagnostics;
using System.Reflection;
using Cairo;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Fonts;
using Cairo.Fonts;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.Recording;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Tee;

if (Directory.Exists("output")) Directory.Delete("output", true);
Directory.CreateDirectory("output");

Console.WriteLine($"FreeType version: {FreeTypeFont.FreeTypeLibVersion()}");
Console.WriteLine();

string[] fontNames =
[
    "SanRemo.ttf",
    "SplineSans-Variable.ttf",
    "SplineSans-Regular.otf"
];

PrintSurfaceInformation();

Core("ft_font_via_file"  , fontNames, LoadFreeTypeFontFromFile);
Core("ft_font_via_data"  , fontNames, LoadFreeTypeFontFromResourceStreamViaLocalArray);
Core("ft_font_via_stream", fontNames, LoadFreeTypeFontFromResourceStream);

FontOptionsDemo();

// This is not really needed, but here just to showcase
// BUT: when called it MUST be outside the scopes of the surfaces, because for
// rendering they need FreeType, so it must not be destroyed before.
FreeTypeFont.DoneFreeType();
//-----------------------------------------------------------------------------
static FreeTypeFont LoadFreeTypeFontFromFile(string fontName)
{
    return FreeTypeFont.LoadFromFile($"fonts/{fontName}");
}

static FreeTypeFont LoadFreeTypeFontFromResourceStreamViaLocalArray(string fontName)
{
    using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fontName)!;
    byte[] fontData         = new byte[fontStream.Length];
    fontStream.ReadExactly(fontData);

    return FreeTypeFont.LoadFromData(fontData);
}

static FreeTypeFont LoadFreeTypeFontFromResourceStream(string fontName)
{
    using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fontName)!;
    return FreeTypeFont.LoadFromStream(fontStream);
}
//-----------------------------------------------------------------------------
static void PrintSurfaceInformation()
{
    using SvgSurface svgSurface = new(100, 100);
    using PdfSurface pdfSurface = new(100,100);

    Console.WriteLine($"""
        HasShowTextGlyphs:
            SVG: {svgSurface.HasShowTextGlyphs}
            PDF: {pdfSurface.HasShowTextGlyphs}
        """);

    Console.WriteLine();
}
//-----------------------------------------------------------------------------
static void Core(string fileName, string[] fontNames, Func<string, FreeTypeFont> fontFactory)
{
    const double FontSize = 36;
    const double PaddingX = 10;
    const double PaddingY = FontSize / 2;

    List<(FreeTypeFont Ft, string Name)> freeTypeFonts = [.. fontNames.Select(name => (fontFactory(name), name))];

    IEnumerable<(FontFace, string)> fontFaces = GetDefaultFonts()
        .Concat(freeTypeFonts.Select(ft => ((ft.Ft as FontFace)!, ft.Name)));

    using RecordingSurface recordingSurface = new();
    using (CairoContext cr                  = new(recordingSurface))
    {
        cr.SetFontSize(FontSize);

        PrintFontInfo(cr);

        cr.Translate(PaddingX, PaddingY);
        double curY = 0;

        foreach ((FontFace Face, string Name) fontFace in fontFaces)
        {
            cr.FontFace = fontFace.Face;

            PrintFontInfo(cr);

            string text = $"Hello from CairoSharp (font: {fontFace.Name})";
            cr.TextExtents(text, out TextExtents textExtents);

            cr.MoveTo(0, curY + textExtents.Height);

            curY += textExtents.Height + PaddingY;

#if USE_TEXT_PATH
            cr.TextPath(text);
            cr.Fill();
#else
            cr.ShowTextGlyphs(text);
#endif
        }
    }

    foreach ((FreeTypeFont freeTypeFont, string name) in freeTypeFonts)
    {
        freeTypeFont.Dispose();
    }

    Rectangle surfaceExtents = recordingSurface.GetInkExtents();
    double width             = surfaceExtents.Width  + 2 * PaddingX;
    double height            = surfaceExtents.Height + 2 * PaddingY;

    using SvgSurface svg = new($"output/{fileName}.svg", width, height);
    using PdfSurface pdf = new($"output/{fileName}.pdf", width, height);
    using TeeSurface tee = new(svg);
    tee.Add(pdf);

    using (CairoContext cr = new(tee))
    {
        cr.Color = KnownColors.White;
        cr.Paint();

        cr.Color = Color.Default;
        cr.Rectangle(0, 0, width, height);
        cr.Stroke();

        cr.SetSourceSurface(recordingSurface, 0, 0);
        cr.Paint();
    }

    tee.WriteToPng($"output/{fileName}.png");
    //-------------------------------------------------------------------------
    static IEnumerable<(FontFace, string)> GetDefaultFonts()
    {
        yield return (DefaultFonts.SansSerif    , "Helvetica");
        yield return (DefaultFonts.SansSerifBold, "Helvetica (bold)");
        yield return (DefaultFonts.MonoSpace    , "Inconsolata (monospace)");
        yield return (DefaultFonts.MonoSpaceBold, "Inconsolata (monospace, bold)");
    }
}
//-----------------------------------------------------------------------------
static void PrintFontInfo(CairoContext cr)
{
    FontFace fontFace = cr.GetTypedFontFace();
    FontType fontType = fontFace.FontType;

    if (fontType == FontType.Toy)
    {
        ToyFontFace? toyFontFace = fontFace as ToyFontFace;
        Debug.Assert(toyFontFace is not null);

        Console.WriteLine($"FontType = {fontType}\tFontFamily = {toyFontFace.Family}");
    }
    else
    {
        Console.WriteLine($"FontType = {fontType}");
    }
}
//-----------------------------------------------------------------------------
static void FontOptionsDemo()
{
    const double FontSize = 36;
    const double PaddingX = 10;
    const double PaddingY = FontSize / 2;

    // Just to get the bounding box in a lazy way...
    using RecordingSurface recordingSurface = new();
    using (CairoContext cr                  = new(recordingSurface))
    {
        Core(cr);
    }

    Rectangle surfaceExtents = recordingSurface.GetInkExtents();
    double width             = surfaceExtents.Width  + 2 * PaddingX;
    double height            = surfaceExtents.Height + 2 * PaddingY;

    const string FileName = "font-options";
    using SvgSurface svg  = new($"output/{FileName}.svg", width, height);
    using PdfSurface pdf  = new($"output/{FileName}.pdf", width, height);
    using TeeSurface tee  = new(svg);
    tee.Add(pdf);

    using (CairoContext cr = new(tee))
    {
        cr.Color = KnownColors.White;
        cr.Paint();

        cr.Color = Color.Default;
        cr.Rectangle(0, 0, width, height);
        cr.Stroke();

        // Due the font synthesizing / font options can't use the recording surface
        // as source surface, instead we need to draw it again manually.
        // I don't know if this is by design or a bug in cairo.
        Core(cr);
    }

    tee.WriteToPng($"output/{FileName}.png");
    //---------------------------------------------------------------------
    static void Core(CairoContext cr)
    {
        cr.SetFontSize(FontSize);

        cr.Translate(PaddingX, PaddingY);
        double curY = 0;

        using (FreeTypeFont freeTypeFont = LoadFreeTypeFontFromFile("SplineSans-Regular.otf"))
        {
            cr.FontFace = freeTypeFont;

            ReadOnlySpan<byte> text = "SplineSans regular, no options set"u8;
            cr.TextExtents(text, out TextExtents textExtents);
            cr.MoveTo(0, curY + textExtents.Height);
            cr.ShowTextGlyphs(text);

            curY += textExtents.Height + PaddingY;
        }

        using (FreeTypeFont freeTypeFont = LoadFreeTypeFontFromFile("SplineSans-Regular.otf"))
        using (freeTypeFont.SetSynthesize(Synthesize.Bold | Synthesize.Oblique))
        {
            cr.FontFace = freeTypeFont;

            ReadOnlySpan<byte> text = "SplineSans regular, synthesized bold | oblique"u8;
            cr.TextExtents(text, out TextExtents textExtents);
            cr.MoveTo(0, curY + textExtents.Height);
            cr.ShowTextGlyphs(text);

            curY += textExtents.Height + PaddingY;
        }

        FontOptions defaultFontOptions = cr.FontOptions.Copy();
        cr.FontFace                    = DefaultFonts.SansSerif;

        using (FontOptions fontOptions = cr.FontOptions.Copy())
        {
            fontOptions.Antialias = Antialias.Best;     // for pixel graphics only
            fontOptions.HintStyle = HintStyle.Full;

            cr.FontOptions = fontOptions;

            ReadOnlySpan<byte> text = "Helvetica, anti-alias best, hint-style full"u8;
            cr.TextExtents(text, out TextExtents textExtents);
            cr.MoveTo(0, curY + textExtents.Height);
            cr.ShowTextGlyphs(text);

            curY += textExtents.Height + PaddingY;
        }

        cr.FontOptions = defaultFontOptions;

        using (FreeTypeFont freeTypeFont = LoadFreeTypeFontFromFile("Fraunces-VariableFont_SOFT,WONK,opsz,wght.ttf"))
        {
            cr.FontFace = freeTypeFont;

            ReadOnlySpan<byte> text = "Fraunces variable font, default"u8;
            cr.TextExtents(text, out TextExtents textExtents);
            cr.MoveTo(0, curY + textExtents.Height);
            cr.ShowTextGlyphs(text);

            curY += textExtents.Height + PaddingY;
        }

        foreach (int fontWeight in (ReadOnlySpan<int>)[100, 200, 400, 600, 800, 1000])
        {
            using FreeTypeFont freeTypeFont = LoadFreeTypeFontFromFile("Fraunces-VariableFont_SOFT,WONK,opsz,wght.ttf");
            using FontOptions fontOptions   = new();

            // Settings on the font options must be set completely before assigning them to cr!
            fontOptions.Variations = $"wght={fontWeight}";

            cr.FontFace    = freeTypeFont;
            cr.FontOptions = fontOptions;

            string text = $"Fraunces variable font, wght={fontWeight}";
            cr.TextExtents(text, out TextExtents textExtents);
            cr.MoveTo(0, curY + textExtents.Height);
            cr.ShowTextGlyphs(text);

            curY += textExtents.Height + PaddingY;
        }
    }
}
