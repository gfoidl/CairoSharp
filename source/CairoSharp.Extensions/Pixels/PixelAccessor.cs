// (c) gfoidl, all rights reserved

using System.Diagnostics.CodeAnalysis;
using Cairo.Surfaces;
using Cairo.Surfaces.Images;

namespace Cairo.Extensions.Pixels;

/// <summary>
/// Allows getting / setting of pixel data with a <see cref="ImageSurface"/>.
/// </summary>
public readonly ref struct PixelAccessor : IDisposable
{
    private readonly ImageSurface? _surface;
    private readonly Span<byte>    _data;
    private readonly int           _stride;
    private readonly int           _width;

    internal PixelAccessor(ImageSurface surface, bool setSurfaceState = true)
    {
        if (surface.Format != Format.Argb32)
        {
            Throw(surface);

            [DoesNotReturn]
            static void Throw(ImageSurface surface)
            {
                throw new NotSupportedException($"Only Format.Argb32 is supported, actual format is {surface.Format}");
            }
        }

        if (setSurfaceState)
        {
            surface.Flush();
            _surface = surface;
        }

        _data   = surface.Data;
        _stride = surface.Stride;
        _width  = surface.Width;
    }

    /// <summary>
    /// Calls <see cref="Surface.MarkDirty()"/>
    /// </summary>
    public void Dispose() => _surface?.MarkDirty();

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
        int width = _width * 4;

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
