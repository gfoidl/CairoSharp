// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Cairo;
using Cairo.Drawing;
using Cairo.Drawing.Patterns;
using Cairo.Drawing.TagsAndLinks;
using Cairo.Drawing.Text;
using Cairo.Fonts;
using Cairo.Fonts.Scaled;
using Cairo.Fonts.User;
using Cairo.Surfaces;
using Cairo.Surfaces.Images;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.PostScript;
using Cairo.Surfaces.Recording;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Tee;
using Cairo.Surfaces.Win32;
using CairoDemo;
using IOPath = System.IO.Path;

// When enabled the RasterSource demo will fail. But actually it's OK, just
// the simple approach for DebugDispose can't handle this, as the image is
// destroyed in RasterSource.csL291 by a native call.
//AppContext.SetSwitch("Cairo.DebugDispose", true);

if (Directory.Exists("output")) Directory.Delete("output", true);
Directory.CreateDirectory("output");
Environment.CurrentDirectory = IOPath.Combine(Environment.CurrentDirectory, "output");

try
{
    PrintCairoInfo();
    PrintSupportedSurfaceVersions();
    ShowSurfaceInformation();

    Primitives();
    AntiAlias();
    Mask();
    Demo01();
    Demo02();
    Arrow();
    Hexagon();
    Gradient();
    MeshPattern();
    MeshPattern1();
    RecordingAndScriptSurface();
    PdfFeatures();
    RasterSource();
    UserFontSimple();
    PngFlatten();
}
catch (Exception ex) when (!Debugger.IsAttached)
{
    Console.WriteLine(ex);
}

if (Debugger.IsAttached)
{
    Console.WriteLine("\nEnd.");
    Console.ReadKey();
}
//-----------------------------------------------------------------------------
static void PrintCairoInfo()
{
    Console.WriteLine($"Cairo version: {CairoAPI.VersionString}");
    Console.WriteLine();
}
//-----------------------------------------------------------------------------
static void PrintSupportedSurfaceVersions()
{
    Console.WriteLine("Supported PDF versions:");

    foreach (PdfVersion pdfVersion in PdfSurface.GetSupportedVersions())
    {
        Console.WriteLine($"\t{pdfVersion.GetString()}");
    }

    Console.WriteLine("Supported SVG versions");

    foreach (SvgVersion svgVersion in SvgSurface.GetSupportedVersions())
    {
        Console.WriteLine($"\t{svgVersion.GetString()}");
    }

    Console.WriteLine();
}
//-----------------------------------------------------------------------------
static void ShowSurfaceInformation()
{
    Console.WriteLine("Surfaces:");

    using (ImageSurface surface = new(Format.Argb32, 100, 100))
    {
        Print(surface);
    }

    using (PdfSurface surface = new(100, 100))
    {
        Print(surface);
    }

    using (PostScriptSurface surface = new(100, 100))
    {
        Print(surface);
    }

    using (SvgSurface surface = new(100, 100))
    {
        Print(surface);
    }

    if (OperatingSystem.IsWindows())
    {
        using (Win32Surface surface = new(Format.Argb32, 100, 100))
        {
            Print(surface);
        }
    }

    static void Print(Surface surface)
    {
        Console.WriteLine($"""
                {surface.GetType().Name,-20}
                    HasShowTextGlyphs:      {surface.HasShowTextGlyphs}
                    supports mime-type png: {surface.SupportsMimeType(MimeTypes.Png)}
                    supports mime-type jpg: {surface.SupportsMimeType(MimeTypes.Jpeg)}
                    supports mime-type uri: {surface.SupportsMimeType(MimeTypes.Uri)}
            """);
    }
}
//-----------------------------------------------------------------------------
static void Primitives()
{
    static void Draw(Surface surface)
    {
        using CairoContext c = new(surface);
        c.Scale(4, 4);

        // Stroke:
        c.LineWidth = 0.1;
        c.Color = new Color(0, 0, 0);
        c.Rectangle(10, 10, 10, 10);
        c.Stroke();

        using (c.Save())
        {
            c.Color = new Color(0, 0, 0);
            c.Translate(20, 5);
            c.MoveTo(0, 0);
            c.LineTo(10, 5);
            c.Stroke();
        }

        // Fill:
        c.Color = new Color(0, 0, 0);
        c.SetSourceRgb(0, 0, 0);
        c.Rectangle(10, 30, 10, 10);
        c.Fill();

        // Text:
        c.Color = new Color(0, 0, 0);
        c.SelectFontFace("Georgia", FontSlant.Normal, FontWeight.Bold);
        c.SetFontSize(10);      // the default anyway
        c.TextExtents("a", out TextExtents te);
        c.MoveTo(
            0.5 - te.Width / 2 - te.XBearing + 10,
            0.5 - te.Height / 2 - te.YBearing + 50);
        c.ShowText("a");

        c.Color = new Color(0, 0, 0);
        c.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Bold);
        c.SetFontSize(10);      // the default anyway
        c.TextExtents("a", out te);
        c.MoveTo(
            0.5 - te.Width / 2 - te.XBearing + 10,
            0.5 - te.Height / 2 - te.YBearing + 60);
        c.ShowText("a");

        c.Color = new Color(0, 0, 1);
        c.TextExtents("b"u8, out te);
        c.MoveTo(
            0.5 - te.Width / 2 - te.XBearing + 10,
            0.5 - te.Height / 2 - te.YBearing + 70);
        c.ShowText("b"u8);
    }

    using (Surface surface = new ImageSurface(Format.Argb32, 200, 320))
    {
        Draw(surface);
        surface.WriteToPng("primitives.png");
    }

    using (Surface surface = new PdfSurface("primitives.pdf", 200, 320))
    {
        Draw(surface);

        // Another page (with the same content...)
        surface.ShowPage();
        Draw(surface);
    }

    using (Surface surface = new SvgSurface("primitives.svg", 200, 320))
    {
        Draw(surface);
    }
}
//-----------------------------------------------------------------------------
static void AntiAlias()
{
    static void Draw(Surface surface)
    {
        using CairoContext ctx = new(surface);
        // Sets the anti-aliasing method:
        ctx.Antialias = Antialias.Subpixel;

        // Sets the line width:
        ctx.LineWidth = 9;

        // red, green, blue, alpha:
        ctx.Color = new Color(0, 0, 0, 1);

        // Sets the Context's start point:
        ctx.MoveTo(10, 10);

        // Draws a "virtual" line:
        ctx.LineTo(40, 60);

        // Stroke the line to the image surface:
        ctx.Stroke();

        ctx.Antialias = Antialias.Gray;
        ctx.LineWidth = 8;
        ctx.Color = new Color(1, 0, 0, 1);
        ctx.LineCap = LineCap.Round;
        ctx.MoveTo(10, 50);
        ctx.LineTo(40, 100);
        ctx.Stroke();

        // Fastest method but low quality:
        ctx.Antialias = Antialias.None;
        ctx.LineWidth = 7;
        ctx.MoveTo(10, 90);
        ctx.LineTo(40, 140);
        ctx.Stroke();
    }

    using (Surface surface = new ImageSurface(Format.Argb32, 70, 150))
    {
        Draw(surface);

        // Save the image as a png image:
        surface.WriteToPng("antialias.png");
    }

    using (Surface surface = new PdfSurface("antialias.pdf", 70, 150))
    {
        Draw(surface);
    }

    using (Surface surface = new SvgSurface("antialias.svg", 70, 150))
    {
        Draw(surface);
    }
}
//-----------------------------------------------------------------------------
static void Mask()
{
    static void Draw(Surface surface, bool hasMultiplePages = false)
    {
        using CairoContext ctx = new(surface);
        ctx.Scale(500, 500);

        using Gradient linpat = new LinearGradient(0, 0, 1, 1);
        linpat.AddColorStop(0, new Color(0, 0.3, 0.8));
        linpat.AddColorStop(1, new Color(1, 0.8, 0.3));

        using Gradient radpat = new RadialGradient(0.5, 0.5, 0.25, 0.5, 0.5, 0.6);
        radpat.AddColorStop(0, new Color(0, 0, 0, 1));
        radpat.AddColorStop(1, new Color(0, 0, 0, 0));

        ctx.SetSource(linpat);

        if (!hasMultiplePages)
        {
            ctx.Mask(radpat);
        }
        else
        {
            ctx.Mask(radpat);

            ctx.ShowPage();
            ctx.Paint();

            ctx.ShowPage();
            ctx.SetSource(radpat);
            ctx.Paint();
        }
    }

    using (Surface surface = new ImageSurface(Format.Argb32, 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("mask.png");
    }

    using (Surface surface = new PdfSurface("mask.pdf", 500, 500))
    {
        Draw(surface, hasMultiplePages: true);
    }

    using (Surface surface = new SvgSurface("mask.svg", 500, 500))
    {
        Draw(surface);
    }
}
//-----------------------------------------------------------------------------
static void Demo01()
{
    static void Draw(Surface surface)
    {
        using CairoContext c = new(surface);
        c.Scale(500, 500);

        c.Color = new Color(0, 0, 0);
        c.MoveTo(0, 0);         // absetzen und neu beginnen
        c.LineTo(1, 1);
        c.MoveTo(1, 0);
        c.LineTo(0, 1);
        c.LineWidth = 0.2;
        c.Stroke();             // Lininen zeichnen

        c.Rectangle(0, 0, 0.5, 0.5);
        c.Color = new Color(1, 0, 0, 0.8);
        c.Fill();

        c.Rectangle(0, 0.5, 0.5, 0.5);
        c.Color = new Color(0, 1, 0, 0.6);
        c.Fill();

        c.Rectangle(0.5, 0, 0.5, 0.5);
        c.Color = new Color(0, 0, 0, 0.4);
        c.Fill();
    }

    using (Surface surface = new ImageSurface(Format.Argb32, 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("demo01.png");
    }

    using (Surface surface = new PdfSurface("demo01.pdf", 500, 500))
    {
        Draw(surface);
    }

    using (Surface surface = new SvgSurface("demo01.svg", 500, 500))
    {
        Draw(surface);
    }
}
//-----------------------------------------------------------------------------
static void Demo02()
{
    static void Draw(Surface surface)
    {
        using CairoContext c = new(surface);
        c.Scale(500, 500);

        using Gradient radpat = new RadialGradient(0.25, 0.25, 0.1, 0.5, 0.5, 0.5);
        radpat.AddColorStop(0, new Color(1.0, 0.8, 0.8));
        radpat.AddColorStop(1, new Color(0.9, 0.0, 0.0));

        for (int i = 1; i < 10; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                c.Rectangle(i / 10d - 0.04, j / 10d - 0.04, 0.08, 0.08);
            }
        }

        c.SetSource(radpat);
        c.Fill();

        using Gradient linpat = new LinearGradient(0.25, 0.35, 0.75, 0.65);
        linpat.AddColorStop(0.00, new Color(1, 1, 1, 0));
        linpat.AddColorStop(0.25, new Color(0, 1, 0, 0.5));
        linpat.AddColorStop(0.50, new Color(1, 1, 1, 0));
        linpat.AddColorStop(0.75, new Color(0, 0, 1, 0.5));
        linpat.AddColorStop(1.00, new Color(1, 1, 1, 0));

        c.Rectangle(0, 0, 1, 1);
        c.SetSource(linpat);
        c.Fill();
    }

    using (Surface surface = new ImageSurface(Format.Argb32, 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("demo02.png");
    }

    using (Surface surface = new PdfSurface("demo02.pdf", 500, 500))
    {
        Draw(surface);
    }

    using (Surface surface = new SvgSurface("demo02.svg", 500, 500))
    {
        Draw(surface);
    }
}
//-----------------------------------------------------------------------------
static void Arrow()
{
    static void Draw(Surface surface)
    {
        using CairoContext c = new(surface);
        c.Scale(500, 500);

        // Only relevant for PNG
        c.Antialias = Antialias.Subpixel;

        // line width because of scale
        double ux = 1, uy = 1;
        c.DeviceToUserDistance(ref ux, ref uy);
        c.LineWidth = Math.Max(ux, uy);

        c.Color = new Color(0, 0, 1);
        c.MoveTo(0.1, 0.10);
        c.LineTo(0.9, 0.45);
        c.Stroke();

        c.Arrow(0.1, 0.50, 0.9, 0.95, 0.05, 10);
        c.Stroke();
    }

    using (Surface surface = new ImageSurface(Format.Argb32, 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("arrow.png");
    }

    using (Surface surface = new PdfSurface("arrow.pdf", 500, 500))
    {
        Draw(surface);
    }

    using (Surface surface = new SvgSurface("arrow.svg", 500, 500))
    {
        Draw(surface);
    }
}
//-----------------------------------------------------------------------------
static void Hexagon()
{
    static PointD[] GetHexagonPoints(double cellSize)
    {
        double ri = cellSize / 2;
        double r = 2 * ri / Math.Sqrt(3);

        var p1 = new PointD(0, r);
        var p2 = new PointD(ri, r / 2);
        var p3 = new PointD(ri, -r / 2);
        var p4 = new PointD(0, -r);
        var p5 = new PointD(-ri, -r / 2);
        var p6 = new PointD(-ri, r / 2);

        return [p1, p2, p3, p4, p5, p6];
    }

    static void Draw(Surface surface)
    {
        using CairoContext c = new(surface);
        c.Scale(500, 500);

        // Hat nur für PNG Relevanz:
        c.Antialias = Antialias.Subpixel;

        // Linienweite, wegen Maßstab so:
        double ux = 1, uy = 1;
        c.DeviceToUserDistance(ref ux, ref uy);
        c.LineWidth = Math.Max(ux, uy);

        PointD[] hexagon = GetHexagonPoints(0.5);
        using (c.Save())
        {
            c.Translate(0.5, 0.5);
            c.MoveTo(hexagon[0]);
            c.LineTo(hexagon[1]);
            c.LineTo(hexagon[2]);
            c.LineTo(hexagon[3]);
            c.LineTo(hexagon[4]);
            c.LineTo(hexagon[5]);
            c.ClosePath();
            c.Stroke();
        }

        c.Color = new Color(0, 0, 1);
        ux = 0.1; uy = 0.1;
        c.DeviceToUserDistance(ref ux, ref uy);
        c.LineWidth = Math.Max(ux, uy);

        c.MoveTo(0.5, 0);
        c.LineTo(0.5, 1);
        c.MoveTo(0, 0.5);
        c.LineTo(1, 0.5);
        c.Stroke();
    }

    using (Surface surface = new ImageSurface(Format.Argb32, 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("hexagon.png");
    }

    using (Surface surface = new PdfSurface("hexagon.pdf", 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("hexagon1.png");
    }

    using (Surface surface = new SvgSurface("hexagon.svg", 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("hexagon2.png");
    }
}
//-----------------------------------------------------------------------------
static void Gradient()
{
    static void Draw(Surface surface)
    {
        using CairoContext c = new(surface);

        using (Gradient pat = new LinearGradient(0.0, 0.0, 0.0, 256.0))
        {
            pat.AddColorStopRgba(1, 0, 0, 0, 1);
            pat.AddColorStopRgba(0, 1, 1, 1, 1);
            c.Rectangle(0, 0, 256, 256);
            c.SetSource(pat);
            c.Fill();
        }

        using (Gradient pat = new RadialGradient(115.2, 102.4, 25.6,
                                                 102.4, 102.4, 128.0))
        {
            pat.AddColorStopRgba(0, 1, 1, 1, 1);
            pat.AddColorStopRgba(1, 0, 0, 0, 1);
            c.SetSource(pat);
            c.Arc(128.0, 128.0, 76.8, 0, 2 * Math.PI);
            c.Fill();
        }
    }

    using (Surface surface = new ImageSurface(Format.Argb32, 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("gradient.png");
    }

    using (Surface surface = new PdfSurface("gradient.pdf", 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("gradient1.png");
    }

    using (Surface surface = new SvgSurface("gradient.svg", 500, 500))
    {
        Draw(surface);
        surface.WriteToPng("gradient2.png");
    }
}
//-----------------------------------------------------------------------------
static void MeshPattern()
{
    // Based on https://gist.github.com/mgdm/3159434

    static void Draw(Surface surface)
    {
        using CairoContext c = new(surface);
        using Mesh mesh      = new();

        // Use the mesh pattern methods to create a rectangle (called a "patch").
        // This rectangle will be the same size as the image
        using (mesh.BeginPatch())
        {
            mesh.LineTo(0, 0);      // corner 0
            mesh.LineTo(400, 0);    // corner 1
            mesh.LineTo(400, 300);  // corner 2
            mesh.LineTo(0, 300);    // corner 3
            mesh.LineTo(0, 0);      // back to corner 0

            mesh.SetCornerColorRgb(0, 0, 0, 0);     // corner 0 - black
            mesh.SetCornerColorRgb(1, 1, 0, 0);     // corner 1 - red
            mesh.SetCornerColorRgb(2, 0, 1, 0);     // corner 2 - green
            mesh.SetCornerColorRgb(3, 0, 0, 1);     // corner 3 - blue
        }

        c.SetSource(mesh);

        // Draw a rectangle and fill it
        c.Rectangle(0, 0, 400, 300);
        c.Fill();
    }

    using Surface primarySurface = new PdfSurface("mesh.pdf", 400, 300);
    using Surface svgSurface     = new SvgSurface("mesh.svg", 400, 300);
    using Surface imageSurface   = new ImageSurface(Format.Argb32, 400, 300);
    using TeeSurface teeSurface  = new(primarySurface);

    teeSurface.Add(svgSurface);
    teeSurface.Add(imageSurface);

    Draw(teeSurface);
    imageSurface.WriteToPng("mesh.png");
}
//-----------------------------------------------------------------------------
static void MeshPattern1()
{
    // Based on https://gitlab.com/saiwp/cairo/-/blob/master/test/mesh-pattern.c?ref_type=heads

    const int PatWidth = 170;
    const int Size     = PatWidth;
    const int Pad      = 2;
    const int Width    = Pad + Size + Pad;
    const int Height   = Width;

    static void Draw(Surface surface)
    {
        using CairoContext cr = new(surface);

        cr.Translate(Pad, Pad);
        cr.Translate(10, 10);

        using Mesh mesh = new();

        // Add a Coons patch
        using (mesh.BeginPatch())
        {
            mesh.MoveTo(0, 0);
            mesh.CurveTo(30, -30, 60, 30, 100, 0);
            mesh.CurveTo(60, 30, 130, 60, 100, 100);
            mesh.CurveTo(60, 70, 30, 130, 0, 100);
            mesh.CurveTo(30, 70, -30, 30, 0, 0);

            mesh.SetCornerColorRgb(0, 1, 0, 0);
            mesh.SetCornerColorRgb(1, 0, 1, 0);
            mesh.SetCornerColorRgb(2, 0, 0, 1);
            mesh.SetCornerColorRgb(3, 1, 1, 0);
        }

        using (mesh.BeginPatch())
        {
            mesh.MoveTo(50, 50);

            mesh.CurveTo(80, 20, 110, 80, 150, 50);
            mesh.CurveTo(110, 80, 180, 110, 150, 150);
            mesh.CurveTo(110, 120, 80, 180, 50, 150);
            mesh.CurveTo(80, 120, 20, 80, 50, 50);

            mesh.SetCornerColorRgba(0, 1, 0, 0, 0.3);
            mesh.SetCornerColorRgb(1, 0, 1, 0);
            mesh.SetCornerColorRgba(2, 0, 0, 1, 0.3);
            mesh.SetCornerColorRgb(3, 1, 1, 0);
        }

        cr.SetSource(mesh);
        cr.Paint();
    }

    using (Surface surface = new ImageSurface(Format.Argb32, Width, Height))
    {
        Draw(surface);
        surface.WriteToPng("mesh1.png");
    }

    using (Surface surface = new PdfSurface("mesh1.pdf", Width, Height))
    {
        Draw(surface);
    }

    using (Surface surface = new SvgSurface("mesh1.svg", Width, Height))
    {
        Draw(surface);
    }
}
//-----------------------------------------------------------------------------
static void RecordingAndScriptSurface()
{
    // Record a script
    {
        using ScriptDevice script   = new("script0.cairo");
        using ScriptSurface surface = new(script, Content.Color, 300, 300);
        using CairoContext context  = new(surface);

        script.Mode = ScriptMode.Ascii;

        script.WriteComment($"Start at {DateTimeOffset.Now}");

        context.Color = new Color(0, 0, 1);
        context.Rectangle(10, 20, 40, 30);
        context.FillPreserve();

        context.LineWidth = 3;      // 2 is the default, so wouldn't show up in the script
        context.Color     = new Color(1, 0, 0);
        context.Stroke();

        // This does not work for the script surface
        //surface.WriteToPng("script.png");
        script.WriteComment($"End at {DateTimeOffset.Now}");
    }

    // Recording surface
    {
        using RecordingSurface recording = new(Content.Color);  // size is infinite
        using CairoContext context       = new(recording);

        // White background, as the default color is black.
        context.Color = new Color(1, 1, 1);
        context.Paint();

        context.Rectangle(50, 50, 200, 100);
        context.Color = new(0.8, 0.8, 0.8);
        context.FillPreserve();
        context.Color = Color.Default;
        context.Stroke();

        using (ScriptDevice script = new("script1.cairo"))
        {
            script.FromRecordingSurface(recording);
        }

        using SvgSurface svg  = new("recording.svg", 300, 300);
        using CairoContext cr = new(svg);

        cr.SetSourceSurface(recording, 0, 0);
        cr.Paint();
        svg.WriteToPng("recording.png");
    }

    // Proxy surface
    {
        using SvgSurface svg        = new("script2.svg", 300, 300);
        using ScriptDevice script   = new("script2.cairo");
        using ScriptSurface surface = new(script, svg);
        using CairoContext context  = new(surface);

        context.Rectangle(50, 50, 200, 100);
        context.Color = new(0.8, 0.8, 0.8);
        context.FillPreserve();
        context.Color = Color.Default;
        context.Stroke();
    }
}
//-----------------------------------------------------------------------------
static void PdfFeatures()
{
    using PdfSurface surface = new("pdf-features.pdf", PageSize.A4.WidthInPoints, PageSize.A4.HeightInPoints);
    surface.RestrictToVersion(PdfVersion.Version1_7);

    using CairoContext context = new(surface);

    int outlineId = surface.AddOutline(PdfSurface.PdfOutlineRoot, "Test Outline (link to cairo)", "uri='https://cairographics.org'", PdfOutlineFlags.Open);

    surface.SetMetadata(PdfMetadata.Author, "gfoidl");
    surface.SetMetadata(PdfMetadata.Subject, "cairo");

    if (CairoAPI.Version > CairoAPI.VersionEncode(1, 18, 0))
    {
        surface.SetCustomMetadata("CairoSharp version", "2.0.0");
        surface.SetThumbnailSize((int)PageSize.A4.WidthInPoints / 20, (int)PageSize.A4.HeightInPoints / 20);
    }

    // Page 1
    {
        context.Rectangle(10, 10, 100, 200);
        context.StrokePreserve();
        context.Color = new Color(0, 1, 0);
        context.Fill();

        // Hyperlink
        using (context.TagBegin(CairoTagConstants.CairoTagLink, "uri='https://github.com/gfoidl/CairoSharp'"))
        {
            context.Color = Color.Default;
            context.MoveTo(200, 50);
            context.ShowText("This is a link to the source repository of CairoSharp");
        }
    }

    // Page 2
    {
        context.ShowPage();
        surface.SetSize(PageSize.A4Landscape.WidthInPoints, PageSize.A4.HeightInPoints);

        context.Color = Color.Default;
        context.Rectangle(10, 10, 200, 100);
        context.StrokePreserve();
        context.Color = new Color(0, 0, 1);
        context.Fill();

        // Hyperlink
        using (context.TagBegin(CairoTagConstants.CairoTagLink, "uri='https://github.com/gfoidl/CairoSharp'"))
        {
            context.Color = Color.Default;
            context.MoveTo(300, 50);

            Span<byte> buffer = stackalloc byte[128];
            Random.Shared.NextBytes(buffer);    // just to poison it
            int written       = Encoding.UTF8.GetBytes("This is a link to the source repository of CairoSharp", buffer);
            buffer            = buffer[..written];
            context.ShowText(buffer);
        }

        surface.SetPageLabel("my page 2");
    }
}
//-----------------------------------------------------------------------------
static void RasterSource()
{
    // Based on https://gitlab.com/saiwp/cairo/-/blob/master/test/raster-source.c?ref_type=heads

    // Note: we set the current dir to output
    PngDimensions("../png.png", out Content content, out int pngWidth, out int pngHeight);

    using Pattern png = new RasterSource("../png.png", content, pngWidth, pngHeight, static (pattern, userData, target, ref extents) =>
    {
        string pngFile = (userData as string)!;
        return new ImageSurface(pngFile);
    });

    using Pattern red = new RasterSource(null, Content.Color, pngWidth, pngHeight, static (pattern, userData, target, ref extents) =>
    {
        Surface image      = target.CreateSimilarImage(Format.Rgb24, extents.Width, extents.Height);
        image.DeviceOffset = new PointD(extents.X, extents.Y);

        using CairoContext cr = new(image);
        cr.SetSourceRgb(1, 0, 0);
        cr.Paint();

        return image;
    });

    const int Width  = 200;
    const int Height = 80;

    using SvgSurface svgSurface     = new("raster-source.svg", Width, Height);
    using PdfSurface pdfSurface     = new("raster-source.pdf", Width, Height);
    using ImageSurface imageSurface = new(Format.Argb32, Width, Height);
    using TeeSurface teeSurface     = new(svgSurface);
    using CairoContext context      = new(teeSurface);

    teeSurface.Add(pdfSurface);
    teeSurface.Add(imageSurface);

    context.SetSourceRgb(0, 0, 1);
    context.Paint();

    context.Translate(0, (Height - pngHeight) / 2);

    for (int i = 0; i < 4; ++i)
    {
        for (int j = 0; j < 4; ++j)
        {
            Pattern source = ((i ^ j) & 1) != 1 ? red : png;
            context.SetSource(source);

            context.Rectangle(i * Width / 4, j * pngHeight / 4, Width / 4, pngHeight / 4);
            context.Fill();
        }
    }

    imageSurface.WriteToPng("raster-source.png");

    // Lazy way of determining PNG dimensions...
    static void PngDimensions(string pngFile, out Content content, out int width, out int height)
    {
        using ImageSurface surface = new(pngFile);

        content = surface.Content;
        width   = surface.Width;
        height  = surface.Height;
    }
}
//-----------------------------------------------------------------------------
static void UserFontSimple()
{
    // Well, this is a really very simple "font". Just for demo how it works.

    static void Draw(Surface surface)
    {
        using UserFont userFont = new(static (ScaledFont sf, CairoContext cr, ref FontExtents fontExtents) =>
        {
            cr.FontExtents(out fontExtents);
            return Status.Success;

        }, static (ScaledFont sf, int glyph, CairoContext cr, ref TextExtents textExtents) =>
        {
            switch (glyph)
            {
                case 'a':
                {
                    cr.MoveTo(0, 10);
                    cr.LineTo(5, 0);
                    cr.LineTo(10, 10);
                    cr.ClosePath();
                    break;
                }
                case 'b':
                {
                    cr.Arc(5, 5, 5, 0, 2 * Math.PI);
                    break;
                }
                default:
                {
                    cr.Rectangle(0, 0, 10, 10);
                    break;
                }
            }

            cr.Fill();

            return Status.Success;
        });

        using CairoContext cr = new(surface);

        cr.Rectangle(0, 0, 400, 150);
        cr.Stroke();

        cr.FontFace = userFont;
        cr.MoveTo(10, 10);

#if SHOW_TEXT
        cr.ShowText("abx");
#else
        cr.TextPath("abx");

        using LinearGradient gradient = new(0, 0, 400, 150);
        gradient.AddColorStopRgb(0.0, 1, 0, 0);
        gradient.AddColorStopRgb(0.5, 0, 1, 0);
        gradient.AddColorStopRgb(1.0, 0, 0, 1);

        cr.SetSource(gradient);
        cr.FillPreserve();

        cr.Color = Color.Default;
        cr.Stroke();
#endif
    }

    using Surface primarySurface = new PdfSurface("user-font.pdf", 400, 150);
    using Surface svgSurface     = new SvgSurface("user-font.svg", 400, 150);
    using Surface imageSurface   = new ImageSurface(Format.Argb32, 400, 150);
    using TeeSurface teeSurface  = new(primarySurface);

    teeSurface.Add(svgSurface);
    teeSurface.Add(imageSurface);

    Draw(teeSurface);
    imageSurface.WriteToPng("user-font.png");
}
//-----------------------------------------------------------------------------
static void PngFlatten()
{
    // Make a transparent PNG opaque
    // Based on https://gitlab.freedesktop.org/cairo/cairo/-/blob/master/test/png-flatten.c?ref_type=heads

    // Note: we set the current dir to output
    using ImageSurface argb  = new("../png-transparent.png");
    using ImageSurface rgb24 = new(Format.Rgb24, argb.Width, argb.Height);
    using CairoContext cr    = new(rgb24);

    // white for the background
    cr.SetSourceRgb(1, 1, 1);
    cr.Paint();

    cr.SetSourceSurface(argb, 0, 0);
    cr.Paint();

    rgb24.WriteToPng("png-opaque.png");
}
//-----------------------------------------------------------------------------
namespace CairoDemo
{
    public static class ContextExtensions
    {
        public static void Arrow(
            this CairoContext c,
            double x0, double y0,
            double x1, double y1,
            double length, double angle)
        {
            // Linie zeichnen:
            c.MoveTo(x0, y0);
            c.LineTo(x1, y1);
            c.Stroke();

            // Pfeil zeichnen:
            angle *= Math.PI / 180d;
            double phi = Math.Atan2(y1 - y0, x0 - x1);
            double xx1 = x1 + length * Math.Cos(phi - angle);
            double yy1 = y1 + length * Math.Sin(phi - angle);
            double xx2 = x1 + length * Math.Cos(phi + angle);
            double yy2 = y1 + length * Math.Sin(phi + angle);

            c.MoveTo(x1, y1);
            c.LineTo(xx1, yy1);
            c.LineTo(xx2, yy2);
            c.ClosePath();
            c.Fill();
        }
    }
}
