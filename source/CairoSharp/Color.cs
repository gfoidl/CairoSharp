// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Cairo;

/// <summary>
/// The color components are floating point numbers in the range [0, 1].
/// </summary>
/// <param name="Red">red component of color in [0,1]</param>
/// <param name="Green">green component of color in [0,1]</param>
/// <param name="Blue">blue component of color in [0,1]</param>
/// <param name="Alpha">alpha component of color in [0,1]</param>
/// <remarks>
/// The color and alpha components are floating point numbers in the range 0 to 1.
/// If the values passed in are outside that range, they will be clamped when used
/// in cairo.
/// <para>
/// Note that the color and alpha values are not premultiplied.
/// </para>
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
[DebuggerNonUserCode]
public readonly record struct Color(double Red, double Green, double Blue, double Alpha)
{
    internal Vector256<double> AsVector256                     => Unsafe.BitCast<Color, Vector256<double>>(this);
    internal static Color FromVector256(Vector256<double> vec) => Unsafe.BitCast<Vector256<double>, Color>(vec);

    /// <summary>
    /// Creates a new <see cref="Color"/> with the given component in the range [0, 1].
    /// </summary>
    /// <param name="red">red component of color</param>
    /// <param name="green">green component of color</param>
    /// <param name="blue">blue component of color</param>
    /// <remarks>
    /// The color components are floating point numbers in the range 0 to 1. If the values
    /// passed in are outside that range, they will be clamped when used in cairo.
    /// </remarks>
    public Color(double red, double green, double blue) : this(red, green, blue, 1d) { }

    /// <summary>
    /// Default color that is opaque black.
    /// </summary>
    public static Color Default { get; } = new Color(0, 0, 0, 1d);

#pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Color other)
#pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    {
        if (Vector256.IsHardwareAccelerated)
        {
            return this.AsVector256 == other.AsVector256;
        }

        return this.Red   == other.Red
            && this.Green == other.Green
            && this.Blue  == other.Blue
            && this.Alpha == other.Alpha;
    }
}
