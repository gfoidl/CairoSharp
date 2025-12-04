// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Pango;

/// <summary>
/// <see cref="Alignment"/> describes how to align the lines of a <see cref="PangoLayout"/>
/// within the available space.
/// </summary>
/// <remarks>
/// If the <see cref="PangoLayout"/> is set to justify using <see cref="PangoLayout.Justify"/>,
/// this only affects partial lines.
/// </remarks>
public enum Alignment
{
    /// <summary>
    /// Put all available space on the right.
    /// </summary>
    Left = 0,

    /// <summary>
    /// Center the line within the available space.
    /// </summary>
    Center = 1,

    /// <summary>
    /// Put all available space on the left.
    /// </summary>
    Right = 2
}
