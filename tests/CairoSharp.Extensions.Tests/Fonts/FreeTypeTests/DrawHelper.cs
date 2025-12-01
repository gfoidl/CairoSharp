// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Fonts;
using Cairo.Fonts;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces.Recording;
using Cairo.Surfaces.SVG;

namespace CairoSharp.Extensions.Tests.Fonts.FreeTypeTests;

internal static class DrawHelper
{
    public static byte[] DrawFontOptions()
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

            using (FreeTypeFont freeTypeFont = Helper.LoadFreeTypeFontFromFile("SplineSans-Regular.otf"))
            {
                cr.FontFace = freeTypeFont;

                ReadOnlySpan<byte> text = "SplineSans regular, no options set"u8;
                cr.TextExtents(text, out TextExtents textExtents);
                cr.MoveTo(0, curY + textExtents.Height);
                cr.ShowTextGlyphs(text);

                curY += textExtents.Height + PaddingY;
            }

            using (FreeTypeFont freeTypeFont = Helper.LoadFreeTypeFontFromFile("SplineSans-Regular.otf"))
            using (freeTypeFont.SetSynthesize(Synthesize.Bold | Synthesize.Oblique))
            {
                cr.FontFace = freeTypeFont;

                ReadOnlySpan<byte> text = "SplineSans regular, synthesized bold | oblique"u8;
                cr.TextExtents(text, out TextExtents textExtents);
                cr.MoveTo(0, curY + textExtents.Height);
                cr.ShowTextGlyphs(text);

                curY += textExtents.Height + PaddingY;
            }

            using FontOptions defaultFontOptions = new();
            cr.FontFace                          = DefaultFonts.SansSerif;

            using (FontOptions fontOptions = defaultFontOptions.Copy())
            {
                fontOptions.Antialias = Antialias.Best;     // for pixel graphics only
                fontOptions.HintStyle = HintStyle.Full;

                cr.SetFontOptions(fontOptions);

                ReadOnlySpan<byte> text = "Helvetica, anti-alias best, hint-style full"u8;
                cr.TextExtents(text, out TextExtents textExtents);
                cr.MoveTo(0, curY + textExtents.Height);
                cr.ShowTextGlyphs(text);

                curY += textExtents.Height + PaddingY;
            }

            cr.SetFontOptions(defaultFontOptions);

            using (FreeTypeFont freeTypeFont = Helper.LoadFreeTypeFontFromFile("Fraunces-VariableFont_SOFT,WONK,opsz,wght.ttf"))
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
                using FreeTypeFont freeTypeFont = Helper.LoadFreeTypeFontFromFile("Fraunces-VariableFont_SOFT,WONK,opsz,wght.ttf");
                using FontOptions fontOptions   = new();

                // Settings on the font options must be set completely before assigning them to cr!
                fontOptions.Variations = $"wght={fontWeight}";

                cr.FontFace = freeTypeFont;
                cr.SetFontOptions(fontOptions);

                string text = $"Fraunces variable font, wght={fontWeight}";
                cr.TextExtents(text, out TextExtents textExtents);
                cr.MoveTo(0, curY + textExtents.Height);
                cr.ShowTextGlyphs(text);

                curY += textExtents.Height + PaddingY;
            }
        }
    }
    //-------------------------------------------------------------------------
    public static byte[] DrawText(bool useDefaultFont)
    {
        const double Size = 500;

        MemoryStream resultStream = new();
        using (SvgSurface svg     = new(resultStream, Size, Size))
        using (CairoContext cr    = new(svg))
        {
            cr.FontFace = DefaultFonts.SansSerifBold;
            cr.SetFontSize(64);
            (_, double y) = cr.TextAlignCenter("CairoSharp"u8, Size, Size, out TextExtents textExtents, moveCurrentPoint: true);
            cr.ShowText("CairoSharp"u8);

            using FreeTypeFont sanRemoFont = Helper.LoadFreeTypeFontFromFile("SanRemo.ttf");
            using (sanRemoFont.SetSynthesize(Synthesize.Bold))
            {
                ReadOnlySpan<byte> text = "<3 cairo"u8;

                cr.FontFace   = sanRemoFont;
                (double x, _) = cr.TextAlignCenter(text, Size, Size, out textExtents);
                cr.FontExtents(out FontExtents fontExtents);
                cr.MoveTo(x, y + fontExtents.Height);
                cr.TextPath(text);

                cr.Color = KnownColors.Blue;
                cr.FillPreserve();
                cr.Color = KnownColors.Yellow;
                cr.Stroke();
            }
        }

        return resultStream.ToArray();
    }
}
