// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Extensions.GObject;
using Cairo.Extensions.Loading;
using static Cairo.Extensions.Pango.PangoNative;

namespace Cairo.Extensions.Pango;

/// <summary>
/// A <see cref="PangoLayout"/> represents an entire paragraph of text.
/// </summary>
public sealed unsafe class PangoLayout : CairoObject<pango_layout>
{
    private readonly CairoContext _cr;
    private bool                  _inMarkupMode;

    /// <summary>
    /// Creates a layout object set up to match the current transformation and target
    /// surface of the <see cref="CairoContext"/>.
    /// </summary>
    /// <param name="cr">A cairo context.</param>
    /// <param name="resolution">
    /// See <see cref="Resolution"/>. As default value for this parameter 72 dpi is used, so that
    /// font sizes given in Pango match the font size given in cairo's user space units.
    /// </param>
    /// <remarks>
    /// This layout can then be used for text measurement with functions like <see cref="GetSize(out double, out double)"/>
    /// or drawing with functions like <see cref="ShowLayout"/>. If you change the transformation or target
    /// surface for <paramref name="cr"/>, you need to call <see cref="UpdateLayout"/>.
    /// <para>
    /// This constructor is the most convenient way to use cairo with Pango, however it is slightly
    /// inefficient since it creates a separate PangoContext object for each layout. This might matter
    /// in an application that was laying out large amounts of text.
    /// </para>
    /// <para>
    /// If no font description is set on the layout, the font description from the layout’s context is used.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="cr"/> is <c>null</c></exception>
    public PangoLayout(CairoContext cr, double resolution = 72d) : base(Create(cr))
    {
        _cr = cr;

        if (resolution != Pango.DefaultResolution)
        {
            this.Resolution = resolution;
        }
    }

    [StackTraceHidden]
    private static pango_layout* Create(CairoContext cr)
    {
        ArgumentNullException.ThrowIfNull(cr);
        cr.CheckDisposed();

        return pango_cairo_create_layout(cr.Handle);
    }

    protected override void DisposeCore(pango_layout* handle)
    {
        Debug.Assert(LibGObjectName == LoadingNative.LibGObjectName);

        GObjectNative.g_object_unref(handle);
    }

    /// <summary>
    /// Sets the default font description for the layout.
    /// </summary>
    /// <param name="fontDescription">
    /// String representation of a font description. The string must have the form
    /// <code>
    /// [FAMILY-LIST] [STYLE-OPTIONS] [SIZE] [VARIATIONS] [FEATURES]
    /// </code>
    /// <para>
    /// See
    /// <a href="https://docs.gtk.org/Pango/type_func.FontDescription.from_string.html#description">Pango docs</a>
    /// for details on how the string should look like.
    /// </para>
    /// <para>
    /// When <c>null</c> is set, the current font description is unset.
    /// </para>
    /// </param>
    /// <remarks>
    /// If no font description is set on the layout, the font description from the layout’s context is used.
    /// </remarks>
    public void SetFontDescriptionFromString(string? fontDescription)
    {
        this.CheckDisposed();

        if (fontDescription is null)
        {
            pango_layout_set_font_description(this.Handle, null);
        }
        else
        {
            pango_font_description* desc = pango_font_description_from_string(fontDescription);
            pango_layout_set_font_description(this.Handle, desc);
            pango_font_description_free(desc);
        }
    }

    /// <summary>
    /// Sets the font description that matches the face.
    /// </summary>
    /// <param name="fontFace">The font face.</param>
    /// <param name="size">
    /// The size of the font in points
    /// </param>
    /// <remarks>
    /// The resulting font description will have the family, style, variant, weight and stretch
    /// of the face, but its size field will be unset.
    /// </remarks>
    public void SetFontDescription(PangoFontFace fontFace, int size)
    {
        this.CheckDisposed();
        fontFace.CheckDisposed();
        ArgumentNullException.ThrowIfNull(fontFace);

        pango_font_description* desc = PangoFontFaceNative.pango_font_face_describe(fontFace.Handle);

        // Must be set before assigning it to the layout.
        pango_font_description_set_size(desc, size * Pango.Scale);
        //pango_font_description_set_absolute_size(desc, size * Pango.Scale);

        pango_layout_set_font_description(this.Handle, desc);
        pango_font_description_free(desc);
    }

    /// <summary>
    /// Determines the logical width and height of a <see cref="PangoLayout"/> in cairo units.
    /// </summary>
    /// <param name="width">the logical width</param>
    /// <param name="height">the logical height</param>
    /// <remarks>
    /// Note: the native Pango API returns the values in Pango units, whilst this API
    /// returns cairo units.<br />
    /// <code>
    /// Pango unit = cairo unit * Pango.Scale
    /// </code>
    /// </remarks>
    public void GetSize(out double width, out double height)
    {
        this.CheckDisposed();
        pango_layout_get_size(this.Handle, out int w, out int h);

        width  = w / (double)Pango.Scale;
        height = h / (double)Pango.Scale;
    }

    /// <summary>
    /// Retrieves the count of lines for the <see cref="PangoLayout"/>.
    /// </summary>
    public int LineCount
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_get_line_count(this.Handle);
        }
    }

    /// <summary>
    /// Returns the number of Unicode characters in the text of <see cref="PangoLayout"/>.
    /// </summary>
    public int CharacterCount
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_get_character_count(this.Handle);
        }
    }

    /// <summary>
    /// Gets the Y position of baseline of the first line in layout.
    /// </summary>
    /// <returns>Baseline of first line, from top of layout.</returns>
    public int GetBaseline()
    {
        this.CheckDisposed();
        return pango_layout_get_baseline(this.Handle);
    }

    /// <summary>
    /// Adds the text in a <see cref="PangoLayout"/> to the current path in the specified
    /// <see cref="CairoContext"/>.
    /// </summary>
    /// <remarks>
    /// The top-left corner of the <see cref="PangoLayout"/> will be drawn at the current
    /// point of the <see cref="CairoContext"/>.
    /// </remarks>
    public void LayoutPath()
    {
        this.CheckDisposed();
        _cr.CheckDisposed();

        pango_cairo_layout_path(_cr.Handle, this.Handle);
    }

    /// <summary>
    /// Draws a <see cref="PangoLayout"/> in the specified <see cref="CairoContext"/>.
    /// </summary>
    /// <remarks>
    /// The top-left corner of the <see cref="PangoLayout"/> will be drawn at the current
    /// point of the <see cref="CairoContext"/>.
    /// </remarks>
    public void ShowLayout()
    {
        this.CheckDisposed();
        _cr.CheckDisposed();

        pango_cairo_show_layout(_cr.Handle, this.Handle);
    }

    /// <summary>
    /// Updates the private PangoContext of a <see cref="PangoLayout"/> created with
    /// <see cref="PangoLayout(CairoContext, double)"/>
    /// to match the current transformation and target surface of a <see cref="CairoContext"/>.
    /// </summary>
    public void UpdateLayout()
    {
        this.CheckDisposed();
        _cr.CheckDisposed();

        pango_cairo_update_layout(_cr.Handle, this.Handle);
    }

    /// <summary>
    /// Sets the text of the layout.
    /// </summary>
    /// <remarks>
    /// This method validates the value and renders invalid UTF-8 with a placeholder glyph.
    /// </remarks>
    public void SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        this.CheckDisposed();

        if (_inMarkupMode)
        {
            this.ClearMarkup();
        }
        Debug.Assert(!_inMarkupMode);

        pango_layout_set_text(this.Handle, text, -1);
    }

    /// <summary>
    /// Sets the text of the layout.
    /// </summary>
    /// <remarks>
    /// This method validates the value and renders invalid UTF-8 with a placeholder glyph.
    /// </remarks>
    public void SetText(ReadOnlySpan<byte> text)
    {
        this.CheckDisposed();

        if (_inMarkupMode)
        {
            this.ClearMarkup();
        }
        Debug.Assert(!_inMarkupMode);

        pango_layout_set_text(this.Handle, text, -1);
    }

    /// <summary>
    /// Gets the text of the layout.
    /// </summary>
    /// <returns>
    /// The text stored in the layout (set by <see cref="SetText(string)"/> or <see cref="SetText(ReadOnlySpan{byte})"/>).
    /// </returns>
    public string GetText()
    {
        this.CheckDisposed();
        return pango_layout_get_text(this.Handle);
    }

    /// <summary>
    /// Sets the layout text and attribute list from marked-up text.
    /// </summary>
    /// <param name="markup">Marked-up text.</param>
    /// <remarks>
    /// See <a href="https://docs.gtk.org/Pango/pango_markup.html">Pango Markup</a>.
    /// <para>
    /// Replaces the current text and attribute list.
    /// </para>
    /// </remarks>
    public void SetMarkup(string markup)
    {
        this.CheckDisposed();

        pango_layout_set_markup(this.Handle, markup, -1);
        _inMarkupMode = true;
    }

    /// <summary>
    /// Sets the layout text and attribute list from marked-up text.
    /// </summary>
    /// <param name="markup">Marked-up text.</param>
    /// <remarks>
    /// See <a href="https://docs.gtk.org/Pango/pango_markup.html">Pango Markup</a>.
    /// <para>
    /// Replaces the current text and attribute list.
    /// </para>
    /// </remarks>
    public void SetMarkup(ReadOnlySpan<byte> markup)
    {
        this.CheckDisposed();

        pango_layout_set_markup(this.Handle, markup, markup.Length);
        _inMarkupMode = true;
    }

    private void ClearMarkup()
    {
        pango_layout_set_attributes(this.Handle, null);
        _inMarkupMode = false;
    }

    /// <summary>
    /// Gets or sets the resolution for the context.
    /// </summary>
    /// <remarks>
    /// This is a scale factor between points specified in a PangoFontDescription and cairo units.
    /// The default value is 96 in Pango, meaning that a 10 point font will be 13 units high.
    /// (10 * 96. / 72. = 13.3).<br />
    /// Note: <see cref="PangoLayout(CairoContext, double)"/> has a default of 72, so that the units
    /// are equivalent to cairo's units.
    /// <para>
    /// Physical inches aren't actually involved; the terminology is conventional.
    /// </para>
    /// <para>
    /// A 0 or negative value means to use the resolution from the font map.
    /// </para>
    /// <para>
    /// The getter returns the resolution in "dots per inch". A negative value will be returned
    /// if no resolution has previously been set.
    /// </para>
    /// </remarks>
    public double Resolution
    {
        get
        {
            this.CheckDisposed();

            pango_context* context = pango_layout_get_context(this.Handle);
            return pango_cairo_context_get_resolution(context);
        }
        set
        {
            this.CheckDisposed();

            pango_context* context = pango_layout_get_context(this.Handle);
            pango_cairo_context_set_resolution(context, value);
        }
    }

    /// <summary>
    /// Gets or sets the width to which the lines of the <see cref="PangoLayout"/> should
    /// wrap or get ellipsized.<br />
    /// The default value is -1: no width set.
    /// </summary>
    /// <remarks>
    /// The desired width in cairo units, or -1 to indicate that no wrapping or ellipsization
    /// should be performed.
    /// <para>
    /// Note: the native Pango API returns the values in Pango units, whilst this API
    /// returns cairo units.<br />
    /// <code>
    /// Pango unit = cairo unit * Pango.Scale
    /// </code>
    /// </para>
    /// </remarks>
    public int Width
    {
        get
        {
            this.CheckDisposed();

            int tmp = pango_layout_get_width(this.Handle);

            return tmp == -1
               ? tmp
               : (int)(tmp / (double)Pango.Scale);
        }
        set
        {
            this.CheckDisposed();

            if (value == -1)
            {
                pango_layout_set_width(this.Handle, -1);
            }
            else
            {
                pango_layout_set_width(this.Handle, value * Pango.Scale);
            }
        }
    }

    /// <summary>
    /// Gets or sets the height to which the <see cref="PangoLayout"/> should be ellipsized at.
    /// </summary>
    /// <remarks>
    /// There are two different behaviors, based on whether <c>value</c> is positive or negative.
    /// <para>
    /// If <c>value</c> is positive, it will be the maximum height of the layout. Only lines would
    /// be shown that would fit, and if there is any text omitted, an ellipsis added. At least one
    /// line is included in each paragraph regardless of how small the height value is.<br />
    /// A value of zero will render exactly one line for the entire layout.
    /// </para>
    /// <para>
    /// If <c>value</c> is negative, it will be the (negative of) maximum number of lines per paragraph.
    /// That is, the total number of lines shown may well be more than this value if the layout contains
    /// multiple paragraphs of text. The default value of -1 means that the first line of each paragraph
    /// is ellipsized. This behavior may be changed in the future to act per layout instead of per
    /// paragraph. File a bug against pango at
    /// <a href="https://gitlab.gnome.org/gnome/pango">https://gitlab.gnome.org/gnome/pango</a>
    /// if your code relies on this behavior.
    /// </para>
    /// <para>
    /// Height setting only has effect if a positive width is set on <see cref="PangoLayout"/> and
    /// ellipsization mode of layout is not <see cref="EllipsizeMode.None"/>. The behavior is undefined if
    /// a height other than -1 is set and ellipsization mode is set to <see cref="EllipsizeMode.None"/>,
    /// and may change in the future.
    /// </para>
    /// <para>
    /// The getter returns the height, in cairo units if positive, or number of lines if
    /// negative.
    /// <para>
    /// Note: the native Pango API returns the values in Pango units, whilst this API
    /// returns cairo units.<br />
    /// <code>
    /// Pango unit = cairo unit * Pango.Scale
    /// </code>
    /// </para>
    /// </para>
    /// </remarks>
    public int Height
    {
        get
        {
            this.CheckDisposed();

            int tmp = pango_layout_get_height(this.Handle);

            return tmp == -1
                ? tmp
                : (int)(tmp / (double)Pango.Scale);
        }
        set
        {
            this.CheckDisposed();

            if (value == -1)
            {
                pango_layout_set_height(this.Handle, -1);
            }
            else
            {
                pango_layout_set_height(this.Handle, value * Pango.Scale);
            }
        }
    }

    /// <summary>
    /// Queries whether the layout had to ellipsize any paragraphs.
    /// </summary>
    /// <remarks>
    /// This returns <c>true</c> if the ellipsization mode for layout is not <see cref="EllipsizeMode.None"/>,
    /// a positive width is set on layout, and there are paragraphs exceeding that width that have
    /// to be ellipsized.<br />
    /// Returns <c>false</c> otherwise.
    /// </remarks>
    public bool IsEllipsized
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_is_ellipsized(this.Handle);
        }
    }

    /// <summary>
    /// Gets or sets the type of ellipsization being performed for <see cref="PangoLayout"/>.
    /// </summary>
    /// <remarks>
    /// Depending on the ellipsization mode ellipsize text is removed from the start, middle,
    /// or end of text so they fit within the width and height of layout set with <see cref="Width"/>
    /// and <see cref="Height"/>.
    /// <para>
    /// If the layout contains characters such as newlines that force it to be layed out in multiple
    /// paragraphs, then whether each paragraph is ellipsized separately or the entire layout is ellipsized
    /// as a whole depends on the set height of the layout.
    /// </para>
    /// <para>
    /// The default value is <see cref="EllipsizeMode.None"/>.
    /// <para>
    /// See <see cref="Height"/> for details.
    /// </para>
    /// </para>
    /// </remarks>
    public EllipsizeMode Ellipsize
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_get_ellipsize(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            pango_layout_set_ellipsize(this.Handle, value);
        }
    }

    /// <summary>
    /// Queries whether the layout had to wrap any paragraphs.
    /// </summary>
    /// <remarks>
    /// This returns <c>true</c> if a positive width is set on layout, and there are paragraphs
    /// exceeding the layout width that have to be wrapped.
    /// </remarks>
    public bool IsWrapped
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_is_wrapped(this.Handle);
        }
    }

    /// <summary>
    /// Gets or sets the wrap mode for the <see cref="PangoLayout"/>.
    /// </summary>
    /// <remarks>
    /// The wrap mode only has effect if a width is set on the layout with <see cref="Width"/>.
    /// To turn off wrapping, set the width to -1.
    /// <para>
    /// The default value is <see cref="WrapMode.Word"/>.
    /// </para>
    /// </remarks>
    public WrapMode Wrap
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_get_wrap(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            pango_layout_set_wrap(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the alignment for the layout: how partial lines are positioned within
    /// the horizontal space available.
    /// </summary>
    /// <remarks>
    /// The default alignment is <see cref="Alignment.Left"/>.
    /// </remarks>
    public Alignment Alignment
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_get_alignment(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            pango_layout_set_alignment(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets whether each complete line should be stretched to fill the entire width
    /// of the layout.
    /// </summary>
    /// <remarks>
    /// Stretching is typically done by adding whitespace, but for some scripts (such as Arabic), the
    /// justification may be done in more complex ways, like extending the characters.
    /// <para>
    /// Note that this setting is not implemented and so is ignored in Pango older than 1.18.
    /// </para>
    /// <para>
    /// Note that tabs and justification conflict with each other: Justification will move content
    /// away from its tab-aligned positions.
    /// </para>
    /// <para>
    /// The default value is <c>false</c>.
    /// </para>
    /// </remarks>
    public bool Justify
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_get_justify(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            pango_layout_set_justify(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets whether the last line should be stretched to fill the entire width of
    /// the layout.
    /// </summary>
    /// <remarks>
    /// This only has an effect if <see cref="Justify"/> has been set as well.
    /// <para>
    /// The default value is <c>false</c>.
    /// </para>
    /// </remarks>
    public bool JustifyLastLine
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_get_justify_last_line(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            pango_layout_set_justify_last_line(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the line spacing factor of layout.
    /// </summary>
    /// <remarks>
    /// Typical values are: 0, 1, 1.5, 2. The default values is 0.
    /// <para>
    /// If <c>value</c> is non-zero, lines are placed so that
    /// <code>
    /// baseline2 = baseline1 + value * height2
    /// </code>
    /// where height2 is the line height of the second line (as determined by the font(s)). In this
    /// case, the spacing set with <see cref="Spacing"/> is ignored.
    /// </para>
    /// <para>
    /// If <c>value</c> is zero (the default), spacing is applied as before.
    /// </para>
    /// </remarks>
    public float LineSpacing
    {
        get
        {
            this.CheckDisposed();
            return pango_layout_get_line_spacing(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            pango_layout_set_line_spacing(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the amount of spacing in cairo units between the lines of the
    /// <see cref="PangoLayout"/>.
    /// </summary>
    /// <remarks>
    /// When placing lines with spacing, Pango arranges things so that
    /// <code>
    /// line2.top = line1.bottom + spacing
    /// </code>
    /// <para>
    /// The default value is 0.
    /// </para>
    /// <para>
    /// Note: Since 1.44, Pango is using the line height (as determined by the font) for placing
    /// lines when the line spacing factor is set to a non-zero value with <see cref="LineSpacing"/>.
    /// In that case, the spacing set with this function is ignored.
    /// </para>
    /// <para>
    /// Note: the native Pango API returns the values in Pango units, whilst this API
    /// returns cairo units.<br />
    /// <code>
    /// Pango unit = cairo unit * Pango.Scale
    /// </code>
    /// </para>
    /// </remarks>
    public int Spacing
    {
        get
        {
            this.CheckDisposed();

            int tmp = pango_layout_get_spacing(this.Handle);
            return (int)(tmp / (double)Pango.Scale);
        }
        set
        {
            this.CheckDisposed();
            pango_layout_set_spacing(this.Handle, value * Pango.Scale);
        }
    }

    /// <summary>
    /// Gets or sets the width in cairo units to indent each paragraph.
    /// </summary>
    /// <remarks>
    /// A negative value of indent will produce a hanging indentation. That is, the first line
    /// will have the full width, and subsequent lines will be indented by the absolute value of indent.
    /// <para>
    /// The indent setting is ignored if layout alignment is set to <see cref="Alignment.Center"/>.
    /// </para>
    /// <para>
    /// The default value is 0.
    /// </para>
    /// <para>
    /// Note: the native Pango API returns the values in Pango units, whilst this API
    /// returns cairo units.<br />
    /// <code>
    /// Pango unit = cairo unit * Pango.Scale
    /// </code>
    /// </para>
    /// </remarks>
    public int Ident
    {
        get
        {
            this.CheckDisposed();

            int tmp = pango_layout_get_indent(this.Handle);
            return (int)(tmp / (double)Pango.Scale);
        }
        set
        {
            this.CheckDisposed();
            pango_layout_set_indent(this.Handle, value * Pango.Scale);
        }
    }
}
