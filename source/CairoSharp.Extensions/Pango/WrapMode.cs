// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Pango;

/// <summary>
/// <see cref="WrapMode"/> describes how to wrap the lines of a <see cref="PangoLayout"/>
/// to the desired width.
/// </summary>
/// <remarks>
/// For <see cref="Word"/>, Pango uses break opportunities that are determined by the Unicode
/// line breaking algorithm.<br />
/// For <see cref="Char"/>, Pango allows breaking at grapheme boundaries that are determined
/// by the Unicode text segmentation algorithm.
/// </remarks>
public enum WrapMode
{
    /// <summary>
    /// Wrap lines at word boundaries.
    /// </summary>
    Word = 0,

    /// <summary>
    /// Wrap lines at character boundaries.
    /// </summary>
    Char = 1,

    /// <summary>
    /// Wrap lines at word boundaries, but fall back to character boundaries if
    /// there is not enough space for a full word.
    /// </summary>
    WordChar = 2,

    /// <summary>
    /// Do not wrap.
    /// </summary>
    None = 3
}
