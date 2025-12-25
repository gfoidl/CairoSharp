// (c) gfoidl, all rights reserved

using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cairo.Extensions.GObject;
using Cairo.Surfaces;
using Cairo.Surfaces.Images;
using static Cairo.Extensions.Loading.LoadingNative;

namespace Cairo.Extensions.Loading.SVG;

/// <summary>
/// Represents a parsed SVG document.
/// </summary>
public sealed unsafe class SvgDocument : Document
{
    private void*       _fileOrStream;
    private RsvgHandle* _handle;

    /// <summary>
    /// Loads the SVG file.
    /// </summary>
    /// <param name="fileName">SVG file</param>
    /// <param name="flags">flags from <see cref="RsvgHandleFlags"/></param>
    /// <param name="dpi">
    /// the DPI at which the SVG will be rendered in cairo. Common values are 75, 90 and 300 DPI. See
    /// <a href="https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/class.Handle.html#resolution-of-the-rendered-image-dots-per-inch-or-dpi">Resolution of the rendered image (dots per inch, or DPI)</a>
    /// for further information. Current CSS assumes a default DPI of 96 (the default used here).
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <c>null</c></exception>
    /// <exception cref="LibRsvgException">an error occured</exception>
    public SvgDocument(string fileName, RsvgHandleFlags flags = RsvgHandleFlags.None, double dpi = 96d)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        GError* error;

        _fileOrStream = g_file_new_for_path(fileName);
        _handle       = rsvg_handle_new_from_gfile_sync((GFile*)_fileOrStream, flags, cancellable: null, &error);

        this.FinishConstruction(dpi, error);
    }

    /// <summary>
    /// Loads the SVG data.
    /// </summary>
    /// <param name="svgData">SVG data</param>
    /// <param name="flags">flags from <see cref="RsvgHandleFlags"/></param>
    /// <param name="dpi">
    /// the DPI at which the SVG will be rendered in cairo. Common values are 75, 90 and 300 DPI. See
    /// <a href="https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/class.Handle.html#resolution-of-the-rendered-image-dots-per-inch-or-dpi">Resolution of the rendered image (dots per inch, or DPI)</a>
    /// for further information. Current CSS assumes a default DPI of 96 (the default used here).
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="svgData"/> is empty</exception>
    /// <exception cref="LibRsvgException">an error occured</exception>
    public SvgDocument(ReadOnlySpan<byte> svgData, RsvgHandleFlags flags = RsvgHandleFlags.None, double dpi = 96d)
    {
        if (svgData.IsEmpty)
        {
            throw new ArgumentException("no data given", nameof(svgData));
        }

        GError* error;

        fixed (byte* ptr = svgData)
        {
            // According https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/ctor.Handle.new_from_data.html
            // rsvg_handle_new_from_data shouldn't be used, as no flags, etc. can be set.
            _fileOrStream = g_memory_input_stream_new_from_data(ptr, (nint)svgData.Length, &DummyDestroyNotify);
            _handle       = rsvg_handle_new_from_stream_sync((GInputStream*)_fileOrStream, null, flags, cancellable: null, &error);
        }

        this.FinishConstruction(dpi, error);

        // Let the GC do it's thing.
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void DummyDestroyNotify(void* _) { }
    }

    private void FinishConstruction(double dpi, GError* error)
    {
        if (_handle is null)
        {
            throw new LibRsvgException(error);
        }

        rsvg_handle_set_dpi(_handle, dpi);
    }

    protected override void DisposeCore()
    {
        if (_handle is not null)
        {
            GObjectNative.g_object_unref(_handle);
            _handle = null;
        }

        if (_fileOrStream is not null)
        {
            GObjectNative.g_object_unref(_fileOrStream);
            _fileOrStream = null;
        }
    }

    protected override void CheckNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_handle       is null, this);
        ObjectDisposedException.ThrowIf(_fileOrStream is null, this);
    }

    internal RsvgHandle* Handle => _handle;

    /// <summary>
    /// Gets the base uri for this <see cref="SvgDocument"/>
    /// </summary>
    public string? BaseUri
    {
        get
        {
            this.CheckNotDisposed();

            return rsvg_handle_get_base_uri(_handle);
        }
    }

    /// <summary>
    /// Converts an SVG document's intrinsic dimensions to pixels, and returns the result.
    /// </summary>
    /// <param name="widthInPixels">Will be set to the computed width; you should round this up to get integer pixels.</param>
    /// <param name="heightInPixels">Will be set to the computed height; you should round this up to get integer pixels.</param>
    /// <returns>
    /// <c>true</c> if the dimensions could be converted directly to pixels; in this case <paramref name="widthInPixels"/> and
    /// <paramref name="heightInPixels"/> will be set accordingly. Note that the dimensions are floating-point numbers, so your
    /// application can know the exact size of an SVG document. To get integer dimensions, you should use
    /// <see cref="Math.Ceiling(double)"/> to round up to the nearest integer (just using <see cref="Math.Round(double)"/>,
    /// may chop off pixels with fractional coverage). If the dimensions cannot be converted to pixels, returns
    /// <c>false</c> and puts 0.0 in both <paramref name="widthInPixels"/> and <paramref name="heightInPixels"/>.
    /// </returns>
    /// <remarks>
    /// This function is able to extract the size in pixels from an SVG document if the document has both <c>width</c> and
    /// <c>height</c> attributes with physical units (px, in, cm, mm, pt, pc) or font-based units (em, ex). For physical
    /// units, the dimensions are normalized to pixels using the dots-per-inch (DPI) value set in the constructor.
    /// For font-based units, this function uses the computed value of the font-size property for the toplevel
    /// <c>&lt;svg&gt;</c> element. In those cases, this method returns <c>true</c>.
    /// <para>
    /// For historical reasons, the default DPI is 90. Current CSS assumes a default DPI of 96.
    /// </para>
    /// <para>
    /// This function is not able to extract the size in pixels directly from the intrinsic dimensions of the SVG document
    /// if the <c>width</c> or <c>height</c> are in percentage units (or if they do not exist, in which case the SVG spec
    /// mandates that they default to 100%), as these require a viewport to be resolved to a final size. In this case,
    /// the method returns <c>false</c>.
    /// </para>
    /// </remarks>
    public bool TryGetSizeInPixels(out double widthInPixels, out double heightInPixels)
        => rsvg_handle_get_intrinsic_size_in_pixels(_handle, out widthInPixels, out heightInPixels);

    /// <summary>
    /// In simple terms, queries the <c>width</c>, <c>height</c>, and <c>viewBox</c> attributes in an SVG document.
    /// </summary>
    /// <param name="width">will be set to the computed value of the <c>width</c> property in the toplevel SVG</param>
    /// <param name="height">will be set to the computed value of the <c>height</c> property in the toplevel SVG</param>
    /// <param name="viewBox">will be set to the value of the <c>viewBox</c> attribute in the toplevel SVG</param>
    /// <remarks>
    /// See <a href="https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/method.Handle.get_intrinsic_dimensions.html#description">librsvg docs</a>
    /// for further information.
    /// </remarks>
    public void GetIntrinsicDimensions(out SvgLength? width, out SvgLength? height, out RsvgRectangle? viewBox)
    {
        width   = null;
        height  = null;
        viewBox = null;
        RsvgRectangle viewBoxLocal;

        rsvg_handle_get_intrinsic_dimensions(_handle,
            out uint hasWidth  , out SvgLength widthLocal,
            out uint hasHeight , out SvgLength heightLocal,
            out uint hasViewBox, &viewBoxLocal);

        if (hasWidth > 0)
        {
            width = widthLocal;
        }

        if (hasHeight > 0)
        {
            height = heightLocal;
        }

        if (hasViewBox > 0)
        {
            viewBox = viewBoxLocal;
        }
    }

    /// <summary>
    /// Renders the SVG to a PNG file.
    /// </summary>
    /// <param name="fileName">the name of a file to write to; on Windows this filename is encoded in UTF-8.</param>
    public void RenderToPng(string fileName)
    {
        using ImageSurface surface = this.RenderToPngCore();
        surface.WriteToPng(fileName);
    }

    /// <summary>
    /// Renders the SVG to a PNG stream.
    /// </summary>
    /// <param name="stream">the stream to write to</param>
    public void RenderToPng(Stream stream)
    {
        using ImageSurface surface = this.RenderToPngCore();
        surface.WriteToPng(stream);
    }

    /// <summary>
    /// Renders the SVG to a PNG buffer writer.
    /// </summary>
    /// <param name="bufferWriter">the buffer writer to write to</param>
    public void RenderToPng(IBufferWriter<byte> bufferWriter)
    {
        using ImageSurface surface = this.RenderToPngCore();
        surface.WriteToPng(bufferWriter);
    }

    private ImageSurface RenderToPngCore()
    {
        // Based on https://gitlab.freedesktop.org/cairo/cairo/-/blob/master/test/svg2png.c?ref_type=heads

        if (!this.TryGetSizeInPixels(out double widthInPixels, out double heightInPixels))
        {
            throw new LibRsvgException("SVG document doesn't have intrinsic dimensions");
        }

        ImageSurface surface  = new(Format.Rgb24, (int)Math.Ceiling(widthInPixels), (int)Math.Ceiling(heightInPixels));
        using CairoContext cr = new(surface);

        cr.SetSourceRgb(1, 1, 1);
        cr.Paint();

        cr.PushGroupWithContent(Content.ColorAlpha);

        LibRSvgExtensions.RenderCore(this.Handle, cr, new RsvgRectangle(0, 0, widthInPixels, heightInPixels));

        cr.PopGroupToSource();
        cr.Paint();

        return surface;
    }
}
