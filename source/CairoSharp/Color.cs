// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cairo;

/// <summary>
/// The color components are floating point numbers in the range [0, 1].
/// </summary>
/// <param name="Red">red component of color</param>
/// <param name="Green">green component of color</param>
/// <param name="Blue">blue component of color</param>
/// <param name="Alpha">alpha component of color</param>
/// <remarks>
/// If the values passed in are outside that range, they will be clamped.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
[DebuggerNonUserCode]
public readonly record struct Color(double Red, double Green, double Blue, double Alpha)
{
    /// <summary>
    /// Creates a new <see cref="Color"/> with the given component in the range [0, 1].
    /// </summary>
    /// <param name="red">red component of color</param>
    /// <param name="green">green component of color</param>
    /// <param name="blue">blue component of color</param>
    public Color(double red, double green, double blue) : this(red, green, blue, 1d) { }

    /// <summary>
    /// Default color that is opaque black.
    /// </summary>
    public static Color Default { get; } = new Color(0, 0, 0, 1d);
}
