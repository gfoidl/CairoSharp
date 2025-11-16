// (c) gfoidl, all rights reserved

using System.Globalization;
using Cairo;
using Cairo.Extensions;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Colors.ColorMaps;
using Cairo.Surfaces;
using Cairo.Surfaces.Images;
using Cairo.Surfaces.Recording;
using IOPath = System.IO.Path;

if (Directory.Exists("lightness")) Directory.Delete("lightness", true);
Directory.CreateDirectory("lightness");
Environment.CurrentDirectory = IOPath.Combine(Environment.CurrentDirectory, "lightness");

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

foreach (ColorMap colorMap in GetColorMaps())
{
    Console.Write($"creating lightness plot for '{colorMap.Name}'...");
    {
        PlotLightnessOfColorMap(colorMap);
    }
    Console.WriteLine("done");
}
//-----------------------------------------------------------------------------
static IEnumerable<ColorMap> GetColorMaps()
{
    foreach (Type type in typeof(ColorMap).Assembly.GetTypes())
    {
        if (type.IsClass && type.IsSubclassOf(typeof(ColorMap)))
        {
            yield return (Activator.CreateInstance(type) as ColorMap)!;
        }
    }
}
//-----------------------------------------------------------------------------
static void PlotLightnessOfColorMap(ColorMap colorMap)
{
    using RecordingSurface recordingSurface = colorMap.PlotLightnessCharacteristics();
    Rectangle inkExtents                    = recordingSurface.GetInkExtents();
    using ImageSurface finalSurface         = new(Format.Argb32, 2 * (int)inkExtents.Width, 2 * (int)inkExtents.Height);
    using CairoContext cr                   = new(finalSurface);

    cr.Scale(2, 2);
    cr.Antialias = Antialias.Best;

    // White background
    cr.Color = KnownColors.White;
    cr.Paint();

    // Need to set at (-x, -y)
    cr.SetSourceSurface(recordingSurface, -inkExtents.X, -inkExtents.Y);
    cr.Paint();

    finalSurface.WriteToPng($"{colorMap.Name}_lightness.png");
}
