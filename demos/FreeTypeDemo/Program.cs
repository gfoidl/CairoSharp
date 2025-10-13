// (c) gfoidl, all rights reserved

using System.Reflection;
using Cairo;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Tee;

Console.WriteLine($"FreeType version: {FreeTypeFont.FreeTypeLibVersion()}");

LoadFontFromFile();
LoadFontFromResourceStream();

// This is not really needed, but here just to showcase
// BUT: when called it MUST be outside the scopes of the surfaces, because for
// rendering they need FreeType, so it must not be destroyed before.
FreeTypeFont.DoneFreeType();

static void LoadFontFromFile()
{
    using FreeTypeFont freeTypeFont = FreeTypeFont.LoadFromFile("SanRemo.ttf");

    Core(freeTypeFont, "test0");
}

static void LoadFontFromResourceStream()
{
    using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FreeTypeDemo.SanRemo.ttf")!;
    byte[] fontData         = new byte[fontStream.Length];
    fontStream.ReadExactly(fontData);

    using FreeTypeFont freeTypeFont = FreeTypeFont.LoadFromData(fontData);

    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();

    Core(freeTypeFont, "test1");
}

static void Core(FreeTypeFont freeTypeFont, string fileName)
{
    using SvgSurface svg = new($"{fileName}.svg", 400, 200);
    using PdfSurface pdf = new($"{fileName}.pdf", 400, 200);
    using TeeSurface tee = new(svg);
    tee.Add(pdf);
    using CairoContext cr = new(tee);

    cr.Rectangle(0, 0, 400, 200);
    cr.Stroke();

    cr.FontFace = freeTypeFont;
    cr.SetFontSize(28);

    cr.MoveTo(10, 100);
    cr.ShowTextGlyphs("Hello from CairoSharp");

    tee.WriteToPng($"{fileName}.png");
}
