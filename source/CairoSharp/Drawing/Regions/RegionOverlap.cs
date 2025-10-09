// (c) gfoidl, all rights reserved

namespace Cairo.Drawing.Regions;

/// <summary>
/// Used as the return value for cairo_region_contains_rectangle().
/// </summary>
public enum RegionOverlap
{
    /// <summary>
    /// The contents are entirely inside the region. (Since 1.10)
    /// </summary>
    In,

    /// <summary>
    /// The contents are entirely outside the region. (Since 1.10)
    /// </summary>
    Out,

    /// <summary>
    /// The contents are partially inside and partially outside the region. (Since 1.10)
    /// </summary>
    Part
}
