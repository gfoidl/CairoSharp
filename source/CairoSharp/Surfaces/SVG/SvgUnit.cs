// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces.SVG;

/// <summary>
/// <see cref="SvgUnit"/> is used to describe the units valid for coordinates
/// and lengths in the SVG specification.
/// </summary>
/// <remarks>
/// See also:
/// <list type="bullet">
/// <item>https://www.w3.org/TR/SVG/coords.html#Units</item>
/// <item>https://www.w3.org/TR/SVG/types.html#DataTypeLength</item>
/// <item>https://www.w3.org/TR/css-values-3/#lengths</item>
/// </list>
/// </remarks>
public enum SvgUnit
{
    /// <summary>
    /// User unit, a value in the current coordinate system. If used in the root
    /// element for the initial coordinate systems it corresponds to pixels. (Since 1.16)
    /// </summary>
    User,

    /// <summary>
    /// The size of the element's font. (Since 1.16)
    /// </summary>
    Em,

    /// <summary>
    /// The x-height of the element’s font. (Since 1.16)
    /// </summary>
    Ex,

    /// <summary>
    /// Pixels (1px = 1/96th of 1in). (Since 1.16)
    /// </summary>
    Pixels,

    /// <summary>
    /// Inches (1in = 2.54cm = 96px). (Since 1.16)
    /// </summary>
    Inch,

    /// <summary>
    /// Centimeters (1cm = 96px/2.54). (Since 1.16)
    /// </summary>
    Centimeter,

    /// <summary>
    /// Millimeters (1mm = 1/10th of 1cm). (Since 1.16)
    /// </summary>
    MilliMeter,

    /// <summary>
    /// Points (1pt = 1/72th of 1in). (Since 1.16)
    /// </summary>
    Point,

    /// <summary>
    /// Picas (1pc = 1/6th of 1in). (Since 1.16)
    /// </summary>
    Picas,

    /// <summary>
    /// Percent, a value that is some fraction of another reference value. (Since 1.16)
    /// </summary>
    Percent
}
