// (c) gfoidl, all rights reserved

namespace Cairo.Fonts;

/// <summary>
/// Specifies if color fonts are to be rendered using the color glyphs or outline
/// glyphs. Glyphs that do not have a color presentation, and non-color fonts
/// are not affected by this font option.
/// </summary>
public enum ColorMode
{
    /// <summary>
    /// Use the default color mode for font backend and target device, since 1.18.
    /// </summary>
    Default,

    /// <summary>
    /// Disable rendering color glyphs. Glyphs are always rendered as outline glyphs, since 1.18.
    /// </summary>
    NoColor,

    /// <summary>
    /// Enable rendering color glyphs. If the font contains a color presentation for a glyph,
    /// and when supported by the font backend, the glyph will be rendered in color, since 1.18.
    /// </summary>
    Color
}
