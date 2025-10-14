// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Loading.SVG;

/// <summary>
/// This is equivalent to <a href="https://www.w3.org/TR/CSS2/syndata.html#length-units">CSS lengths</a>.
/// </summary>
/// <param name="Length">Numeric part of the length</param>
/// <param name="Unit">Unit part of the length</param>
public readonly record struct SvgLength(double Length, SvgUnit Unit);
