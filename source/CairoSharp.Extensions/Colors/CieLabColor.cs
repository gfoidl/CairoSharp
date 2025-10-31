// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;

namespace Cairo.Extensions.Colors;

/// <summary>
/// A color given in the CIE-L*a*b* color space.
/// </summary>
/// <param name="L">
/// L* (Lightness) represents the lightness of the color, ranging from 0 (black) to 100 (white).
/// </param>
/// <param name="A">
/// a* (Green-Red Axis) represents the color position between green and red. Negative values indicate green,
/// and positive values indicate red. Range is typically [-110,110] or [-128,127].
/// </param>
/// <param name="B">
/// b* (Blue-Yellow Axis) represents the color position between blue and yellow. Negative values indicate
/// blue, and positive values indicate yellow. Range is typically [-110,110] or [-128,127].
/// </param>
/// <remarks>
/// CIE-L*a*b* is a nonlinear transformation of RGB where the euclidean distance between two
/// colors is equal to their perceptual distances (for distances less than ~10 units).
/// <para>
/// Algorithms that process color images often produce better results in CIE-L*a*b*.
/// </para>
/// <para>
/// For further information see e.g. <a href="https://kaizoudou.com/from-rgb-to-lab-color-space/">From RGB to L*a*b* color space</a>.
/// </para>
/// </remarks>
public readonly record struct CieLabColor(double L, double A, double B)
{
    /// <summary>
    /// Validates that the color components are in bounds.
    /// </summary>
    public void Validate()
    {
        if (this.L <    0 || this.L > 100) ThrowL();
        if (this.A < -110 || this.A > 110) ThrowA();
        if (this.B < -110 || this.B > 110) ThrowB();

        static void ThrowL() => throw new ArgumentException("L is outside of [0,100].");
        static void ThrowA() => throw new ArgumentException("a is outside of [-110,110].");
        static void ThrowB() => throw new ArgumentException("b is outside of [-110,110].");
    }

    /// <summary>
    /// Reference white D65.
    /// </summary>
    public static CieLabColor Xn { get; } = new CieLabColor(0.950456, 1, 1.088754);

    /// <summary>
    /// Returns the sRGB (<see cref="Color"/>) for this color.
    /// </summary>
    /// <returns>The sRGB color</returns>
    /// <remarks>
    /// The transformation is based on ITU-R recommendation BT.709 and
    /// uses D65 as reference white point.
    /// <para>
    /// The algorithmic error for the transformation is in the magnitude
    /// of 1e-5. Note that due to rounding to integers the error in the output may become 1.
    /// </para>
    /// <para>
    /// When a L*a*b* color cannot be represented in the RGB color space, then the component values will be
    /// clamped in the interval [0,1], and no exception is thrown.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">a color component is outside the valid bounds</exception>
    /// <seealso cref="ColorExtensions.ToCieLab(Color)"/>
    public Color ToRGB()
    {
        double y = (this.L + 16d) / 116d;
        double x = this.A / 500d + y;
        double z = y - this.B / 200d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double Inv_f_of_t(double value)
        {
            double value3 = value * value * value;

            if (value3 > 0.008856)
            {
                return value3;
            }

            return (value - 16d / 116d) / 7.787;
        }

        x = Inv_f_of_t(x);
        y = Inv_f_of_t(y);
        z = Inv_f_of_t(z);

        // Normalize with reference white
        x *= Xn.L;
        y *= Xn.A;
        z *= Xn.B;

        // XYZ -> RGB, thereby use the same reference white D65.
        double r =  3.24045480 * x + -1.53713890 * y + -0.49853155 * z;
        double g = -0.96926639 * x +  1.87601093 * y +  0.04155608 * z;
        double b =  0.05564342 * x + -0.20402585 * y +  1.05722516 * z;

        r = Math.Clamp(r, 0, 1);
        g = Math.Clamp(g, 0, 1);
        b = Math.Clamp(b, 0, 1);

        Color color = new(r, g, b);
        return color.GammaCorrection(gammaExpansion: false);
    }

    /// <summary>
    /// Calculates the quadratic perceptual distance of the colors in the CIE-L*a*b*
    /// color space according the euclidean norm.
    /// </summary>
    /// <param name="other">the other <see cref="CieLabColor"/></param>
    /// <returns>the quadratic perceptual distance of the colors</returns>
    public double ColorDistance2(CieLabColor other)
    {
        double dL  = this.L - other.L;
        double dL2 = dL * dL;

        double da  = this.A - other.A;
        double da2 = da * da;

        double db  = this.B - other.B;
        double db2 = db * db;

        return dL2 + da2 + db2;
    }

    /// <summary>
    /// Calculates the perceptual distance of the colors in the CIE-L*a*b* color space
    /// according the euclidean norm.
    /// </summary>
    /// <param name="other">the other <see cref="CieLabColor"/></param>
    /// <returns>the perceptual distance of the colors</returns>
    public double ColorDistance(CieLabColor other) => Math.Sqrt(this.ColorDistance2(other));
}
