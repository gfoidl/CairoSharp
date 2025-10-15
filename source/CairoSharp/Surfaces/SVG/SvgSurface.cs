// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.SVG.SvgSurfaceNative;

namespace Cairo.Surfaces.SVG;

/// <summary>
/// SVG Surfaces — Rendering SVG documents
/// </summary>
/// <remarks>
/// The SVG surface is used to render cairo graphics to SVG files and is a multi-page vector surface backend.
/// </remarks>
public sealed unsafe class SvgSurface : StreamSurface
{
    internal SvgSurface(cairo_surface_t* surface, bool isOwnedByCairo, bool needsDestroy = true)
        : base(surface, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Creates a SVG surface of the specified size in points, that may be queried and used as a source,
    /// without generating a temporary file.
    /// </summary>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <remarks>
    /// See <see cref="SvgSurface(string?, double, double)"/> for further information.
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public SvgSurface(double widthInPoints, double heightInPoints) : this(null as string, widthInPoints, heightInPoints) { }

    /// <summary>
    /// Creates a SVG surface of the specified size in points to be written to <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">
    /// a filename for the SVG output (must be writable), <c>null</c> may be used to specify no output.
    /// This will generate a SVG surface that may be queried and used as a source, without generating a temporary file.
    /// </param>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <remarks>
    /// The SVG surface backend recognizes the following MIME types for the data attached to a surface
    /// (see <see cref="Surface.SetMimeData(string, ReadOnlySpan{byte})"/>) when it is used as a
    /// source pattern for drawing on this surface: <see cref="MimeTypes.Jpeg"/>, <see cref="MimeTypes.Png"/>,
    /// <see cref="MimeTypes.Uri"/>. If any of them is specified, the SVG backend emits a href with the
    /// content of MIME data instead of a surface snapshot (PNG, Base64-encoded) in the corresponding image tag.
    /// <para>
    /// The unofficial MIME type <see cref="MimeTypes.Uri"/> is examined first. If present, the URI is emitted
    /// as is: assuring the correctness of URI is left to the client code.
    /// </para>
    /// <para>
    /// If <see cref="MimeTypes.Uri"/> is not present, but <see cref="MimeTypes.Jpeg"/> or <see cref="MimeTypes.Png"/>
    /// is specified, the corresponding data is Base64-encoded and emitted.
    /// </para>
    /// <para>
    /// If <see cref="MimeTypes.UniqueId"/> is present, all surfaces with the same unique identifier will
    /// only be embedded once.
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public SvgSurface(string? fileName, double widthInPoints, double heightInPoints)
        : base(cairo_svg_surface_create(fileName, widthInPoints, heightInPoints)) { }

    /// <summary>
    /// Creates a SVG surface of the specified size in points to be written incrementally to the stream
    /// </summary>
    /// <param name="stream">The stream to which the SVG content is written to</param>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <exception cref="CairoException">when construction fails</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <c>null</c></exception>
    /// <exception cref="ArgumentException"><paramref name="stream"/> is not writeable</exception>
    public SvgSurface(Stream stream, double widthInPoints, double heightInPoints)
        : base(CreateForWriteStream(stream, widthInPoints, heightInPoints, &cairo_svg_surface_create_for_stream)) { }

    /// <summary>
    /// Creates a SVG surface of the specified size in points to be written incrementally via the
    /// <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">The callback to be invoked with the SVG content to be written</param>
    /// <param name="state">A state object that is passed to the <paramref name="callback"/></param>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <exception cref="CairoException">when construction fails</exception>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c></exception>
    public SvgSurface(Callback<object> callback, object? state, double widthInPoints, double heightInPoints)
        : base(CreateForDelegate(state, callback, widthInPoints, heightInPoints, &cairo_svg_surface_create_for_stream)) { }

    /// <summary>
    /// Gets or sets the unit of the SVG surface.
    /// </summary>
    /// <remarks>
    /// If the surface passed as an argument is not a SVG surface, the function sets the error status to
    /// <see cref="Status.SurfaceTypeMismatch"/> and returns <see cref="SvgUnit.User"/>.
    /// <para>
    /// Use the specified unit for the width and height of the generated SVG file.
    /// See <see cref="SvgUnit"/> for a list of available unit values that can be used here.
    /// </para>
    /// <para>
    /// This property can be called at any time before generating the SVG file.
    /// </para>
    /// <para>
    /// However to minimize the risk of ambiguities it's recommended to call it before any drawing
    /// operations have been performed on the given surface, to make it clearer what the unit used
    /// in the drawing operations is.
    /// </para>
    /// <para>
    /// The simplest way to do this is to call this property immediately after creating the SVG surface.
    /// </para>
    /// <para>
    /// Note if this property is never called, the default unit for SVG documents generated by cairo will be user unit.
    /// </para>
    /// </remarks>
    public SvgUnit DocumentUnit
    {
        get
        {
            this.CheckDisposed();
            return cairo_svg_surface_get_document_unit(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_svg_surface_set_document_unit(this.Handle, value);
        }
    }

    /// <summary>
    /// Restricts the generated SVG file to version. See <see cref="SvgVersionExtensions.GetSupportedVersions"/> for a list of available
    /// version values that can be used here.
    /// </summary>
    /// <param name="version">SVG version</param>
    /// <remarks>
    /// This method should only be called before any drawing operations have been performed on
    /// the given surface. The simplest way to do this is to call this method immediately after
    /// creating the surface.
    /// </remarks>
    public void RestrictToVersion(SvgVersion version)
    {
        this.CheckDisposed();
        cairo_svg_surface_restrict_to_version(this.Handle, version);
    }

    /// <summary>
    /// The <see cref="SvgSurface"/> can have multiple pages.
    /// </summary>
    /// <remarks>
    /// See <see cref="Surface.ShowPage"/> / <see cref="Surface.CopyPage"/>.
    /// </remarks>
    public override bool CanHaveMultiplePages => true;
}
