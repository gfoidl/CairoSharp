// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using static Cairo.Surfaces.PDF.PdfSurfaceNative;

namespace Cairo.Surfaces.PDF;

/// <summary>
/// PDF Surfaces — Rendering PDF documents
/// </summary>
/// <remarks>
/// The PDF surface is used to render cairo graphics to Adobe PDF files and is a multi-page
/// vector surface backend.
/// <para>
/// The following mime types are supported on source patterns: <see cref="MimeTypes.Jpeg"/>,
/// <see cref="MimeTypes.Jp2"/>, <see cref="MimeTypes.UniqueId"/>, <see cref="MimeTypes.Jbig2"/>,
/// <see cref="MimeTypes.Jbig2Global"/>, <see cref="MimeTypes.Jbig2GlobalId"/>, <see cref="MimeTypes.CcittFax"/>,
/// <see cref="MimeTypes.CcittFaxParams"/>.
/// </para>
/// <para>
/// For embedded image formats see <a href="https://www.cairographics.org/manual/cairo-PDF-Surfaces.html">cairo docs</a>.
/// </para>
/// </remarks>
public sealed unsafe class PdfSurface : Surface
{
    private GCHandle _streamHandle;     // mutable struct

    internal PdfSurface(void* handle, bool owner, bool throwOnConstructionError = true)
        : base(handle, owner, throwOnConstructionError) { }

    private PdfSurface((IntPtr Handle, GCHandle StreamHandle) arg, bool throwOnConstructionError)
        : base(arg.Handle.ToPointer(), owner: true, throwOnConstructionError)
        => _streamHandle = arg.StreamHandle;

    protected override void DisposeCore(void* handle)
    {
        if (_streamHandle.IsAllocated)
        {
            _streamHandle.Free();
        }

        base.DisposeCore(handle);
    }

    /// <summary>
    /// Creates a PDF surface of the specified size in points, that may be queried and used as a source,
    /// without generating a temporary file.
    /// </summary>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="throwOnConstructionError">
    /// when <c>true</c> (the default) an exception is thrown when the surface could not be created.
    /// </param>
    /// <remarks>
    /// See <see cref="PdfSurface(string?, double, double, bool)"/> for further information.
    /// </remarks>
    /// <exception cref="CairoException">
    /// when construction fails and <paramref name="throwOnConstructionError"/> is set to <c>true</c>
    /// </exception>
    public PdfSurface(double widthInPoints, double heightInPoints, bool throwOnConstructionError = true)
        : this(null as string, widthInPoints, heightInPoints, throwOnConstructionError) { }

    /// <summary>
    /// Creates a PDF surface of the specified size in points to be written to filename.
    /// </summary>
    /// <param name="fileName">
    /// a filename for the PDF output (must be writable), <c>null</c> may be used to specify no output.
    /// This will generate a PDF surface that may be queried and used as a source, without
    /// generating a temporary file.
    /// </param>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="throwOnConstructionError">
    /// when <c>true</c> (the default) an exception is thrown when the surface could not be created.
    /// </param>
    /// <exception cref="CairoException">
    /// when construction fails and <paramref name="throwOnConstructionError"/> is set to <c>true</c>
    /// </exception>
    public PdfSurface(string? fileName, double widthInPoints, double heightInPoints, bool throwOnConstructionError = true)
        : base(cairo_pdf_surface_create(fileName, widthInPoints, heightInPoints), owner: true, throwOnConstructionError) { }

    /// <summary>
    /// Creates a PDF surface of the specified size in points to be written incrementally to the
    /// stream represented by write_func and closure.
    /// </summary>
    /// <param name="stream">The stream to which the PDF content is written to</param>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="throwOnConstructionError">
    /// when <c>true</c> (the default) an exception is thrown when the surface could not be created.
    /// </param>
    /// <exception cref="CairoException">
    /// when construction fails and <paramref name="throwOnConstructionError"/> is set to <c>true</c>
    /// </exception>
    /// <exception cref="ArgumentException">the stream is not writeable</exception>
    public PdfSurface(Stream stream, double widthInPoints, double heightInPoints, bool throwOnConstructionError = true)
        : this(StreamHelper.CreateForWriteStream(stream, widthInPoints, heightInPoints, &cairo_pdf_surface_create_for_stream), throwOnConstructionError) { }

    /// <summary>
    /// Restricts the generated PDF file to version. See <see cref="PdfVersionExtensions.GetSupportedVersions"/>
    /// for a list of available version values that can be used here.
    /// </summary>
    /// <param name="version">PDF version</param>
    /// <remarks>
    /// This method should only be called before any drawing operations have been performed on the
    /// given surface. The simplest way to do this is to call this method immediately after creating the surface.
    /// </remarks>
    public void RestrictToVersion(PdfVersion version)
    {
        this.CheckDisposed();
        cairo_pdf_surface_restrict_to_version(this.Handle, version);
    }

    /// <summary>
    /// Changes the size of a PDF surface for the current (and subsequent) pages.
    /// </summary>
    /// <param name="widthInPoints">new surface width, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">new surface height, in points (1 point == 1/72.0 inch)</param>
    /// <remarks>
    /// This method should only be called before any drawing operations have been performed on the current
    /// page. The simplest way to do this is to call this method immediately after creating the surface
    /// or immediately after completing a page with either <see cref="CairoContext.ShowPage"/>
    /// or <see cref="CairoContext.CopyPage"/>.
    /// </remarks>
    public void SetSize(double widthInPoints, double heightInPoints)
    {
        this.CheckDisposed();
        cairo_pdf_surface_set_size(this.Handle, widthInPoints, heightInPoints);
    }

    public const int PdfOutlineRoot = 0;

    /// <summary>
    /// Add an item to the document outline hierarchy with the name <paramref name="outlineName"/> that
    /// links to the location specified by <paramref name="linkAttribs"/>. Link attributes have the same keys
    /// and values as the Link Tag, excluding the "rect" attribute. The item will be a child of the item with
    /// id parent_id. Use <see cref="PdfOutlineRoot"/> as the parent id of top level items.
    /// </summary>
    /// <param name="parentId">the id of the parent item or <see cref="PdfOutlineRoot"/> if this is a top level item.</param>
    /// <param name="outlineName">the name of the outline</param>
    /// <param name="linkAttribs">the link attributes specifying where this outline links to</param>
    /// <param name="flags">outline item flags</param>
    /// <returns> the id for the added item.</returns>
    public int AddOutline(int parentId, string outlineName, string linkAttribs, PdfOutlineFlags flags)
    {
        this.CheckDisposed();
        return cairo_pdf_surface_add_outline(this.Handle, parentId, outlineName, linkAttribs, flags);
    }

    /// <summary>
    /// Set document metadata.
    /// </summary>
    /// <param name="metadata">The metadata item to set.</param>
    /// <param name="value">metadata value</param>
    /// <remarks>
    /// The <see cref="PdfMetadata.CreationDate"/> and <see cref="PdfMetadata.ModificationDate"/>
    /// values must be in ISO-8601 format: YYYY-MM-DDThh:mm:ss. An optional timezone of the form "[+/-]hh:mm" or "Z"
    /// for UTC time can be appended. All other metadata values can be any UTF-8 string.
    /// </remarks>
    public void SetMetadata(PdfMetadata metadata, string value)
    {
        this.CheckDisposed();
        cairo_pdf_surface_set_metadata(this.Handle, metadata, value);
    }

    /// <summary>
    /// Set custom document metadata.
    /// </summary>
    /// <param name="name">The name of the custom metadata item to set (utf8).</param>
    /// <param name="value">The value of the metadata (utf8).</param>
    /// <remarks>
    /// <paramref name="name"/> may be any string except for the following names reserved by PDF:
    /// "Title", "Author", "Subject", "Keywords", "Creator", "Producer", "CreationDate",
    /// "ModDate", "Trapped".
    /// <para>
    /// If value is <c>null</c> or an empty string, the name metadata will not be set.
    /// </para>
    /// </remarks>
    public void SetCustomMetadata(string name, string? value)
    {
        CairoAPI.CheckSupportedVersion(1, 18, 0);

        this.CheckDisposed();
        cairo_pdf_surface_set_custom_metadata(this.Handle, name, value);
    }

    /// <summary>
    /// Set page label for the current page.
    /// </summary>
    /// <param name="label">The page label.</param>
    public void SetPageLabel(string label)
    {
        this.CheckDisposed();
        cairo_pdf_surface_set_page_label(this.Handle, label);
    }

    /// <summary>
    /// Set the thumbnail image size for the current and all subsequent pages.
    /// Setting a width or height of 0 disables thumbnails for the current and
    /// subsequent pages.
    /// </summary>
    /// <param name="width">Thumbnail width.</param>
    /// <param name="height">Thumbnail height</param>
    public void SetThumbnailSize(int width, int height)
    {
        this.CheckDisposed();
        cairo_pdf_surface_set_thumbnail_size(this.Handle, width, height);
    }
}
