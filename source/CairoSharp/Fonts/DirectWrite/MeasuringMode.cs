// (c) gfoidl, all rights reserved

namespace Cairo.Fonts.DirectWrite;

/// <summary>
/// Indicates the measuring method used for text layout.
/// </summary>
public enum MeasuringMode
{
    /// <summary>
    /// Specifies that text is measured using glyph ideal metrics whose values are
    /// independent to the current display resolution.
    /// </summary>
    Natural,

    /// <summary>
    /// Specifies that text is measured using glyph display-compatible metrics
    /// whose values tuned for the current display resolution.
    /// </summary>
    GdiClassic,

    /// <summary>
    /// Specifies that text is measured using the same glyph display metrics as text
    /// measured by GDI using a font created with CLEARTYPE_NATURAL_QUALITY.
    /// </summary>
    GdiNatural
}
