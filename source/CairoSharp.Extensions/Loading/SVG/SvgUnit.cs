// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Loading.SVG;

/// <summary>
/// Units for SVG. These have the same meaning as <a href="https://www.w3.org/TR/CSS2/syndata.html#length-units">CSS length units</a>.
/// </summary>
/// <remarks>
/// If you test for the values of this enum, please note that librsvg may add other units in the future
/// as its support for CSS improves. Please make your code handle unknown units gracefully (e.g. with a
/// <see langword="default"/> case in a <see langword="switch"/> statement).
/// </remarks>
public enum SvgUnit
{
    /// <summary>
    /// Percentage values; where 1.0 means 100%
    /// </summary>
    Percent = 0,

    /// <summary>
    /// Pixels
    /// </summary>
    Pixel = 1,

    /// <summary>
    /// Em, or the current font size
    /// </summary>
    Em = 2,

    /// <summary>
    /// X-height of the current font
    /// </summary>
    Ex = 3,

    /// <summary>
    /// Inches
    /// </summary>
    Inch = 4,

    /// <summary>
    /// Centimeters
    /// </summary>
    Centimeter = 5,

    /// <summary>
    /// Millimeters
    /// </summary>
    Millimeters = 6,

    /// <summary>
    /// Points, or 1/72 inch
    /// </summary>
    Pt = 7,

    /// <summary>
    /// Picas, or 1/6 inch (12 points)
    /// </summary>
    Pc = 8,

    /// <summary>
    /// Advance measure of a '0' character (depends on the text orientation)
    /// </summary>
    Ch = 9
}
