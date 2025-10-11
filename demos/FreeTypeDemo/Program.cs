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
    using FreeTypeLibrary freeTypeLibrary = new();
    PrintFreeTypeVersion(freeTypeLibrary);

    FT_FaceRec_* face;
    FT_Error ftStatus = FT.FT_New_Face(freeTypeLibrary.Native, (byte*)Marshal.StringToHGlobalAnsi("SanRemo.ttf"), 0, &face);

    if (ftStatus != FT_Error.FT_Err_Ok)
    {
        Console.WriteLine(ftStatus);
        return;
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
        FT.FT_Done_Face(face);
    }
}
//-----------------------------------------------------------------------------
static unsafe void PrintFreeTypeVersion(FreeTypeLibrary freeTypeLibrary)
{
    int major, minor, patch;
    FT.FT_Library_Version(freeTypeLibrary.Native, &major, &minor, &patch);

    Console.WriteLine($"FreeType version: {major}.{minor}.{patch}");
}
