// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Tee;

unsafe
{
    Console.WriteLine($"FreeType version: {FreeTypeFont.FreeTypeLibVersion()}");

    using FreeTypeFont freeTypeFont = FreeTypeFont.LoadFromFile("SanRemo.ttf");

    using SvgSurface svg = new("test.svg", 400, 200);
    using PdfSurface pdf = new("test.pdf", 400, 200);
    using TeeSurface tee = new(svg);
    tee.Add(pdf);
    using CairoContext cr = new(tee);

    cr.Rectangle(0, 0, 400, 200);
    cr.Stroke();

    cr.FontFace = freeTypeFont;
    cr.SetFontSize(28);

    const string Text = "Hello from CairoSharp";
    cr.MoveTo(10, 100);
    cr.ShowTextGlyphs(Text);

    tee.WriteToPng("test.png");
}

// This is not really needed, but here just to showcase
// BUT: when called it MUST be outside the scopes of the surfaces, because for
// rendering they need FreeType, so it must not be destroyed before.
FreeTypeFont.DoneFreeType();
