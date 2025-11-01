// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Drawing;
using GdiColor = System.Drawing.Color;

namespace CreateColorMaps;

internal sealed class DefaultBitmapColorMapGenerator(string inputFile, string name, string? comment = null)
    : ColorMapGenerator(name, comment)
{
    private readonly string _inputFile = inputFile;

    protected override string Type => "Default";
    //-------------------------------------------------------------------------
    protected override IEnumerable<(double Red, double Green, double Blue)> GetColors()
    {
        // palette is a 1px height bitmap with the colors for the color map.
        using Bitmap? palette = Image.FromFile(_inputFile) as Bitmap;
        Debug.Assert(palette is not null);

        for (int i = 0; i < palette.Width; ++i)
        {
            GdiColor gdiColor = palette.GetPixel(palette.Width - 1 - i, 0);

            double r = gdiColor.R / 255d;
            double g = gdiColor.G / 255d;
            double b = gdiColor.B / 255d;

            yield return (r, g, b);
        }
    }
}
