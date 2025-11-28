// (c) gfoidl, all rights reserved

namespace Cairo.Fonts.FreeType;

/// <summary>
/// A set of synthesis options to control how FreeType renders the glyphs for a particular font face.
/// </summary>
/// <remarks>
/// Individual synthesis features of a <see cref="FreeTypeFont"/> can be set using
/// <see cref="FreeTypeFont.SetSynthesize(Synthesize)"/>, or disabled using
/// <see cref="FreeTypeFont.UnsetSynthesize(Synthesize)"/>. The currently enabled set of synthesis
/// options can be queried with <see cref="FreeTypeFont.GetSynthesize"/>.
/// <para>
/// Note: that when synthesizing glyphs, the font metrics returned will only be estimates.
/// </para>
/// </remarks>
[Flags]
public enum Synthesize : uint
{
    /// <summary>
    /// Default, i.e. none of <see cref="Bold"/> or <see cref="Oblique"/>
    /// </summary>
    /// <remarks>
    /// When used in <see cref="FreeTypeFont.SetSynthesize(Synthesize)"/>, this will actually
    /// call <see cref="FreeTypeFont.UnsetSynthesize(Synthesize)"/> internally.
    /// </remarks>
    None = 0,

    /// <summary>
    /// Embolden the glyphs (redraw with a pixel offset)
    /// </summary>
    Bold = 1 << 0,

    /// <summary>
    /// Slant the glyph outline by 12 degrees to the right.
    /// </summary>
    Oblique = 1 << 1
}
