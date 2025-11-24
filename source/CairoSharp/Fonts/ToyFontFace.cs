// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Drawing.Text;
using static Cairo.Drawing.Text.TextNative;

namespace Cairo.Fonts;

/// <summary>
/// The <see cref="ToyFontFace"/> class can be used instead of <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/>
/// to create a toy font independently of a context.
/// </summary>
public sealed unsafe class ToyFontFace : FontFace
{
    internal ToyFontFace(cairo_font_face_t* fontFace, bool isOwnedByCairo = false, bool needsDestroy = true)
        : base(fontFace, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Creates a font face from a triplet of family, slant, and weight. These font faces are used in implementation
    /// of the <see cref="CairoContext"/> "toy" font API.
    /// </summary>
    /// <param name="family">a font family name, encoded in UTF-8</param>
    /// <param name="slant">the slant for the font</param>
    /// <param name="weight">the weight for the font</param>
    /// <remarks>
    /// If family is the zero-length string "", the platform-specific default family is assumed. The default
    /// family then can be queried using cairo_toy_font_face_get_family().
    /// <para>
    /// The cairo_select_font_face() function uses this to create font faces. See that function for limitations and
    /// other details of toy font faces.
    /// </para>
    /// </remarks>
    public ToyFontFace(string family, FontSlant slant = FontSlant.Normal, FontWeight weight = FontWeight.Normal)
        : base(cairo_toy_font_face_create(family, slant, weight)) { }

    /// <summary>
    /// Gets the family name of a toy font.
    /// </summary>
    public string Family
    {
        get
        {
            this.CheckDisposed();

            field ??= cairo_toy_font_face_get_family(this.Handle);

            Debug.Assert(field is not null);
            return field;
        }
    }

    /// <summary>
    /// Gets the slant a toy font.
    /// </summary>
    public FontSlant Slant
    {
        get
        {
            this.CheckDisposed();
            return cairo_toy_font_face_get_slant(this.Handle);
        }
    }

    /// <summary>
    /// Gets the weight a toy font.
    /// </summary>
    public FontWeight Weight
    {
        get
        {
            this.CheckDisposed();
            return cairo_toy_font_face_get_weight(this.Handle);
        }
    }
}
