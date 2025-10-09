// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using Cairo.Drawing;

namespace Cairo;

/// <summary>
/// A data structure for holding a rectangle.
/// </summary>
/// <param name="X">the X coordinate of the top left corner of the rectangle</param>
/// <param name="Y">the Y coordinate to the top left corner of the rectangle</param>
/// <param name="Width">the width of the rectangle</param>
/// <param name="Height">the height of the rectangle</param>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Rectangle(double X, double Y, double Width, double Height)
{
    /// <summary>
    /// Create a new <see cref="Rectangle"/>
    /// </summary>
    /// <param name="point">top left corner of the rectangle</param>
    /// <param name="width">the width of the rectangle</param>
    /// <param name="height">the height of the rectangle</param>
    public Rectangle(Point point, double width, double height) : this(point.X, point.Y, width, height) { }
}
