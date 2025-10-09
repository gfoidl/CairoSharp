// (c) gfoidl, all rights reserved

using System.Diagnostics.CodeAnalysis;
using Cairo.Drawing.Text;
using Cairo.Utilities;
using static Cairo.Fonts.Scaled.ScaledFontNative;

namespace Cairo.Fonts.Scaled;

/// <summary>
/// cairo_scaled_font_t — Font face at particular size and options
/// </summary>
/// <remarks>
/// cairo_scaled_font_t represents a realization of a font face at a particular size
/// and transformation and a certain set of font options.
/// </remarks>
public sealed unsafe class ScaledFont : FontFace
{
    private readonly FontOptions? _fontOptions;

    internal ScaledFont(void* handle, bool owner, bool throwOnConstructionError = true)
        : base(handle, owner, throwOnConstructionError, &cairo_scaled_font_reference) { }

    /// <summary>
    /// Creates a <see cref="ScaledFont"/> object from a font face and matrices that describe
    /// the size of the font and the environment in which it will be used.
    /// </summary>
    /// <param name="fontFace">a <see cref="FontFace"/></param>
    /// <param name="fontMatrix">
    /// font space to user space transformation matrix for the font. In the simplest case of a N point
    /// font, this matrix is just a scale by N, but it can also be used to shear the font or
    /// stretch it unequally along the two axes. See <see cref="TextExtensions.GetFontMatrix(CairoContext, out Matrix)"/>.
    /// </param>
    /// <param name="ctm">user to device transformation matrix with which the font will be used.</param>
    /// <param name="fontOptions">options to use when getting metrics for the font and rendering with it.</param>
    /// <remarks>
    /// Destroy with <see cref="CairoObject.Dispose()"/>.
    /// </remarks>
    public ScaledFont(FontFace fontFace, ref Matrix fontMatrix, ref Matrix ctm, FontOptions fontOptions)
        : base(cairo_scaled_font_create(fontFace.Handle, ref fontMatrix, ref ctm, fontOptions.Handle), owner: true)
        => _fontOptions = fontOptions;

    protected override void DisposeCore(void* handle) => cairo_scaled_font_destroy(handle);

    /// <summary>
    /// Checks whether an error has previously occurred for this scaled_font.
    /// </summary>
    /// <remarks>
    /// <see cref="Status.Success"/> or another error such as <see cref="Status.NoMemory"/>.
    /// </remarks>
    public new Status Status
    {
        get
        {
            this.CheckDisposed();
            return cairo_scaled_font_status(this.Handle);
        }
    }

    /// <summary>
    /// Gets the metrics for a <see cref="ScaledFont"/>.
    /// </summary>
    public FontExtents FontExtents
    {
        get
        {
            this.CheckDisposed();

            cairo_scaled_font_extents(this.Handle, out FontExtents fontExtents);
            return fontExtents;
        }
    }

    /// <summary>
    /// Gets the extents for a string of text. The extents describe a user-space rectangle
    /// that encloses the "inked" portion of the text drawn at the origin (0,0) (as it would
    /// be drawn by <see cref="TextExtensions.ShowText(CairoContext, string?)"/> if the cairo graphics state were
    /// set to the same <see cref="FontFace"/>, font_matrix, ctm, and font_options as scaled_font).
    /// Additionally, the x_advance and y_advance values indicate the amount by which the current
    /// point would be advanced by <see cref="TextExtensions.ShowText(CairoContext, string?)"/>.
    /// </summary>
    /// <param name="text">a NUL-terminated string of text, encoded in UTF-8</param>
    /// <remarks>
    /// Note that whitespace characters do not directly contribute to the size of the rectangle
    /// (extents.width and extents.height). They do contribute indirectly by changing the position
    /// of non-whitespace characters. In particular, trailing whitespace characters are likely to not
    /// affect the size of the rectangle, though they will affect the x_advance and y_advance values.
    /// </remarks>
    public void TextExtents(string text, out TextExtents textExtents)
    {
        this.CheckDisposed();
        cairo_scaled_font_text_extents(this.Handle, text, out textExtents);
    }

    /// <summary>
    /// Gets the extents for an array of glyphs. The extents describe a user-space rectangle
    /// that encloses the "inked" portion of the glyphs, (as they would be drawn by
    /// <see cref="TextExtensions.ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/> if the cairo graphics
    /// state were set to the same <see cref="FontFace"/>, font_matrix, ctm, and font_options as scaled_font).
    /// Additionally, the x_advance and y_advance values indicate the amount by which the current point
    /// would be advanced by <see cref="TextExtensions.ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>.
    /// </summary>
    /// <param name="glyphs">an array of glyph IDs with X and Y offsets.</param>
    public void GlyphExtents(ReadOnlySpan<Glyph> glyphs, out TextExtents textExtents)
    {
        this.CheckDisposed();

        fixed (Glyph* ptr = glyphs)
        {
            cairo_scaled_font_glyph_extents(this.Handle, ptr, glyphs.Length, out textExtents);
        }
    }

    /// <summary>
    /// Converts UTF-8 text to an array of glyphs, optionally with cluster mapping, that can be used to
    /// render later using <see cref="ScaledFont"/>.
    /// </summary>
    /// <param name="x">X position to place first glyph</param>
    /// <param name="y">Y position to place first glyph</param>
    /// <param name="text">a string of text</param>
    /// <param name="clusters">pointer to array of cluster mapping information to fill</param>
    /// <param name="clusterFlags">
    /// pointer to location to store cluster flags corresponding to the output <paramref name="clusters"/>
    /// </param>
    /// <param name="useClusterMapping">whether cluster mapping should be used or not</param>
    /// <returns>array of glyphs</returns>
    /// <remarks>
    /// For details of how <paramref name="clusters"/>, and <paramref name="clusterFlags"/> map input UTF-8 text
    /// to the output glyphs see
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, string, ReadOnlySpan{Glyph}, ReadOnlySpan{TextCluster}, ClusterFlags)"/>.
    /// <para>
    /// The output values can be readily passed to
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, string, ReadOnlySpan{Glyph}, ReadOnlySpan{TextCluster}, ClusterFlags)"/>,
    /// <see cref="TextExtensions.ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>, or related methods, assuming that the exact
    /// same <see cref="ScaledFont"/> is used for the operation.
    /// </para>
    /// </remarks>
    /// <example>
    /// See <a href="https://www.codeproject.com/articles/Programming-Cairo-Text-Output-Beyond-the-toy-text">sample.</a>
    /// </example>
    public GlyphArray TextToGlyphs(double x, double y, string text, out TextClusterArray? clusters, out ClusterFlags clusterFlags, bool useClusterMapping = true)
    {
        this.CheckDisposed();

        if (useClusterMapping)
        {
            Status status = cairo_scaled_font_text_to_glyphs(
                this.Handle,
                x, y,
                text, -1,
                out Glyph* glyphs, out int numGlyphs,
                out TextCluster* clustersNative, out int numClusters,
                out clusterFlags);

            status.ThrowIfNotSuccess();

            clusters = new TextClusterArray(clustersNative, numClusters);
            return new GlyphArray(glyphs, numGlyphs);
        }
        else
        {
            Status status = cairo_scaled_font_text_to_glyphs(
                this.Handle,
                x, y,
                text, -1,
                out Glyph* glyphs, out int numGlyphs,
                null, null, null);

            status.ThrowIfNotSuccess();

            clusters     = null;
            clusterFlags = default;
            return new GlyphArray(glyphs, numGlyphs);
        }
    }

    /// <summary>
    /// Converts UTF-8 text to an array of glyphs, optionally with cluster mapping, that can be used to
    /// render later using <see cref="ScaledFont"/>.
    /// </summary>
    /// <param name="point">position to place first glyph</param>
    /// <param name="text">a string of text</param>
    /// <param name="clusters">pointer to array of cluster mapping information to fill</param>
    /// <param name="clusterFlags">
    /// pointer to location to store cluster flags corresponding to the output <paramref name="clusters"/>
    /// </param>
    /// <param name="useClusterMapping">whether cluster mapping should be used or not</param>
    /// <returns>array of glyphs</returns>
    /// <remarks>
    /// For details of how <paramref name="clusters"/>, and <paramref name="clusterFlags"/> map input UTF-8 text
    /// to the output glyphs see
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, string, ReadOnlySpan{Glyph}, ReadOnlySpan{TextCluster}, ClusterFlags)"/>.
    /// <para>
    /// The output values can be readily passed to
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, string, ReadOnlySpan{Glyph}, ReadOnlySpan{TextCluster}, ClusterFlags)"/>,
    /// <see cref="TextExtensions.ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>, or related methods, assuming that the exact
    /// same <see cref="ScaledFont"/> is used for the operation.
    /// </para>
    /// </remarks>
    public GlyphArray TextToGlyphs(PointD point, string text, out TextClusterArray? clusters, out ClusterFlags clusterFlags, bool useClusterMapping = true)
        => this.TextToGlyphs(point.X, point.Y, text, out clusters, out clusterFlags, useClusterMapping);

    /// <summary>
    /// Gets the font face that this scaled font uses. This might be the font face passed to
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>, but this does
    /// not hold true for all possible cases.
    /// </summary>
    public FontFace FontFace
    {
        get
        {
            this.CheckDisposed();

            void* handle= cairo_scaled_font_get_font_face(this.Handle);
            return new ScaledFont(handle, owner: false);
        }
    }

    /// <summary>
    /// Gets the font options with which <see cref="ScaledFont"/> was created.
    /// </summary>
    [MaybeNull]
    public FontOptions FontOptions
    {
        get
        {
            this.CheckDisposed();

            if (_fontOptions is null)
            {
                return null;
            }

            cairo_scaled_font_get_font_options(this.Handle, _fontOptions.Handle);
            return _fontOptions;
        }
    }

    /// <summary>
    /// Gets the font matrix with which <see cref="ScaledFont"/> was created.
    /// </summary>
    public void GetFontMatrix(out Matrix fontMatrix)
    {
        this.CheckDisposed();
        cairo_scaled_font_get_font_matrix(this.Handle, out fontMatrix);
    }

    /// <summary>
    /// Gets the CTM with which <see cref="ScaledFont"/> was created.
    /// </summary>
    /// <remarks>
    /// Note that the translation offsets (x0, y0) of the CTM are ignored by
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>. So, the
    /// matrix this method returns always has 0,0 as x0,y0.
    /// </remarks>
    public void GetCtm(out Matrix ctm)
    {
        this.CheckDisposed();
        cairo_scaled_font_get_ctm(this.Handle, out ctm);
    }

    /// <summary>
    /// Gets the scale matrix with which <see cref="ScaledFont"/> was created.
    /// </summary>
    /// <remarks>
    /// The scale matrix is product of the font matrix and the ctm associated with the scaled font,
    /// and hence is the matrix mapping from font space to device space.
    /// </remarks>
    public void GetScaleMatrix(out Matrix scaleMatrix)
    {
        this.CheckDisposed();
        cairo_scaled_font_get_scale_matrix(this.Handle, out scaleMatrix);
    }

    /// <summary>
    /// This property returns the type of the backend used to create a scaled font. See
    /// <see cref="FontType"/> for available types. However, this property never returns
    /// <see cref="FontType.Toy"/>.
    /// </summary>
    public new FontType FontType
    {
        get
        {
            this.CheckDisposed();
            return cairo_scaled_font_get_type(this.Handle);
        }
    }

    /// <summary>
    /// Returns the current reference count of scaled_font.
    /// </summary>
    internal new int ReferenceCount
    {
        get
        {
            this.CheckDisposed();
            return (int)cairo_scaled_font_get_reference_count(this.Handle);
        }
    }

    internal static new ScaledFont? Lookup(void* handle, bool owner = false)
    {
        if (handle is null)
        {
            return null;
        }

        return new ScaledFont(handle, owner);
    }
}
