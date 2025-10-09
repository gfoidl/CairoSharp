// (c) gfoidl, all rights reserved

namespace Cairo.Drawing;

/// <summary>
/// Specifies how to render the endpoints of the path when stroking.
/// </summary>
/// <remarks>
/// The default line cap style is <see cref="LineCap.Butt"/>.
/// </remarks>
public enum LineCap
{
    /// <summary>
    /// start (stop) the line exactly at the start (end) point (Since 1.0)
    /// </summary>
    Butt,

    /// <summary>
    /// use a round ending, the center of the circle is the end point (Since 1.0)
    /// </summary>
    Round,

    /// <summary>
    /// use squared ending, the center of the square is the end point (Since 1.0)
    /// </summary>
    Square
}
