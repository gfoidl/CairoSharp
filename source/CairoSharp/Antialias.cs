// (c) gfoidl, all rights reserved

namespace Cairo;

/// <summary>
/// Specifies the type of antialiasing to do when rendering text or shapes.
/// </summary>
/// <remarks>
/// As it is not necessarily clear from the above what advantages a particular antialias method provides,
/// since 1.12, there is also a set of hints: <see cref="Fast"/>: Allow the backend to degrade
/// raster quality for speed <see cref="Good"/>: A balance between speed and quality
/// <see cref="Best"/>: A high-fidelity, but potentially slow, raster mode.
/// <para>
/// These make no guarantee on how the backend will perform its rasterisation (if it even rasterises!), nor
/// that they have any differing effect other than to enable some form of antialiasing. In the case of glyph
/// rendering, <see cref="Fast"/> and <see cref="Good"/> will be mapped to <see cref="Gray"/>,
/// with <see cref="Best"/> being equivalent to <see cref="Subpixel"/>.
/// </para>
/// <para>
/// The interpretation of <see cref="Default"/> is left entirely up to the backend, typically this
/// will be similar to <see cref="Good"/>.
/// </para>
/// </remarks>
public enum Antialias
{
    /// <summary>
    /// Use the default antialiasing for the subsystem and target device, since 1.0
    /// </summary>
    Default,

    /// <summary>
    /// Use a bilevel alpha mask, since 1.0
    /// </summary>
    None,

    /// <summary>
    /// Perform single-color antialiasing (using shades of gray for black text on a white background, for example), since 1.0
    /// </summary>
    Gray,

    /// <summary>
    /// Perform antialiasing by taking advantage of the order of subpixel elements on devices such as LCD panels, since 1.0
    /// </summary>
    Subpixel,

    /// <summary>
    /// Hint that the backend should perform some antialiasing but prefer speed over quality, since 1.12
    /// </summary>
    Fast,

    /// <summary>
    /// The backend should balance quality against performance, since 1.12
    /// </summary>
    Good,

    /// <summary>
    /// Hint that the backend should render at the highest quality, sacrificing speed if necessary, since 1.12
    /// </summary>
    Best
}
