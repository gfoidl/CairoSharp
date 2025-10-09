// (c) gfoidl, all rights reserved

namespace Cairo.Drawing.Patterns;

/// <summary>
/// <see cref="Filter"/> is used to indicate what filtering should be applied when
/// reading pixel values from patterns. See cairo_pattern_set_filter() for indicating
/// the desired filter to be used with a particular pattern.
/// </summary>
public enum Filter
{
    /// <summary>
    /// A high-performance filter, with quality similar to <see cref="Nearest"/> (Since 1.0)
    /// </summary>
    Fast,

    /// <summary>
    /// A reasonable-performance filter, with quality similar to <see cref="Bilinear"/> (Since 1.0)
    /// </summary>
    Good,

    /// <summary>
    /// The highest-quality available, performance may not be suitable for interactive use. (Since 1.0)
    /// </summary>
    Best,

    /// <summary>
    /// Nearest-neighbor filtering (Since 1.0)
    /// </summary>
    Nearest,

    /// <summary>
    /// Linear interpolation in two dimensions (Since 1.0)
    /// </summary>
    Bilinear,

    /// <summary>
    /// This filter value is currently unimplemented, and should not be used in current code. (Since 1.0)
    /// </summary>
    Gaussian
}
