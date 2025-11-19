// (c) gfoidl, all rights reserved

using System.IO;
using static Cairo.Surfaces.PostScript.PostScriptSurfaceNative;

namespace Cairo.Surfaces.PostScript;

/// <summary>
/// PostScript Surfaces — Rendering PostScript documents
/// </summary>
/// <remarks>
/// The PostScript surface is used to render cairo graphics to Adobe PostScript files
/// and is a multi-page vector surface backend.
/// <para>
/// The following mime types are supported on source patterns: <see cref="MimeTypes.Jpeg"/>,
/// <see cref="MimeTypes.UniqueId"/>, <see cref="MimeTypes.CcittFax"/>, <see cref="MimeTypes.CcittFaxParams"/>,
/// <see cref="MimeTypes.Eps"/>, <see cref="MimeTypes.EpsParams"/>.
/// </para>
/// <para>
/// Source surfaces used by the PostScript surface that have a <see cref="MimeTypes.UniqueId"/> mime type will
/// be stored in PostScript printer memory for the duration of the print job. <see cref="MimeTypes.UniqueId"/> should
/// only be used for small frequently used sources.
/// </para>
/// <para>
/// The <see cref="MimeTypes.CcittFax"/> and <see cref="MimeTypes.CcittFaxParams"/> mime types are
/// documented in CCITT Fax Images.
/// </para>
/// <para>
/// Embedding EPS files<br />
/// Encapsulated PostScript files can be embedded in the PS output by setting the <see cref="MimeTypes.Eps"/> mime data
/// on a surface to the EPS data and painting the surface. The EPS will be scaled and translated
/// to the extents of the surface the EPS data is attached to.
/// </para>
/// <para>
/// The <see cref="MimeTypes.Eps"/> mime type requires the <see cref="MimeTypes.EpsParams"/> mime data to also be
/// provided in order to specify the embeddding parameters. <see cref="MimeTypes.EpsParams"/> mime data must contain
/// a string of the form "bbox=[llx lly urx ury]" that specifies the bounding box (in PS coordinates) of
/// the EPS graphics. The parameters are: lower left x, lower left y, upper right x, upper right y.
/// Normally the bbox data is identical to the %%%BoundingBox data in the EPS file.
/// </para>
/// </remarks>
public sealed unsafe class PostScriptSurface : StreamSurface
{
    internal PostScriptSurface(cairo_surface_t* surface, bool isOwnedByCairo, bool needsDestroy = true)
        : base(surface, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Creates a PostScript surface of the specified size in points, that may be queried and used as a source,
    /// without generating a temporary file.
    /// </summary>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <remarks>
    /// See <see cref="PostScriptSurface(string?, double, double)"/> for further information.
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public PostScriptSurface(double widthInPoints, double heightInPoints) : this(null as string, widthInPoints, heightInPoints) { }

    /// <summary>
    /// Creates a PostScript surface of the specified size in points to be written to filename.
    /// See <see cref="PostScriptSurface(Stream, double, double)"/> for a more flexible mechanism for
    /// handling the PostScript output than simply writing it to a named file.
    /// </summary>
    /// <param name="fileName">
    /// a filename for the PS output (must be writable), <c>null</c> may be used to specify no output.
    /// This will generate a PS surface that may be queried and used as a source, without generating a temporary file.
    /// </param>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <remarks>
    /// Note that the size of individual pages of the PostScript output can vary. See <see cref="SetSize"/>.
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public PostScriptSurface(string? fileName, double widthInPoints, double heightInPoints)
        : base(cairo_ps_surface_create(fileName, widthInPoints, heightInPoints)) { }

    /// <summary>
    /// Creates a PostScript surface of the specified size in points to be written incrementally to the
    /// stream represented by write_func and closure. See <see cref="PostScriptSurface(string?, double, double)"/>
    /// for a more convenient way to simply direct the PostScript output to a named file.
    /// </summary>
    /// <param name="stream">The stream to which the PS content is written to</param>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <remarks>
    /// Note that the size of individual pages of the PostScript output can vary. See <see cref="SetSize"/>.
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <c>null</c></exception>
    /// <exception cref="ArgumentException"><paramref name="stream"/> is not writeable</exception>
    public PostScriptSurface(Stream stream, double widthInPoints, double heightInPoints)
        : base(CreateForWriteStream(stream, widthInPoints, heightInPoints, &cairo_ps_surface_create_for_stream)) { }

    /// <summary>
    /// Creates a PostScript surface of the specified size in points to be written incrementally via the
    /// <paramref name="callback"/>.
    /// </summary>
    /// <param name="callback">The callback to be invoked with the PS content to be written</param>
    /// <param name="state">A state object that is passed to the <paramref name="callback"/></param>
    /// <param name="widthInPoints">width of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">height of the surface, in points (1 point == 1/72.0 inch)</param>
    /// <remarks>For a strong typed variant see <see cref="Create"/>.</remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c></exception>
    public PostScriptSurface(Callback callback, object? state, double widthInPoints, double heightInPoints)
        : base(CreateForDelegate(state, callback, widthInPoints, heightInPoints, &cairo_ps_surface_create_for_stream)) { }

    /// <summary>
    /// Restricts the generated PostSript file to level. See <see cref="PostScriptLevelExtensions.GetSupportedLevels"/>
    /// for a list of available level values that can be used here.
    /// </summary>
    /// <param name="level">PostScript level</param>
    /// <remarks>
    /// This method should only be called before any drawing operations have been performed
    /// on the given surface. The simplest way to do this is to call this method immediately after
    /// creating the surface.
    /// </remarks>
    public void RestricToLevel(PostScriptLevel level)
    {
        this.CheckDisposed();
        cairo_ps_surface_restrict_to_level(this.Handle, level);
    }

    /// <summary>
    /// Gets or sets whether the PostScript surface will output Encapsulated PostScript.
    /// </summary>
    /// <remarks>
    /// If eps is <c>true</c>, the PostScript surface will output Encapsulated PostScript.
    /// <para>
    /// This method should only be called before any drawing operations have been performed on
    /// the current page. The simplest way to do this is to call this method immediately after
    /// creating the surface. An Encapsulated PostScript file should never contain more than one page.
    /// </para>
    /// </remarks>
    public bool Eps
    {
        get
        {
            this.CheckDisposed();
            return cairo_ps_surface_get_eps(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_ps_surface_set_eps(this.Handle, value);
        }
    }

    /// <summary>
    /// Changes the size of a PostScript surface for the current (and subsequent) pages.
    /// </summary>
    /// <param name="widthInPoints">new surface width, in points (1 point == 1/72.0 inch)</param>
    /// <param name="heightInPoints">new surface height, in points (1 point == 1/72.0 inch)</param>
    /// <remarks>
    /// This method should only be called before any drawing operations have been performed on the
    /// current page. The simplest way to do this is to call this method immediately after creating
    /// the surface or immediately after completing a page with either <see cref="CairoContext.ShowPage"/>
    /// or <see cref="CairoContext.CopyPage"/>.
    /// </remarks>
    public void SetSize(double widthInPoints, double heightInPoints)
    {
        this.CheckDisposed();
        cairo_ps_surface_set_size(this.Handle, widthInPoints, heightInPoints);
    }

    /// <summary>
    /// This method indicates that subsequent calls to <see cref="DscComment"/> should direct
    /// comments to the Setup section of the PostScript output.
    /// </summary>
    /// <remarks>
    /// This method should be called at most once per surface, and must be called before any call
    /// to <see cref="DscBeginPageSetup"/> and before any drawing is performed to the surface.
    /// <para>
    /// See <see cref="DscComment"/> for more details.
    /// </para>
    /// </remarks>
    public void DscBeginSetup()
    {
        this.CheckDisposed();
        cairo_ps_surface_dsc_begin_setup(this.Handle);
    }

    /// <summary>
    /// This method indicates that subsequent calls to <see cref="DscComment"/> should direct
    /// comments to the PageSetup section of the PostScript output.
    /// </summary>
    /// <remarks>
    /// This method call is only needed for the first page of a surface. It should be called after
    /// any call to <see cref="DscBeginSetup"/> and before any drawing is performed to the surface.
    /// <para>
    /// See <see cref="DscComment"/> for more details.
    /// </para>
    /// </remarks>
    public void DscBeginPageSetup()
    {
        this.CheckDisposed();
        cairo_ps_surface_dsc_begin_page_setup(this.Handle);
    }

    /// <summary>
    /// Emit a comment into the PostScript output for the given surface.
    /// </summary>
    /// <param name="comment">a comment string to be emitted into the PostScript output</param>
    /// <remarks>
    /// The comment is expected to conform to the PostScript Language Document Structuring Conventions (DSC).
    /// Please see that manual for details on the available comments and their meanings. In particular,
    /// the %%IncludeFeature comment allows a device-independent means of controlling printer device features.
    /// So the PostScript Printer Description Files Specification will also be a useful reference.
    /// <para>
    /// See <a href="https://www.cairographics.org/manual/cairo-PostScript-Surfaces.html#cairo-ps-surface-dsc-comment">cairo docs</a>
    /// for further information.
    /// </para>
    /// </remarks>
    public void DscComment(string comment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(comment.Length, 255);

        if (!comment.StartsWith('%'))
        {
            throw new ArgumentException("comment must start with % according the DSC", nameof(comment));
        }

        this.CheckDisposed();
        cairo_ps_surface_dsc_comment(this.Handle, comment);
    }

    /// <summary>
    /// The <see cref="PostScriptSurface"/> can have multiple pages.
    /// </summary>
    /// <remarks>
    /// See <see cref="Surface.ShowPage"/> / <see cref="Surface.CopyPage"/>.
    /// </remarks>
    public override bool CanHaveMultiplePages => true;
}
