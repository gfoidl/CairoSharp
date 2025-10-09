// (c) gfoidl, all rights reserved

namespace Cairo.Drawing.Patterns;

/// <summary>
/// <see cref="Extend"/> is used to describe how pattern color/alpha will be determined for
/// areas "outside" the pattern's natural area, (for example, outside the surface bounds or
/// outside the gradient geometry).
/// </summary>
/// <remarks>
/// Mesh patterns are not affected by the extend mode.
/// <para>
/// The default extend mode is <see cref="None"/> for surface patterns and
/// <see cref="Pad"/> for gradient patterns.
/// </para>
/// <para>
/// New entries may be added in future versions.
/// </para>
/// </remarks>
public enum Extend
{
    /// <summary>
    /// pixels outside of the source pattern are fully transparent (Since 1.0)
    /// </summary>
    None,

    /// <summary>
    /// the pattern is tiled by repeating (Since 1.0)
    /// </summary>
    Repeat,

    /// <summary>
    /// the pattern is tiled by reflecting at the edges (Since 1.0; but only implemented
    /// for surface patterns since 1.6)
    /// </summary>
    Reflect,

    /// <summary>
    /// pixels outside of the pattern copy the closest pixel from the source (Since 1.2; but
    /// only implemented for surface patterns since 1.6)
    /// </summary>
    Pad
}
