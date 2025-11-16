// (c) gfoidl, all rights reserved

namespace Cairo.Drawing;

/// <summary>
/// Specifies how to render the junction of two lines when stroking.
/// </summary>
/// <remarks>
/// The default line join style is <see cref="Miter"/>.
/// </remarks>
public enum LineJoin
{
    /// <summary>
    /// use a sharp (angled) corner, see cairo_set_miter_limit() (Since 1.0)
    /// </summary>
    Miter,

    /// <summary>
    /// use a rounded join, the center of the circle is the joint point (Since 1.0)
    /// </summary>
    Round,

    /// <summary>
    /// use a cut-off join, the join is cut off at half the line width from the joint point (Since 1.0)
    /// </summary>
    Bevel
}
