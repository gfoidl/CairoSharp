// (c) gfoidl, all rights reserved

namespace Cairo.Fonts.FreeType;

/// <summary>
/// A set of synthesis options to control how FreeType renders the glyphs for a particular font face.
/// </summary>
/// <remarks>
/// Individual synthesis features of a cairo_ft_font_face_t can be set using cairo_ft_font_face_set_synthesize(),
/// or disabled using cairo_ft_font_face_unset_synthesize(). The currently enabled set of synthesis
/// options can be queried with cairo_ft_font_face_get_synthesize().
/// <para>
/// Note: that when synthesizing glyphs, the font metrics returned will only be estimates.
/// </para>
/// </remarks>
[Flags]
public enum Synthesize
{
    /// <summary>
    /// Embolden the glyphs (redraw with a pixel offset)
    /// </summary>
    Bold = 1 << 0,

    /// <summary>
    /// Slant the glyph outline by 12 degrees to the right.
    /// </summary>
    Oblique = 1 << 1
}
