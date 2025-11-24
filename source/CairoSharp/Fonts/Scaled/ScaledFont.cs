// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Drawing.Text;
using static Cairo.Fonts.Scaled.ScaledFontNative;

namespace Cairo.Fonts.Scaled;

/// <summary>
/// <see cref="ScaledFont"/> — Font face at particular size and options
/// </summary>
/// <remarks>
/// <see cref="ScaledFont"/> represents a realization of a <see cref="Fonts.FontFace"/> at a
/// particular size and transformation and a certain set of <see cref="FontOptions"/>.
/// </remarks>
public sealed unsafe class ScaledFont : FontFace
{
    internal ScaledFont(cairo_scaled_font_t* scaledFont, bool isOwnedByCairo, bool needsDestroy = true)
        : base((cairo_font_face_t*)scaledFont, isOwnedByCairo, needsDestroy, &ScaledFontReference) { }

    private static cairo_font_face_t* ScaledFontReference(cairo_font_face_t* fontFace)
    {
        return (cairo_font_face_t*)cairo_scaled_font_reference((cairo_scaled_font_t*)fontFace);
    }

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
        : base((cairo_font_face_t*)cairo_scaled_font_create(fontFace.Handle, ref fontMatrix, ref ctm, fontOptions.Handle)) { }

    protected override void DisposeCore(cairo_font_face_t* fontFace)
    {
        cairo_scaled_font_t* scaledFont = (cairo_scaled_font_t*)fontFace;
        cairo_scaled_font_destroy(scaledFont);

        PrintDebugInfo(scaledFont);
        [Conditional("DEBUG")]
        static void PrintDebugInfo(cairo_scaled_font_t* scaledFont)
        {
            uint rc = cairo_scaled_font_get_reference_count(scaledFont);
            Debug.WriteLine($"ScaledFont 0x{(nint)scaledFont}: reference count = {rc}");
        }
    }

    private cairo_scaled_font_t* ScaledFontHandle => (cairo_scaled_font_t*)this.Handle;

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
            return cairo_scaled_font_status(this.ScaledFontHandle);
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

            cairo_scaled_font_extents(this.ScaledFontHandle, out FontExtents fontExtents);
            return fontExtents;
        }
    }

    /// <summary>
    /// Gets the extents for a string of text. The extents describe a user-space rectangle
    /// that encloses the "inked" portion of the text drawn at the origin (0,0) (as it would
    /// be drawn by <see cref="TextExtensions.ShowText(CairoContext, string?)"/> if the cairo graphics state were
    /// set to the same font_face, font_matrix, ctm, and font_options as scaled_font).
    /// Additionally, the <see cref="TextExtents.XAdvance"/> and <see cref="TextExtents.YAdvance"/> values
    /// indicate the amount by which the current point would be advanced by
    /// <see cref="TextExtensions.ShowText(CairoContext, string?)"/>.
    /// </summary>
    /// <param name="text">a string of text</param>
    /// <param name="textExtents">a <see cref="Fonts.TextExtents"/> which to store the retrieved extents.</param>
    /// <remarks>
    /// Note that whitespace characters do not directly contribute to the size of the rectangle
    /// (extents.width and extents.height). They do contribute indirectly by changing the position
    /// of non-whitespace characters. In particular, trailing whitespace characters are likely to not
    /// affect the size of the rectangle, though they will affect the x_advance and y_advance values.
    /// </remarks>
    public void TextExtents(string text, out TextExtents textExtents)
    {
        this.CheckDisposed();
        cairo_scaled_font_text_extents(this.ScaledFontHandle, text, out textExtents);
    }

    /// <summary>
    /// Gets the extents for a string of text. The extents describe a user-space rectangle
    /// that encloses the "inked" portion of the text drawn at the origin (0,0) (as it would
    /// be drawn by <see cref="TextExtensions.ShowText(CairoContext, ReadOnlySpan{byte})"/> if the
    /// cairo graphics state were set to the same font_face, font_matrix, ctm, and font_options as scaled_font).
    /// Additionally, the <see cref="TextExtents.XAdvance"/> and <see cref="TextExtents.YAdvance"/> values
    /// indicate the amount by which the current point would be advanced by
    /// <see cref="TextExtensions.ShowText(CairoContext, ReadOnlySpan{byte})"/>.
    /// </summary>
    /// <param name="utf8">a NUL-terminated string of text, encoded in UTF-8</param>
    /// <param name="textExtents">a <see cref="Fonts.TextExtents"/> which to store the retrieved extents.</param>
    /// <remarks>
    /// Note that whitespace characters do not directly contribute to the size of the rectangle
    /// (extents.width and extents.height). They do contribute indirectly by changing the position
    /// of non-whitespace characters. In particular, trailing whitespace characters are likely to not
    /// affect the size of the rectangle, though they will affect the x_advance and y_advance values.
    /// </remarks>
    public void TextExtents(ReadOnlySpan<byte> utf8, out TextExtents textExtents)
    {
        this.CheckDisposed();
        cairo_scaled_font_text_extents(this.ScaledFontHandle, utf8, out textExtents);
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
    /// <param name="textExtents">a <see cref="Fonts.TextExtents"/> which to store the retrieved extents.</param>
    public void GlyphExtents(ReadOnlySpan<Glyph> glyphs, out TextExtents textExtents)
    {
        this.CheckDisposed();

        fixed (Glyph* ptr = glyphs)
        {
            cairo_scaled_font_glyph_extents(this.ScaledFontHandle, ptr, glyphs.Length, out textExtents);
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
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, string, GlyphArray, TextClusterArray, ClusterFlags)"/>.
    /// <para>
    /// The output values can be readily passed to
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, string, GlyphArray, TextClusterArray, ClusterFlags)"/>,
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
            fixed (ClusterFlags* clusterFlagsPinned = &clusterFlags)
            {
                TextCluster* clustersNative;
                int numClusters;

                Status status = cairo_scaled_font_text_to_glyphs(
                    this.ScaledFontHandle,
                    x, y,
                    text, -1,
                    out Glyph* glyphs, out int numGlyphs,
                    &clustersNative, &numClusters, clusterFlagsPinned);

                status.ThrowIfNotSuccess();

                clusters = new TextClusterArray(clustersNative, numClusters);
                return new GlyphArray(glyphs, numGlyphs);
            }
        }
        else
        {
            Status status = cairo_scaled_font_text_to_glyphs(
                this.ScaledFontHandle,
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
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, string, GlyphArray, TextClusterArray, ClusterFlags)"/>.
    /// <para>
    /// The output values can be readily passed to
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, string, GlyphArray, TextClusterArray, ClusterFlags)"/>,
    /// <see cref="TextExtensions.ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>, or related methods, assuming that the exact
    /// same <see cref="ScaledFont"/> is used for the operation.
    /// </para>
    /// </remarks>
    /// <example>
    /// See <a href="https://www.codeproject.com/articles/Programming-Cairo-Text-Output-Beyond-the-toy-text">sample.</a>
    /// </example>
    public GlyphArray TextToGlyphs(PointD point, string text, out TextClusterArray? clusters, out ClusterFlags clusterFlags, bool useClusterMapping = true)
        => this.TextToGlyphs(point.X, point.Y, text, out clusters, out clusterFlags, useClusterMapping);

    /// <summary>
    /// Converts UTF-8 text to an array of glyphs, optionally with cluster mapping, that can be used to
    /// render later using <see cref="ScaledFont"/>.
    /// </summary>
    /// <param name="x">X position to place first glyph</param>
    /// <param name="y">Y position to place first glyph</param>
    /// <param name="utf8">a string of text encoded in UTF-8</param>
    /// <param name="clusters">pointer to array of cluster mapping information to fill</param>
    /// <param name="clusterFlags">
    /// pointer to location to store cluster flags corresponding to the output <paramref name="clusters"/>
    /// </param>
    /// <param name="useClusterMapping">whether cluster mapping should be used or not</param>
    /// <returns>array of glyphs</returns>
    /// <remarks>
    /// For details of how <paramref name="clusters"/>, and <paramref name="clusterFlags"/> map input UTF-8 text
    /// to the output glyphs see
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, ReadOnlySpan{byte}, GlyphArray, TextClusterArray, ClusterFlags)"/>.
    /// <para>
    /// The output values can be readily passed to
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, ReadOnlySpan{byte}, GlyphArray, TextClusterArray, ClusterFlags)"/>,
    /// <see cref="TextExtensions.ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>, or related methods, assuming that the exact
    /// same <see cref="ScaledFont"/> is used for the operation.
    /// </para>
    /// </remarks>
    /// <example>
    /// See <a href="https://www.codeproject.com/articles/Programming-Cairo-Text-Output-Beyond-the-toy-text">sample.</a>
    /// </example>
    public GlyphArray TextToGlyphs(double x, double y, ReadOnlySpan<byte> utf8, out TextClusterArray? clusters, out ClusterFlags clusterFlags, bool useClusterMapping = true)
    {
        this.CheckDisposed();

        if (useClusterMapping)
        {
            fixed (ClusterFlags* clusterFlagsPinned = &clusterFlags)
            {
                TextCluster* clustersNative;
                int numClusters;

                Status status = cairo_scaled_font_text_to_glyphs(
                    this.ScaledFontHandle,
                    x, y,
                    utf8, utf8.Length,
                    out Glyph* glyphs, out int numGlyphs,
                    &clustersNative, &numClusters, clusterFlagsPinned);

                status.ThrowIfNotSuccess();

                clusters = new TextClusterArray(clustersNative, numClusters);
                return new GlyphArray(glyphs, numGlyphs);
            }
        }
        else
        {
            Status status = cairo_scaled_font_text_to_glyphs(
                this.ScaledFontHandle,
                x, y,
                utf8, utf8.Length,
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
    /// <param name="utf8">a string of text encoded in UTF-8</param>
    /// <param name="clusters">pointer to array of cluster mapping information to fill</param>
    /// <param name="clusterFlags">
    /// pointer to location to store cluster flags corresponding to the output <paramref name="clusters"/>
    /// </param>
    /// <param name="useClusterMapping">whether cluster mapping should be used or not</param>
    /// <returns>array of glyphs</returns>
    /// <remarks>
    /// For details of how <paramref name="clusters"/>, and <paramref name="clusterFlags"/> map input UTF-8 text
    /// to the output glyphs see
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, ReadOnlySpan{byte}, GlyphArray, TextClusterArray, ClusterFlags)"/>.
    /// <para>
    /// The output values can be readily passed to
    /// <see cref="TextExtensions.ShowTextGlyphs(CairoContext, ReadOnlySpan{byte}, GlyphArray, TextClusterArray, ClusterFlags)"/>,
    /// <see cref="TextExtensions.ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>, or related methods, assuming that the exact
    /// same <see cref="ScaledFont"/> is used for the operation.
    /// </para>
    /// </remarks>
    /// <example>
    /// See <a href="https://www.codeproject.com/articles/Programming-Cairo-Text-Output-Beyond-the-toy-text">sample.</a>
    /// </example>
    public GlyphArray TextToGlyphs(PointD point, ReadOnlySpan<byte> utf8, out TextClusterArray? clusters, out ClusterFlags clusterFlags, bool useClusterMapping = true)
        => this.TextToGlyphs(point.X, point.Y, utf8, out clusters, out clusterFlags, useClusterMapping);

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

            cairo_font_face_t* fontFace = cairo_scaled_font_get_font_face(this.ScaledFontHandle);
            return new FontFace(fontFace, isOwnedByCairo: true, referenceFunc: &ScaledFontReference);
        }
    }

    /// <summary>
    /// Stores the font options with which this <see cref="ScaledFont"/> was created into <paramref name="options"/>.
    /// </summary>
    /// <param name="options">return value for the font options</param>
    public void GetFontOptions(FontOptions options)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(options);

        cairo_scaled_font_get_font_options(this.ScaledFontHandle, options.Handle);
    }

    /// <summary>
    /// Gets the font matrix with which <see cref="ScaledFont"/> was created.
    /// </summary>
    public void GetFontMatrix(out Matrix fontMatrix)
    {
        this.CheckDisposed();
        cairo_scaled_font_get_font_matrix(this.ScaledFontHandle, out fontMatrix);
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
        cairo_scaled_font_get_ctm(this.ScaledFontHandle, out ctm);
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
        cairo_scaled_font_get_scale_matrix(this.ScaledFontHandle, out scaleMatrix);
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
            return cairo_scaled_font_get_type(this.ScaledFontHandle);
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
            return (int)cairo_scaled_font_get_reference_count(this.ScaledFontHandle);
        }
    }
}
