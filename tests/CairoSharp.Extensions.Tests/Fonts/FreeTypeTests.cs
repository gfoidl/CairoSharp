// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Fonts;
using Cairo.Fonts;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.Recording;
using Cairo.Surfaces.SVG;

namespace CairoSharp.Extensions.Tests.Fonts;

[TestFixture]
public class FreeTypeTests
{
    private static readonly byte[] s_expectedSvgData = File.ReadAllBytes("Fonts/font-options.svg");
    //-------------------------------------------------------------------------
    [Test]
    public void FontOptions_demo___OK()
    {
        byte[] actual = DrawCore();

        Assert.That(actual, Is.EqualTo(s_expectedSvgData).AsCollection);
    }
    //-------------------------------------------------------------------------
    private static byte[] DrawCore()
    {
        // Code is taken from the FreeTypeDemo, just with only SVG surface (instead of Tee with PDF, and PNG).

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

        MemoryStream resultStream = new();
        using (SvgSurface svg     = new(resultStream, width, height))
        using (CairoContext cr    = new(svg))
        {
            cr.Color = KnownColors.White;
            cr.Paint();

            cr.Color = Color.Default;
            cr.Rectangle(0, 0, width, height);
            cr.Stroke();

            // Due to font synthesizing / font options can't use the recording surface
            // as source surface, instead we need to draw it again manually.
            // I don't know if this is by design or a bug in cairo.
            Core(cr);
        }

        return resultStream.ToArray();
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
    //-------------------------------------------------------------------------
    private static FreeTypeFont LoadFreeTypeFontFromFile(string fontName) => FreeTypeFont.LoadFromFile($"Fonts/fonts/{fontName}");
}
