// (c) gfoidl, all rights reserved

using System.Diagnostics;
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
    /// <remarks>
    /// This layout can then be used for text measurement with functions like <see cref="GetSize(out int, out int)"/>
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
    public PangoLayout(CairoContext cr) : base(Create(cr)) => _cr = cr;

    private static pango_layout* Create(CairoContext cr)
    {
        ArgumentNullException.ThrowIfNull(cr);
        cr.CheckDisposed();

        return pango_cairo_create_layout(cr.Handle);
    }

    protected override void DisposeCore(pango_layout* handle)
    {
        Debug.Assert(LibGObjectName == LoadingNative.LibGObjectName);

        LoadingNative.g_object_unref(handle);
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
    /// Determines the logical width and height of a <see cref="PangoLayout"/> in Pango units.
    /// </summary>
    /// <param name="width">the logical width</param>
    /// <param name="height">the logical height</param>
    public void GetSize(out int width, out int height)
    {
        this.CheckDisposed();
        pango_layout_get_size(this.Handle, out width, out height);
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
    /// Updates the private PangoContext of a <see cref="PangoLayout"/> created with <see cref="PangoLayout(CairoContext)"/>
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
}
