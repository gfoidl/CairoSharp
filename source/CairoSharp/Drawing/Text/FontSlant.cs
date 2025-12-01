// (c) gfoidl, all rights reserved

namespace Cairo.Drawing.Text;

/// <summary>
/// Specifies variants of a font face based on their slant.
/// </summary>
public enum FontSlant
{
    /// <summary>
    /// Upright font style, since 1.0
    /// </summary>
    Normal,

    /// <summary>
    /// Italic font style, since 1.0
    /// </summary>
    /// <remarks>
    /// <see cref="Italic"/> is typographically set, whilst <see cref="Oblique"/> is
    /// just sheared and skewed of the original font.
    /// </remarks>
    Italic,

    /// <summary>
    /// Oblique font style, since 1.0
    /// </summary>
    /// <remarks>
    /// <see cref="Italic"/> is typographically set, whilst <see cref="Oblique"/> is
    /// just sheared and skewed of the original font.
    /// </remarks>
    Oblique
}
