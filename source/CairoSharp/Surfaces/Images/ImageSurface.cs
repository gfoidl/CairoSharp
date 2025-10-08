// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.Images.ImageSurfaceNative;
using static Cairo.Surfaces.Images.PngSupportNative;

namespace Cairo.Surfaces.Images;

/// <summary>
/// Image Surfaces — Rendering to memory buffers
/// </summary>
/// <remarks>
/// Image surfaces provide the ability to render to memory buffers either allocated by
/// cairo or by the calling code. The supported image formats are those defined in
/// <see cref="Surfaces.Format"/>.
/// </remarks>
public sealed unsafe class ImageSurface : Surface
{
    internal ImageSurface(void* handle, bool owner, bool throwOnConstructionError = true)
        : base(handle, owner, throwOnConstructionError) { }

    /// <summary>
    /// Creates an image surface of the specified format and dimensions. Initially the surface
    /// contents are set to 0. (Specifically, within each pixel, each color or alpha channel
    /// belonging to format will be 0. The contents of bits within a pixel, but not
    /// belonging to the given format are undefined).
    /// </summary>
    /// <param name="format">format of pixels in the surface to create</param>
    /// <param name="width">width of the surface, in pixels</param>
    /// <param name="height">height of the surface, in pixels</param>
    /// <param name="throwOnConstructionError">
    /// when <c>true</c> (the default) an exception is thrown when the surface could not be created.
    /// </param>
    /// <remarks>
    /// The caller owns the surface and should call <see cref="CairoObject.Dispose()"/> when done with it.
    /// </remarks>
    /// <exception cref="CairoException">
    /// when construction fails and <paramref name="throwOnConstructionError"/> is set to <c>true</c>
    /// </exception>
    public ImageSurface(Format format, int width, int height, bool throwOnConstructionError = true)
        : base(cairo_image_surface_create(format, width, height), owner: true, throwOnConstructionError) { }

    /// <summary>
    /// Creates an image surface for the provided pixel data. The output buffer must be kept
    /// around until the <see cref="Surface"/> is destroyed or <see cref="Surface.Finish"/>
    /// is called on the surface. The initial contents of data will be used as the initial image
    /// contents; you must explicitly clear the buffer, using, for example, cairo_rectangle()
    /// and cairo_fill() if you want it cleared.
    /// </summary>
    /// <param name="data">
    /// a pointer to a buffer supplied by the application in which to write contents.
    /// This pointer must be suitably aligned for any kind of variable, (for example, a
    /// pointer returned by malloc).
    /// </param>
    /// <param name="format">the format of pixels in the buffer</param>
    /// <param name="widht">the width of the image to be stored in the buffer</param>
    /// <param name="height">the height of the image to be stored in the buffer</param>
    /// <param name="stride">
    /// the number of bytes between the start of rows in the buffer as allocated.
    /// This value should always be computed by <see cref="FormatExtensions.GetStrideForWidth(Format, int)"/>
    /// before allocating the data buffer.
    /// </param>
    /// <param name="throwOnConstructionError">
    /// when <c>true</c> (the default) an exception is thrown when the surface could not be created.
    /// </param>
    /// <remarks>
    /// Note that the stride may be larger than width * bytes_per_pixel to provide proper alignment
    /// for each pixel and row. This alignment is required to allow high-performance rendering
    /// within cairo. The correct way to obtain a legal stride value is to call <see cref="FormatExtensions.GetStrideForWidth(Format, int)"/>
    /// with the desired format and maximum image width value, and then use the resulting stride value to allocate
    /// the data and to create the image surface.
    /// </remarks>
    /// <exception cref="CairoException">
    /// when construction fails and <paramref name="throwOnConstructionError"/> is set to <c>true</c>
    /// </exception>
    public ImageSurface(ReadOnlySpan<byte> data, Format format, int widht, int height, int stride, bool throwOnConstructionError = true)
        : base(cairo_image_surface_create_for_data(data, format, widht, height, stride), owner: true, throwOnConstructionError) { }

    /// <summary>
    /// Creates a new image surface and initializes the contents to the given PNG file.
    /// </summary>
    /// <param name="pngFile">name of PNG file to load. On Windows this filename is encoded in UTF-8.</param>
    /// <param name="throwOnConstructionError">
    /// when <c>true</c> (the default) an exception is thrown when the surface could not be created.
    /// </param>
    /// <remarks>
    /// a new cairo_surface_t initialized with the contents of the PNG file, or a "nil" surface if any error
    /// occurred. A nil surface can be checked for with <see cref="Surface.Status"/> which may return one
    /// of the following values: <see cref="Status.NoMemory"/>, <see cref="Status.FileNotFound"/>,
    /// <see cref="Status.ReadError"/>, <see cref="Status.PngError"/>.
    /// <para>
    /// Alternatively, you can allow errors to propagate through the drawing operations and check the
    /// status on the context upon completion using <see cref="CairoContext.Status"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">
    /// when construction fails and <paramref name="throwOnConstructionError"/> is set to <c>true</c>
    /// </exception>
    public ImageSurface(string pngFile, bool throwOnConstructionError = true)
        : base(cairo_image_surface_create_from_png(pngFile), owner: true, throwOnConstructionError) { }

    /// <summary>
    /// Creates  a new <see cref="ImageSurface"/> initialized with the contents of the PNG data or a "nil" surface
    /// if the data read is not a valid PNG image or memory could not be allocated for the operation.
    /// A nil surface can be checked for with <see cref="Surface.Status"/> which may return one of the
    /// following values: <see cref="Status.NoMemory"/>, <see cref="Status.ReadError"/>, <see cref="Status.PngError"/>
    /// </summary>
    /// <param name="pngData">The PNG data</param>
    /// <param name="throwOnConstructionError">
    /// when <c>true</c> (the default) an exception is thrown when the surface could not be created.
    /// </param>
    /// <remarks>
    /// Alternatively, you can allow errors to propagate through the drawing operations and check the status
    /// on the context upon completion using <see cref="CairoContext.Status"/>.
    /// </remarks>
    /// <exception cref="CairoException">
    /// when construction fails and <paramref name="throwOnConstructionError"/> is set to <c>true</c>
    /// </exception>
    public ImageSurface(ReadOnlySpan<byte> pngData, bool throwOnConstructionError = true)
        : base(PngHelper.CreateForPngData(pngData), owner: true, throwOnConstructionError) { }

    /// <summary>
    /// Get a pointer to the data of the image surface, for direct inspection or modification.
    /// </summary>
    /// <remarks>
    /// A call to <see cref="Surface.Flush"/> is required before accessing the pixel data to
    /// ensure that all pending drawing operations are finished. A call to <see cref="Surface.MarkDirty()"/>
    /// is required after the data is modified.
    /// <para>
    ///  a pointer to the image data of this surface or <see cref="ReadOnlySpan{T}.Empty"/> if surface is
    ///  not an image surface, or if <see cref="Surface.Finish"/> has been called.
    /// </para>
    /// </remarks>
    public ReadOnlySpan<byte> Data
    {
        get
        {
            this.CheckDisposed();

            byte* data = cairo_image_surface_get_data(this.Handle);

            if (data is null)
            {
                return [];
            }

            int length = this.Height * this.Stride;
            return new ReadOnlySpan<byte>(data, length);
        }
    }

    /// <summary>
    /// Get the format of the surface.
    /// </summary>
    public Format Format
    {
        get
        {
            this.CheckDisposed();
            return cairo_image_surface_get_format(this.Handle);
        }
    }

    /// <summary>
    /// Get the width of the image surface in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            this.CheckDisposed();
            return cairo_image_surface_get_width(this.Handle);
        }
    }

    /// <summary>
    /// Get the height of the image surface in pixels.
    /// </summary>
    public int Height
    {
        get
        {
            this.CheckDisposed();
            return cairo_image_surface_get_height(this.Handle);
        }
    }

    /// <summary>
    /// Get the stride of the image surface in bytes
    /// </summary>
    /// <remarks>
    ///  the stride of the image surface in bytes (or 0 if surface is not an image surface).
    ///  The stride is the distance in bytes from the beginning of one row of the
    ///  image data to the beginning of the next row.
    /// </remarks>
    public int Stride
    {
        get
        {
            this.CheckDisposed();
            return cairo_image_surface_get_stride(this.Handle);
        }
    }
}
