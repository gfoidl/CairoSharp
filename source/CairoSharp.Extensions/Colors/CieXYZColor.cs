// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Cairo.Extensions.Colors;

/// <summary>
/// The CIE XYZ color space.
/// </summary>
/// <param name="X">X component of the color in [0,1]</param>
/// <param name="Y">Y component of the color in [0,1]</param>
/// <param name="Z">Z component of the color in [0,1]</param>
[StructLayout(LayoutKind.Sequential, Size = 4 * sizeof(double))]    // speed over size here
public readonly record struct CieXYZColor(double X, double Y, double Z)
{
    internal Vector256<double> AsVector256                           => Unsafe.BitCast<CieXYZColor, Vector256<double>>(this);
    internal static CieXYZColor FromVector256(Vector256<double> vec) => Unsafe.BitCast<Vector256<double>, CieXYZColor>(vec);

    /// <summary>
    /// Returns the sRGB (<see cref="Color"/>) for this color.
    /// </summary>
    /// <returns>The sRGB color</returns>
    /// <seealso cref="ColorExtensions.ToCieXYZ(Color)"/>
    public Color ToRGB() => CieHelper.ToRGB(this);
}
