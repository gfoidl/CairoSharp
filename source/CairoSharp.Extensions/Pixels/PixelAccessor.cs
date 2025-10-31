// (c) gfoidl, all rights reserved

using Cairo.Surfaces;
using Cairo.Surfaces.Images;

namespace Cairo.Extensions.Pixels;

/// <summary>
/// Allows getting / setting of pixel data with a <see cref="ImageSurface"/>.
/// </summary>
public readonly ref struct PixelAccessor : IDisposable
{
    private readonly ImageSurface _surface;
    private readonly Span<byte>   _data;
    private readonly int          _stride;

    internal PixelAccessor(ImageSurface surface)
    {
        if (surface.Format != Format.Argb32)
        {
            throw new NotSupportedException($"Only Format.Argb32 is supported, actual format is {surface.Format}");
        }

        surface.Flush();

        _surface = surface;
        _data    = surface.Data;
        _stride  = surface.Stride;
    }

    /// <summary>
    /// Calls <see cref="Surface.MarkDirty()"/>
    /// </summary>
    public void Dispose() => _surface.MarkDirty();

    /// <summary>
    /// Gets or sets the <see cref="Color"/> at (<paramref name="x"/>, <paramref name="y"/>).
    /// </summary>
    /// <param name="x">x position of the pixel</param>
    /// <param name="y">y position of the pixel</param>
    /// <returns>The <see cref="Color"/> at the position.</returns>
    public Color this[int x, int y]
    {
        get => PixelHelper.GetColor(_data, this.GetIndex(x, y));
        set => PixelHelper.SetColor(_data, this.GetIndex(x, y), value);
    }

    private int GetIndex(int x, int y) => y * _stride + x * 4;

    /// <summary>
    /// Gets the specified pixel row.
    /// </summary>
    /// <param name="y">the row</param>
    /// <returns>a span for direct manipulation of the pixels in the row</returns>
    /// <remarks>
    /// The row is <see cref="ImageSurface.Width"/> long (not <see cref="ImageSurface.Stride"/>).
    /// </remarks>
    public Span<byte> GetRowBytes(int y)
    {
        int start = y * _stride;
        int width = _surface.Width * 4;

        return _data.Slice(start, width);
    }

    /// <summary>
    /// Gets a <see cref="PixelRowAccessor"/> for the specified row.
    /// </summary>
    /// <param name="y">the row</param>
    /// <returns>
    /// A <see cref="PixelRowAccessor"/> for direct manipulation of the pixels in the row.
    /// </returns>
    public PixelRowAccessor GetRowAccessor(int y)
    {
        Span<byte> rowBytes = this.GetRowBytes(y);
        return new PixelRowAccessor(rowBytes);
    }
}

/// <summary>
/// Allows getting / setting of pixel data within a single row.
/// </summary>
public readonly ref struct PixelRowAccessor
{
    private readonly Span<byte> _rowBytes;

    internal PixelRowAccessor(Span<byte> rowBytes) => _rowBytes = rowBytes;

    /// <summary>
    /// Gets or sets the <see cref="Color"/> at <paramref name="x"/>.
    /// </summary>
    /// <param name="x">x position of the pixel</param>
    /// <returns>The <see cref="Color"/> at the position.</returns>
    public Color this[int x]
    {
        get => PixelHelper.GetColor(_rowBytes, GetIndex(x));
        set => PixelHelper.SetColor(_rowBytes, GetIndex(x), value);
    }

    private static int GetIndex(int x) => x * 4;
}

internal static class PixelHelper
{
    // In Format.Argb32 pre-multiplied alpha is used.

    public static Color GetColor(ReadOnlySpan<byte> data, int idx)
    {
        byte a = data[idx + 3];
        byte r = data[idx + 2];
        byte g = data[idx + 1];
        byte b = data[idx + 0];

        const double OneBy255 = 1d / 255;

        double red, green, blue, alpha;

        if (a == 0xFF)
        {
            alpha = 1d;
            red   = r * OneBy255;
            green = g * OneBy255;
            blue  = b * OneBy255;
        }
        else
        {
            alpha             = a * OneBy255;
            double oneByAlpha = 1d / alpha;

            red   = r * OneBy255 * oneByAlpha;
            green = g * OneBy255 * oneByAlpha;
            blue  = b * OneBy255 * oneByAlpha;

        }

        return new Color(red, green, blue, alpha);
    }

    public static void SetColor(Span<byte> data, int idx, Color color)
    {
        byte r, g, b, a;

        if (color.Alpha == 1d)
        {
            a = 0xFF;
            r = (byte)(color.Red   * 255d);
            g = (byte)(color.Green * 255d);
            b = (byte)(color.Blue  * 255d);
        }
        else
        {
            a = (byte)(color.Alpha * 255d);
            r = (byte)(color.Red   * color.Alpha * 255d);
            g = (byte)(color.Green * color.Alpha * 255d);
            b = (byte)(color.Blue  * color.Alpha * 255d);
        }

        data[idx + 3] = a;
        data[idx + 2] = r;
        data[idx + 1] = g;
        data[idx + 0] = b;
    }
}
