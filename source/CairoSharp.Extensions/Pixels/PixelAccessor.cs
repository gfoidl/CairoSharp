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
        // In Format.Argb32 pre-multiplied alpha is used.

        get
        {
            int idx = GetIndex(x, y);

            byte a = _data[idx + 3];
            byte r = _data[idx + 2];
            byte g = _data[idx + 1];
            byte b = _data[idx + 0];

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
        set
        {
            byte r, g, b, a;

            if (value.Alpha == 1d)
            {
                a = 0xFF;
                r = (byte)(value.Red   * 255d);
                g = (byte)(value.Green * 255d);
                b = (byte)(value.Blue  * 255d);
            }
            else
            {
                a = (byte)(value.Alpha * 255d);
                r = (byte)(value.Red   * value.Alpha * 255d);
                g = (byte)(value.Green * value.Alpha * 255d);
                b = (byte)(value.Blue  * value.Alpha * 255d);
            }

            int idx = GetIndex(x, y);

            _data[idx + 3] = a;
            _data[idx + 2] = r;
            _data[idx + 1] = g;
            _data[idx + 0] = b;
        }
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
        int width = _surface.Width;

        return _data.Slice(start, width);
    }
}
