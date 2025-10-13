// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;
using Cairo;
using Cairo.Extensions;
using Cairo.Extensions.Loading.SVG;
using Cairo.Extensions.Shapes;
using Cairo.Surfaces.SVG;
using IOPath = System.IO.Path;

if (OperatingSystem.IsWindows())
{
    LibRSvgNative.DllImportResolver = static (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
    {
        string? path = null;

        if (libraryName == LibRSvgNative.LibRSvgName)
        {
            path = IOPath.Combine(@"C:\Program Files\Inkscape\bin", "librsvg-2-2.dll");
        }
        else if (libraryName == LibRSvgNative.LibGioName)
        {
            path = IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgio-2.0-0.dll");
        }
        else if (libraryName == LibRSvgNative.LigGObjectName)
        {
            path = IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgobject-2.0-0.dll");
        }

        if (path is not null && NativeLibrary.TryLoad(path, out nint handle))
        {
            return handle;
        }

        return default;
    };
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
    Console.WriteLine($"Cairo version: {CairoAPI.VersionString}");
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
        byte[] svgData = File.ReadAllBytes("../demo02.svg");

        using SvgSurface svgSurface = new("svg2png_1.svg", 500, 500);
        using CairoContext cr       = new(svgSurface);

        cr.LoadSvg(svgData, viewPort);
        svgSurface.WriteToPng("svg2png_1.png");
    }
}
//-----------------------------------------------------------------------------
static void Pdf2Png()
{

}
