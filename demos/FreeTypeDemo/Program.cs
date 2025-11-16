// (c) gfoidl, all rights reserved

//#define USE_TEXT_PATH

using System.Reflection;
using Cairo;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Tee;

Console.WriteLine($"FreeType version: {FreeTypeFont.FreeTypeLibVersion()}");
Console.WriteLine();

string[] fontNames =
[
    "SanRemo.ttf",
    "SplineSans-Variable.ttf",
    "SplineSans-Regular.otf"
];

foreach (string fontName in fontNames)
{
    LoadFontFromFile(fontName);
    LoadFontFromResourceStreamViaLocalArray(fontName);
    LoadFontFromResourceStream(fontName);
}

// This is not really needed, but here just to showcase
// BUT: when called it MUST be outside the scopes of the surfaces, because for
// rendering they need FreeType, so it must not be destroyed before.
FreeTypeFont.DoneFreeType();

static void LoadFontFromFile(string fontName)
{
    using FreeTypeFont freeTypeFont = FreeTypeFont.LoadFromFile($"fonts/{fontName}");

    Core(freeTypeFont, "test0", fontName);
}

static void LoadFontFromResourceStreamViaLocalArray(string fontName)
{
    using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fontName)!;
    byte[] fontData         = new byte[fontStream.Length];
    fontStream.ReadExactly(fontData);

    using FreeTypeFont freeTypeFont = FreeTypeFont.LoadFromData(fontData);

    Core(freeTypeFont, "test1", fontName);
}

static void LoadFontFromResourceStream(string fontName)
{
    using Stream fontStream         = Assembly.GetExecutingAssembly().GetManifestResourceStream(fontName)!;
    using FreeTypeFont freeTypeFont = FreeTypeFont.LoadFromStream(fontStream);

    Core(freeTypeFont, "test2", fontName);
}

static void Core(FreeTypeFont freeTypeFont, string fileName, string fontName)
{
    fileName = GetFileName(fileName, fontName);

    using SvgSurface svg = new($"{fileName}.svg", 400, 200);
    using PdfSurface pdf = new($"{fileName}.pdf", 400, 200);
    using TeeSurface tee = new(svg);
    tee.Add(pdf);
    using CairoContext cr = new(tee);

    if (!InfoState.InfoPrinted)
    {
        PrintSurfaceInformation(svg, pdf);
        InfoState.InfoPrinted = true;
    }

    cr.Rectangle(0, 0, 400, 200);
    cr.Stroke();

    cr.FontFace = freeTypeFont;
    cr.SetFontSize(28);

    cr.MoveTo(10, 100);

    const string Text = "Hello from CairoSharp";

#if USE_TEXT_PATH
    cr.TextPath(Text);
    cr.Fill();
#else
    cr.ShowTextGlyphs(Text);
#endif

    tee.WriteToPng($"{fileName}.png");
}

static string GetFileName(string fileName, string fontName)
{
    fontName = Path.GetFileNameWithoutExtension(fontName);
    return $"{fileName}_{fontName}";
}

static void PrintSurfaceInformation(SvgSurface svgSurface, PdfSurface pdfSurface)
{
    Console.WriteLine($"""
        HasShowTextGlyphs:
            SVG: {svgSurface.HasShowTextGlyphs}
            PDF: {pdfSurface.HasShowTextGlyphs}
        """);
}

internal static class InfoState
{
    public static bool InfoPrinted { get; set; }
}
