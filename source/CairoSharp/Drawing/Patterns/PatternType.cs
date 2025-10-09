// (c) gfoidl, all rights reserved

namespace Cairo.Drawing.Patterns;

/// <summary>
/// <see cref="PatternType"/> is used to describe the type of a given pattern.
/// <para>
/// The type of a pattern is determined by the function used to create it. The
/// cairo_pattern_create_rgb() and cairo_pattern_create_rgba() functions create
/// <see cref="Solid"/> patterns. The remaining cairo_pattern_create functions map to
/// pattern types in obvious ways.
/// </para>
/// </summary>
/// <remarks>
/// The pattern type can be queried with cairo_pattern_get_type()
/// <para>
/// Most cairo_pattern_t functions can be called with a pattern of any type,
/// (though trying to change the extend or filter for a solid pattern will have no effect).
/// A notable exception is cairo_pattern_add_color_stop_rgb() and cairo_pattern_add_color_stop_rgba()
/// which must only be called with gradient patterns (either <see cref="Linear"/> or <see cref="Radial"/>).
/// Otherwise the pattern will be shutdown and put into an error state.
/// </para>
/// <para>
/// New entries may be added in future versions.
/// </para>
/// </remarks>
public enum PatternType
{
    /// <summary>
    /// The pattern is a solid (uniform) color. It may be opaque or translucent, since 1.2.
    /// </summary>
    Solid,

    /// <summary>
    /// The pattern is a based on a surface (an image), since 1.2.
    /// </summary>
    Surface,

    /// <summary>
    /// The pattern is a linear gradient, since 1.2.
    /// </summary>
    Linear,

    /// <summary>
    /// The pattern is a radial gradient, since 1.2.
    /// </summary>
    Radial,

    /// <summary>
    /// The pattern is a mesh, since 1.12.
    /// </summary>
    Mesh,

    /// <summary>
    /// The pattern is a user pattern providing raster data, since 1.12.
    /// </summary>
    RasterSource
}

