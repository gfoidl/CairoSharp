// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using Cairo;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Tee;
using FreeTypeSharp;

unsafe
{
    FT_LibraryRec_* freeTypeLibrary;
    FT_Error ftStatus = FT.FT_Init_FreeType(&freeTypeLibrary);

    if (ftStatus != FT_Error.FT_Err_Ok)
    {
        Console.WriteLine(ftStatus);
        Environment.Exit(1);
    }

    PrintFreeTypeVersion(freeTypeLibrary);

    FT_FaceRec_* face;
    ftStatus = FT.FT_New_Face(freeTypeLibrary, (byte*)Marshal.StringToHGlobalAnsi("SanRemo.ttf"), 0, &face);

    if (ftStatus != FT_Error.FT_Err_Ok)
    {
        Console.WriteLine(ftStatus);
        Environment.Exit(1);
    }

    try
    {
        using SvgSurface svg = new("test.svg", 400, 200);
        using PdfSurface pdf = new("test.pdf", 400, 200);
        using TeeSurface tee = new(svg);
        tee.Add(pdf);
        using CairoContext cr = new(tee);

        cr.Rectangle(0, 0, 400, 200);
        cr.Stroke();

        using FreeTypeFont freeTypeFont = new(new IntPtr(face), 0);
        cr.FontFace = freeTypeFont;
        cr.SetFontSize(28);

        const string Text = "Hello from CairoSharp";
        cr.MoveTo(10, 100);
        cr.ShowTextGlyphs(Text);

        tee.WriteToPng("test.png");
    }
    finally
    {
        ftStatus = FT.FT_Done_Face(face);

        if (ftStatus != FT_Error.FT_Err_Ok)
        {
            Console.WriteLine(ftStatus);
            Environment.Exit(1);
        }

        // https://freetype.org/freetype2/docs/reference/ft2-library_setup.html#ft_done_freetype
        // This call frees also all children, so the above call isn't needed.
        ftStatus = FT.FT_Done_FreeType(freeTypeLibrary);

        if (ftStatus != FT_Error.FT_Err_Ok)
        {
            Console.WriteLine(ftStatus);
            Environment.Exit(1);
        }
    }
}
//-----------------------------------------------------------------------------
static unsafe void PrintFreeTypeVersion(FT_LibraryRec_* freeTypeLibrary)
{
    int major, minor, patch;
    FT.FT_Library_Version(freeTypeLibrary, &major, &minor, &patch);

    Console.WriteLine($"FreeType version: {major}.{minor}.{patch}");
}
