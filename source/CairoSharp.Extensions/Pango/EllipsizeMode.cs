// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Pango;

/// <summary>
/// <see cref="EllipsizeMode"/> describes what sort of ellipsization should be
/// applied to text.
/// </summary>
/// <remarks>
/// In the ellipsization process characters are removed from the text in order to make
/// it fit to a given width and replaced with an ellipsis.
/// </remarks>
public enum EllipsizeMode
{
    /// <summary>
    /// No ellipsization.
    /// </summary>
    None = 0,

    /// <summary>
    /// Omit characters at the start of the text.
    /// </summary>
    Start = 1,

    /// <summary>
    /// Omit characters in the middle of the text.
    /// </summary>
    Middle = 2,

    /// <summary>
    /// Omit characters at the end of the text.
    /// </summary>
    End = 3
}
