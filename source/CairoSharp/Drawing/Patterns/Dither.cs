// (c) gfoidl, all rights reserved

namespace Cairo.Drawing.Patterns;

/// <summary>
/// Dither is an intentionally applied form of noise used to randomize quantization
/// error, preventing large-scale patterns such as color banding in images (e.g. for
/// gradients). Ordered dithering applies a precomputed threshold matrix to spread the errors smoothly.
/// </summary>
/// <remarks>
/// <see cref="Dither"/> is modeled on pixman dithering algorithm choice. As of Pixman 0.40,
/// <see cref="Fast"/> corresponds to a 8x8 ordered bayer noise and <see cref="Good"/> and
/// <see cref="Best"/> use an ordered 64x64 precomputed blue noise.
/// </remarks>
public enum Dither
{
    /// <summary>
    /// No dithering.
    /// </summary>
    None,

    /// <summary>
    /// Default choice at cairo compile time. Currently <see cref="None"/>
    /// </summary>
    Default,

    /// <summary>
    /// Fastest dithering algorithm supported by the backend
    /// </summary>
    Fast,

    /// <summary>
    /// An algorithm with smoother dithering than FAST
    /// </summary>
    Good,

    /// <summary>
    /// Best algorithm available in the backend
    /// </summary>
    Best
}
