// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Colors;

/// <summary>
/// The mode for gray scale.
/// </summary>
public enum GrayScaleMode
{
    /// <summary>
    /// Averages the most prominent and least prominent color components.
    /// </summary>
    Lightness = 0,

    /// <summary>
    /// Average of R, G, and B color components.
    /// </summary>
    Average = 1,

    /// <summary>
    /// Average base on the human perception.
    /// </summary>
    /// <remarks>
    /// The human eye is more sensitive to green than to other colors, so green will be
    /// weighted most heavily.
    /// </remarks>
    Luminosity = 2,

    /// <summary>
    /// Uses the <see cref="CieLabColor.L"/>ightness value as gray value.
    /// </summary>
    CieLab = 3,

    /// <summary>
    /// Performs gamma expansion (sRGB -&gt; linearized RGB) and the averages the components.
    /// </summary>
    GammaExpandedAverage = 4
}
