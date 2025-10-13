// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;
using Cairo;
using Cairo.Drawing.Patterns;
using Cairo.Extensions;
using Cairo.Extensions.Loading;
using Cairo.Extensions.Loading.PDF;
using Cairo.Extensions.Loading.SVG;
using Cairo.Surfaces;
using Cairo.Surfaces.PostScript;
using Cairo.Surfaces.SVG;
using IOPath = System.IO.Path;

if (OperatingSystem.IsWindows())
{
    LoadingNative.DllImportResolver = static (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
    {
        string? path = libraryName switch
        {
            LoadingNative.LibRSvgName    => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "librsvg-2-2.dll"),
            LoadingNative.LibPopplerName => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libpoppler-glib-8.dll"),
            LoadingNative.LigGObjectName => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgobject-2.0-0.dll"),
            LoadingNative.LibGioName     => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgio-2.0-0.dll"),
            _                            => null
        };

        if (path is not null && NativeLibrary.TryLoad(path, out nint handle))
        {
            return handle;
        }

        return default;
    };
}
else
{
    // nothing to do on Linux :-)
}

if (Directory.Exists("output")) Directory.Delete("output", true);
Directory.CreateDirectory("output");
Environment.CurrentDirectory = IOPath.Combine(Environment.CurrentDirectory, "output");

PrintVersionInfos();

Svg2Png();
Pdf2Png();
//-----------------------------------------------------------------------------
static void PrintVersionInfos()
{
    Console.WriteLine($"Cairo version:   {CairoAPI.VersionString}");
    Console.WriteLine($"LibRsvg version: {LoadingNative.GetLibRsvgVersion()}");
    Console.WriteLine($"Poppler version: {LoadingNative.GetPopplerVersion()}");
    Console.WriteLine();
}
//-----------------------------------------------------------------------------
static void Svg2Png()
{
    Rectangle viewPort = new(0, 0, 500, 500);

    // Loading via file
    {
        using SvgSurface svgSurface = new("svg2png_0.svg", 500, 500);
        using CairoContext cr       = new(svgSurface);

        // Note: we set the current dir to output
        cr.LoadSvg("../demo02.svg", viewPort);

        svgSurface.WriteToPng("svg2png_0.png");
    }

    // Loading via byte array
    {
        using SvgSurface svgSurface = new("svg2png_1.svg", 500, 500);
        using CairoContext cr       = new(svgSurface);

        byte[] svgData = File.ReadAllBytes("../demo02.svg");
        cr.LoadSvg(svgData, viewPort);

        svgSurface.WriteToPng("svg2png_1.png");
    }

    // Playing around with separate surface for the logo (output is the same as below)
    {
        using SvgSurface svgSurface = new("svg2png_2.svg", 500, 500);
        using CairoContext cr       = new(svgSurface);

        cr.Rectangle(0, 0, 500, 500);
        cr.Stroke();

        using PostScriptSurface svgLogo    = new(50, 50);   // it's stack based similar to cairo's drawing model
        using (CairoContext svgLogoContext = new(svgLogo))
        {
            svgLogoContext.LoadSvg("../calculator-svgrepo-com.svg", new Rectangle(0, 0, 50, 50));
        }

        cr.SetSourceSurface(svgLogo, 0, 0);
        cr.Paint();

        cr.SetSourceSurface(svgLogo, 100, 50);
        cr.Paint();

        using (cr.Save())
        {
            cr.Translate(250, 250);
            cr.Rotate(45.DegreesToRadians());

            cr.Rectangle(-50, -50, 100, 100);
            cr.Color = KnownColors.Blue;
            cr.Stroke();

            cr.SetSourceSurface(svgLogo, 0, 0);
            cr.Paint();
        }

        svgSurface.WriteToPng("svg2png_2.png");
    }

    // Playing around with PushGroup / PopGroup for the logo (output is the same as above)
    {
        using SvgSurface svgSurface = new("svg2png_3.svg", 500, 500);
        using CairoContext cr       = new(svgSurface);

        cr.Rectangle(0, 0, 500, 500);
        cr.Stroke();

        cr.PushGroup();
        cr.LoadSvg("../calculator-svgrepo-com.svg", new Rectangle(0, 0, 50, 50));

        using SurfacePattern svgLogoPattern = cr.PopGroup();
        Surface svgLogo                     = svgLogoPattern.Surface;

        cr.SetSourceSurface(svgLogo, 0, 0);
        cr.Paint();

        cr.SetSourceSurface(svgLogo, 100, 50);
        cr.Paint();

        using (cr.Save())
        {
            cr.Translate(250, 250);
            cr.Rotate(45.DegreesToRadians());

            cr.Rectangle(-50, -50, 100, 100);
            cr.Color = KnownColors.Blue;
            cr.Stroke();

            cr.SetSourceSurface(svgLogo, 0, 0);
            cr.Paint();
        }

        svgSurface.WriteToPng("svg2png_3.png");
    }
}
//-----------------------------------------------------------------------------
static void Pdf2Png()
{
    // Loading via file
    {
        using SvgSurface svgSurface = new("pdf2png_0.svg", 500, 500);
        using CairoContext cr       = new(svgSurface);

        // Note: we set the current dir to output
        cr.LoadPdf("../demo02.pdf", 0);

        svgSurface.WriteToPng("pdf2png_0.png");
    }

    // Loading via byte array
    {
        using SvgSurface svgSurface = new("pdf2png_1.svg", 500, 500);
        using CairoContext cr       = new(svgSurface);

        byte[] pdfData = File.ReadAllBytes("../demo02.pdf");
        cr.LoadPdf(pdfData, 0);

        svgSurface.WriteToPng("pdf2png_1.png");
    }

    // Playing around is similar to the demo at SVG above.
}
