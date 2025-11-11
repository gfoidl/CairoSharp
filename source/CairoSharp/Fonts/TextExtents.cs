// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Fonts;

/// <summary>
/// The <see cref="TextExtents"/> structure stores the extents of a single glyph or a string of
/// glyphs in user-space coordinates. Because text extents are in user-space coordinates,
/// they are mostly, but not entirely, independent of the current transformation matrix.
/// If you call cairo_scale(cr, 2.0, 2.0), text will be drawn twice as big, but the reported
/// text extents will not be doubled. They will change slightly due to hinting (so you can't
/// assume that metrics are independent of the transformation matrix), but otherwise will
/// remain unchanged.
/// </summary>
/// <param name="XBearing">
/// the horizontal distance from the origin to the leftmost part of the glyphs as drawn.
/// Positive if the glyphs lie entirely to the right of the origin.
/// </param>
/// <param name="YBearing">
/// the vertical distance from the origin to the topmost part of the glyphs as drawn.
/// Positive only if the glyphs lie completely below the origin; will usually be negative.
/// </param>
/// <param name="Width">width of the glyphs as drawn</param>
/// <param name="Height">height of the glyphs as drawn</param>
/// <param name="XAdvance">distance to advance in the X direction after drawing these glyphs</param>
/// <param name="YAdvance">
/// distance to advance in the Y direction after drawing these glyphs. Will typically be zero
/// except for vertical text layout as found in East-Asian languages.
/// </param>
/// <remarks>
/// A visualization of the extent's parameters is given in <a href="https://freetype.org/freetype2/docs/glyphs/glyph-metrics-3.svg">this image</a>
/// from <a href="https://freetype.org/freetype2/docs/glyphs/glyphs-3.html">Glyph Metrics</a>.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct TextExtents(
    double XBearing,
    double YBearing,
    double Width,
    double Height,
    double XAdvance,
    double YAdvance);
