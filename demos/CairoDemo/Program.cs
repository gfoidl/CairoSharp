// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo;
using Cairo.Drawing;
using Cairo.Drawing.Patterns;
using Cairo.Drawing.TagsAndLinks;
using Cairo.Drawing.Text;
using Cairo.Fonts;
using Cairo.Surfaces;
using Cairo.Surfaces.Images;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.Recording;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Tee;
using CairoDemo;
using IOPath = System.IO.Path;

AppContext.SetSwitch("Cairo.DebugDispose", true);

if (Directory.Exists("output")) Directory.Delete("output", true);
Directory.CreateDirectory("output");
Environment.CurrentDirectory = IOPath.Combine(Environment.CurrentDirectory, "output");

PrintCairoInfo();
PrintSupportedSurfaceVersions();

try
{
    Primitives();
    AntiAlias();
    Mask();
    Demo01();
    Demo02();
    Arrow();
    Hexagon();
    Gradient();
    MeshPattern();
    RecordingAndScriptSurface();
    PdfFeatures();
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
        c.FontSize = 10;
        c.TextExtents("a", out TextExtents te);
        c.MoveTo(
            0.5 - te.Width / 2 - te.XBearing + 10,
            0.5 - te.Height / 2 - te.YBearing + 50);
        c.ShowText("a");

        c.Color = new Color(0, 0, 0);
        c.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Bold);
        c.FontSize = 10;
        c.TextExtents("a", out te);
        c.MoveTo(
            0.5 - te.Width / 2 - te.XBearing + 10,
            0.5 - te.Height / 2 - te.YBearing + 60);
        c.ShowText("a");
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

        ctx.Source = linpat;

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
            ctx.Source = radpat;
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

        c.Source = radpat;
        c.Fill();

        using Gradient linpat = new LinearGradient(0.25, 0.35, 0.75, 0.65);
        linpat.AddColorStop(0.00, new Color(1, 1, 1, 0));
        linpat.AddColorStop(0.25, new Color(0, 1, 0, 0.5));
        linpat.AddColorStop(0.50, new Color(1, 1, 1, 0));
        linpat.AddColorStop(0.75, new Color(0, 0, 1, 0.5));
        linpat.AddColorStop(1.00, new Color(1, 1, 1, 0));

        c.Rectangle(0, 0, 1, 1);
        c.Source = linpat;
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
static void RecordingAndScriptSurface()
{
    using RecordingSurface recordingSurface = new(Content.Color);
    using CairoContext context              = new(recordingSurface);
    using ScriptSurface script              = new("script.cairo");

    script.Mode = ScriptMode.Ascii;
    script.FromRecordingSurface(recordingSurface);
    script.WriteComment($"Start at {DateTimeOffset.Now}");

    context.Color = new Color(0, 0, 1);
    context.Rectangle(10, 20, 40, 30);
    context.FillPreserve();

    context.LineWidth = 2;
    context.Color = new Color(1, 0, 0);
    context.Stroke();

    using (ImageSurface img = new(Format.Argb32, 300, 300))
    using (CairoContext cr  = new(img))
    {
        cr.SetSourceSurface(recordingSurface, 0, 0);
        cr.Paint();

        img.WriteToPng("recording.png");
    }

    script.WriteComment($"End at {DateTimeOffset.Now}");
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

    surface.SetCustomMetadata("CairoSharp version", "2.0.0");
    surface.SetThumbnailSize((int)PageSize.A4.WidthInPoints / 20, (int)PageSize.A4.HeightInPoints / 20);

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

        surface.SetPageLabel("my page 2");
    }
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
