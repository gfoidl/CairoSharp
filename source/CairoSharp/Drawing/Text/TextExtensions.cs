// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Drawing.Text;
using Cairo.Fonts;
using Cairo.Fonts.Scaled;
using static Cairo.Drawing.Text.TextNative;

namespace Cairo;

/// <summary>
/// text — Rendering text and glyphs
/// </summary>
/// <remarks>
/// The functions with text in their name form cairo's toy text API. The toy API takes UTF-8 encoded
/// text and is limited in its functionality to rendering simple left-to-right text with no
/// advanced features. That means for example that most complex scripts like Hebrew, Arabic, and Indic
/// scripts are out of question. No kerning or correct positioning of diacritical marks either.
/// The font selection is pretty limited too and doesn't handle the case that the selected font does
/// not cover the characters in the text. This set of functions are really that, a toy text API,
/// for testing and demonstration purposes. Any serious application should avoid them.
/// <para>
/// The functions with glyphs in their name form cairo's low-level text API. The low-level API
/// relies on the user to convert text to a set of glyph indexes and positions. This is a very hard
/// problem and is best handled by external libraries, like the pangocairo that is part of the Pango
/// text layout and rendering library. Pango is available from http://www.pango.org/.
/// </para>
/// </remarks>
public static unsafe class TextExtensions
{
    extension(CairoContext cr)
    {
        /// <summary>
        /// Selects a family and style of font from a simplified description as a family name, slant and
        /// weight. Cairo provides no operation to list available family names on the system (this is a "toy",
        /// remember), but the standard CSS2 generic family names, ("serif", "sans-serif", "cursive", "fantasy",
        /// "monospace"), are likely to work as expected.
        /// </summary>
        /// <param name="family">a font family name, encoded in UTF-8</param>
        /// <param name="slant">the slant for the font</param>
        /// <param name="weight">the weight for the font</param>
        /// <remarks>
        /// If family starts with the string "@cairo:", or if no native font backends are compiled in, cairo will
        /// use an internal font family. The internal font family recognizes many modifiers in the family string,
        /// most notably, it recognizes the string "monospace". That is, the family name "@cairo:monospace" will
        /// use the monospace version of the internal font family.
        /// <para>
        /// For "real" font selection, see the font-backend-specific font_face_create functions for the font backend
        /// you are using. (For example, if you are using the freetype-based cairo-ft font backend, see
        /// cairo_ft_font_face_create_for_ft_face() or cairo_ft_font_face_create_for_pattern().) The resulting font
        /// face could then be used with cairo_scaled_font_create() and cairo_set_scaled_font().
        /// </para>
        /// <para>
        /// Similarly, when using the "real" font support, you can call directly into the underlying font system,
        /// (such as fontconfig or freetype), for operations such as listing available fonts, etc.
        /// </para>
        /// <para>
        /// It is expected that most applications will need to use a more comprehensive font handling and text layout
        /// library, (for example, pango), in conjunction with cairo.
        /// </para>
        /// <para>
        /// If text is drawn without a call to cairo_select_font_face(), (nor cairo_set_font_face() nor cairo_set_scaled_font()),
        /// the default family is platform-specific, but is essentially "sans-serif". Default slant is
        /// <see cref="FontSlant.Normal"/>, and default weight is <see cref="FontWeight.Normal"/>.
        /// </para>
        /// <para>
        /// This method is equivalent to a call to cairo_toy_font_face_create() followed by cairo_set_font_face().
        /// </para>
        /// </remarks>
        public void SelectFontFace(string family, FontSlant slant = FontSlant.Normal, FontWeight weight = FontWeight.Normal)
        {
            cr.CheckDisposed();
            cairo_select_font_face(cr.Handle, family, slant, weight);
        }

        /// <summary>
        /// Sets the current font matrix to a scale by a factor of size, replacing any font matrix previously set
        /// with cairo_set_font_size() or cairo_set_font_matrix(). This results in a font size of size user space units.
        /// (More precisely, this matrix will result in the font's em-square being a size by size square in user space.)
        /// </summary>
        /// <remarks>
        /// If text is drawn without a call to <see cref="set_FontSize(CairoContext, double)"/>(),
        /// (nor cairo_set_font_matrix() nor cairo_set_scaled_font()), the default font size is 10.0.
        /// </remarks>
        public double FontSize
        {
            set
            {
                cr.CheckDisposed();
                cairo_set_font_size(cr.Handle, value);
            }
        }

        /// <summary>
        /// Sets the current font matrix to matrix. The font matrix gives a transformation from the design
        /// space of the font (in this space, the em-square is 1 unit by 1 unit) to user space.
        /// Normally, a simple scale is used (see cairo_set_font_size()), but a more complex font matrix
        /// can be used to shear the font or stretch it unequally along the two axes
        /// </summary>
        /// <param name="matrix">a <see cref="Matrix"/> describing a transform to be applied to the current font.</param>
        public void SetFontMatrix(ref Matrix matrix)
        {
            cr.CheckDisposed();
            cairo_set_font_matrix(cr.Handle, ref matrix);
        }

        /// <summary>
        /// Stores the current font matrix into matrix.
        /// </summary>
        /// <param name="matrix">return value for the matrix</param>
        /// <remarks>
        /// See <see cref="SetFontMatrix(CairoContext, ref Matrix)"/>
        /// </remarks>
        public void GetFontMatrix(out Matrix matrix)
        {
            cr.CheckDisposed();
            cairo_get_font_matrix(cr.Handle, out matrix);
        }

        /// <summary>
        /// Sets a set of custom font rendering options for the <see cref="CairoContext"/>. Rendering
        /// options are derived by merging these options with the options derived from underlying surface;
        /// if the value in options has a default value (like <see cref="Antialias.Default"/>), then the
        /// value from the surface is used.
        /// <para>
        /// Get retrieves font rendering options set via the setter. Note that the returned options do not
        /// include any options derived from the underlying surface; they are literally the options
        /// passed to the setter.
        /// </para>
        /// </summary>
        public FontOptions FontOptions
        {
            get
            {
                cr.CheckDisposed();

                FontOptions fontOptions = new();
                cairo_get_font_options(cr.Handle, fontOptions.Handle);
                return fontOptions;
            }
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                cr.CheckDisposed();
                cairo_set_font_options(cr.Handle, value.Handle);
            }
        }

        /// <summary>
        /// Replaces the current <see cref="FontFace"/> object in the <see cref="CairoContext"/> with
        /// <see cref="FontFace"/>. The replaced font face in the <see cref="CairoContext"/> will be destroyed
        /// if there are no other references to it.
        /// <para>
        /// When <c>null</c> is set, the default font is restored.
        /// </para>
        /// </summary>
        public FontFace FontFace
        {
            get
            {
                cr.CheckDisposed();

                void* handle = cairo_get_font_face(cr.Handle);
                return new FontFace(handle, isOwnedByCairo: true);
            }
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                cr.CheckDisposed();
                cairo_set_font_face(cr.Handle, value.Handle);
            }
        }

        /// <summary>
        /// Replaces the current font face, font matrix, and font options in the <see cref="CairoContext"/> with
        /// those of the <see cref="ScaledFont"/>. Except for some translation, the current CTM of the <see cref="CairoContext"/>
        /// should be the same as that of the <see cref="ScaledFont"/>, which can be accessed using cairo_scaled_font_get_ctm().
        /// </summary>
        public ScaledFont ScaledFont
        {
            get
            {
                cr.CheckDisposed();

                void* handle = cairo_get_scaled_font(cr.Handle);
                return new ScaledFont(handle, isOwnedByCairo: true);
            }
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                cr.CheckDisposed();
                cairo_set_scaled_font(cr.Handle, value.Handle);
            }
        }

        /// <summary>
        /// A drawing operator that generates the shape from a string of UTF-8 characters,
        /// rendered according to the current font_face, font_size (font_matrix), and font_options.
        /// </summary>
        /// <param name="text">string to render or <c>null</c></param>
        /// <remarks>
        /// This method first computes a set of glyphs for the string of text. The first glyph is
        /// placed so that its origin is at the current point. The origin of each subsequent glyph
        /// is offset from that of the previous glyph by the advance values of the previous glyph.
        /// <para>
        /// After this call the current point is moved to the origin of where the next glyph would be placed
        /// in this same progression. That is, the current point will be at the origin of the final glyph offset
        /// by its advance values. This allows for easy display of a single logical string with multiple
        /// calls to <see cref="ShowText(CairoContext, string?)"/>.
        /// </para>
        /// <para>
        /// Note: The <see cref="ShowText(CairoContext, string?)"/> method call is part of what the cairo
        /// designers call the "toy" text API. It is convenient for short demos and simple programs, but it is
        /// not expected to be adequate for serious text-using applications. See <see cref="ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>
        /// for the "real" text display API in cairo.
        /// </para>
        /// </remarks>
        public void ShowText(string? text)
        {
            cr.CheckDisposed();

            if (text is null)
            {
                return;
            }

            cairo_show_text(cr.Handle, text);
        }

        /// <summary>
        /// A drawing operator that generates the shape from an array of glyphs, rendered according to
        /// the current font face, font size (font matrix), and font options.
        /// </summary>
        /// <param name="glyphs">array of glyphs to show</param>
        public void ShowGlyphs(params ReadOnlySpan<Glyph> glyphs)
        {
            cr.CheckDisposed();

            fixed (Glyph* ptr = glyphs)
            {
                cairo_show_glyphs(cr.Handle, ptr, glyphs.Length);
            }
        }

        /// <summary>
        /// A drawing operator that generates the shape from an array of glyphs, rendered according to
        /// the current font face, font size (font matrix), and font options.
        /// </summary>
        /// <param name="glyphs">array of glyphs to show</param>
        public void ShowGlyphs(GlyphArray glyphs) => cr.ShowGlyphs(glyphs.Span);

        /// <summary>
        /// This operation has rendering effects similar to <see cref="ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>
        /// but, if the target surface supports it, uses the provided text and cluster mapping to embed the text for
        /// the glyphs shown in the output. If the target does not support the extended attributes, this function acts
        /// like the basic <see cref="ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/> as if it had been passed glyphs.
        /// </summary>
        /// <param name="text">string of text</param>
        /// <param name="glyphs">array of glyphs to show</param>
        /// <param name="clusters">array of cluster mapping information</param>
        /// <param name="clusterFlags">cluster mapping flags</param>
        /// <remarks>
        /// The mapping between utf8 and glyphs is provided by an array of clusters. Each cluster covers a number of
        /// text bytes and glyphs, and neighboring clusters cover neighboring areas of utf8 and glyphs. The clusters
        /// should collectively cover utf8 and glyphs in entirety.
        /// <para>
        /// The first cluster always covers bytes from the beginning of utf8. If <paramref name="clusterFlags"/> do not have
        /// the <see cref="ClusterFlags.Backward"/>set, the first cluster also covers the beginning of glyphs, otherwise it
        /// covers the end of the glyphs array and following clusters move backward.
        /// </para>
        /// <para>
        /// See <see cref="TextCluster"/> for constraints on valid clusters.
        /// </para>
        /// </remarks>
        /// <example>
        /// See <a href="https://www.codeproject.com/articles/Programming-Cairo-Text-Output-Beyond-the-toy-text">sample.</a>
        /// </example>
        public void ShowTextGlyphs(string text, ReadOnlySpan<Glyph> glyphs, ReadOnlySpan<TextCluster> clusters, ClusterFlags clusterFlags = ClusterFlags.None)
        {
            cr.CheckDisposed();

            fixed (Glyph* glyphNative             = glyphs)
            fixed (TextCluster* textClusterNative = clusters)
            {
                // -1 as the marshalled string is null-terminated
                cairo_show_text_glyphs(cr.Handle, text, -1, glyphNative, glyphs.Length, textClusterNative, clusters.Length, clusterFlags);
            }
        }

        /// <summary>
        /// This operation has rendering effects similar to <see cref="ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>
        /// but, if the target surface supports it, uses the provided text and cluster mapping to embed the text for
        /// the glyphs shown in the output. If the target does not support the extended attributes, this method acts
        /// like the basic <see cref="ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/> as if it had been passed glyphs.
        /// </summary>
        /// <param name="text">string of text</param>
        /// <param name="glyphs">array of glyphs to show</param>
        /// <param name="clusters">array of cluster mapping information</param>
        /// <param name="clusterFlags">cluster mapping flags</param>
        /// <remarks>
        /// The mapping between utf8 and glyphs is provided by an array of clusters. Each cluster covers a number of
        /// text bytes and glyphs, and neighboring clusters cover neighboring areas of utf8 and glyphs. The clusters
        /// should collectively cover utf8 and glyphs in entirety.
        /// <para>
        /// The first cluster always covers bytes from the beginning of utf8. If <paramref name="clusterFlags"/> do not have
        /// the <see cref="ClusterFlags.Backward"/>set, the first cluster also covers the beginning of glyphs, otherwise it
        /// covers the end of the glyphs array and following clusters move backward.
        /// </para>
        /// <para>
        /// See <see cref="TextCluster"/> for constraints on valid clusters.
        /// </para>
        /// </remarks>
        /// <example>
        /// See <a href="https://www.codeproject.com/articles/Programming-Cairo-Text-Output-Beyond-the-toy-text">sample.</a>
        /// </example>
        public void ShowTextGlyphs(string text, GlyphArray glyphs, TextClusterArray clusters, ClusterFlags clusterFlags = ClusterFlags.None)
            => cr.ShowTextGlyphs(text, glyphs.Span, clusters.Span, clusterFlags);

        /// <summary>
        /// This operation has rendering effects similar to <see cref="ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>
        /// but, if the target surface supports it, uses the provided text and cluster mapping to embed the text for
        /// the glyphs shown in the output. If the target does not support the extended attributes, this method acts
        /// like the basic <see cref="ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/> as if it had been passed glyphs.
        /// </summary>
        /// <param name="text">string of text</param>
        /// <param name="useClusterMapping">whether cluster mapping should be used or not</param>
        /// <remarks>
        /// This is convenience method that performs
        /// <see cref="ScaledFont.TextToGlyphs(double, double, string, out TextClusterArray?, out ClusterFlags, bool)"/>
        /// and
        /// <see cref="ShowTextGlyphs(CairoContext, string, GlyphArray, TextClusterArray, ClusterFlags)"/>
        /// combined.
        /// </remarks>
        public void ShowTextGlyphs(string text, bool useClusterMapping = true)
        {
            cr.CheckDisposed();

            // https://www.cairographics.org/manual/cairo-cairo-scaled-font-t.html#cairo-scaled-font-text-to-glyphs

            using ScaledFont scaledFont = cr.ScaledFont;
            PointD currentPoint         = cr.CurrentPoint;
            GlyphArray glyphs           = scaledFont.TextToGlyphs(currentPoint, text, out TextClusterArray? clusters, out ClusterFlags clusterFlags, useClusterMapping);

            Debug.Assert(useClusterMapping && clusters is not null);

            try
            {
                if (useClusterMapping)
                {
                    cr.ShowTextGlyphs(text, glyphs, clusters, clusterFlags);
                }
                else
                {
                    cr.ShowGlyphs(glyphs);
                }
            }
            finally
            {
                glyphs   .Dispose();
                clusters?.Dispose();
            }
        }

        /// <summary>
        /// Gets the font extents for the currently selected font.
        /// </summary>
        /// <param name="extents">a <see cref="Fonts.FontExtents"/> object into which the results will be stored.</param>
        public void FontExtents(out FontExtents extents)
        {
            cr.CheckDisposed();
            cairo_font_extents(cr.Handle, out extents);
        }

        /// <summary>
        /// Gets the extents for a string of text. The extents describe a user-space rectangle that encloses the
        /// "inked" portion of the text, (as it would be drawn by <see cref="ShowText(CairoContext, string?)"/>).
        /// Additionally, the <see cref="TextExtents.XAdvance"/> and <see cref="TextExtents.YAdvance"/> values
        /// indicate the amount by which the current point would be advanced by <see cref="ShowText(CairoContext, string?)"/>.
        /// </summary>
        /// <param name="text">a string, or <c>null</c></param>
        /// <param name="extents">a <see cref="Fonts.TextExtents"/> object into which the results will be stored</param>
        /// <remarks>
        /// Note that whitespace characters do not directly contribute to the size of the rectangle (extents.width and extents.height).
        /// They do contribute indirectly by changing the position of non-whitespace characters. In particular, trailing
        /// whitespace characters are likely to not affect the size of the rectangle, though they will affect
        /// the <see cref="TextExtents.XAdvance"/> and <see cref="TextExtents.YAdvance"/> values.
        /// </remarks>
        public void TextExtents(string? text, out TextExtents extents)
        {
            cr.CheckDisposed();
            cairo_text_extents(cr.Handle, text, out extents);
        }

        /// <summary>
        /// Gets the extents for an array of glyphs. The extents describe a user-space rectangle that encloses the
        /// "inked" portion of the glyphs, (as they would be drawn by <see cref="ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>).
        /// Additionally, the <see cref="TextExtents.XAdvance"/> and <see cref="TextExtents.YAdvance"/> values indicate the
        /// amount by which the current point would be advanced by <see cref="ShowGlyphs(CairoContext, ReadOnlySpan{Glyph})"/>.
        /// </summary>
        /// <param name="glyphs">an array of <see cref="Glyph"/> objects</param>
        /// <param name="extents">a <see cref="Fonts.TextExtents"/> object into which the results will be stored</param>
        /// <remarks>
        /// Note that whitespace glyphs do not contribute to the size of the rectangle (extents.width and extents.height).
        /// </remarks>
        public void GlyphExtents(ReadOnlySpan<Glyph> glyphs, out TextExtents extents)
        {
            cr.CheckDisposed();

            fixed (Glyph* ptr = glyphs)
            {
                cairo_glyph_extents(cr.Handle, ptr, glyphs.Length, out extents);
            }
        }
    }
}
