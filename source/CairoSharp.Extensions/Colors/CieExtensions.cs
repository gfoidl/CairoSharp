// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Cairo.Extensions.Colors;

/// <summary>
/// Extensions for <see cref="Color"/>.
/// </summary>
public static class CieExtensions
{
    extension(Color color)
    {
        /// <summary>
        /// Returns the <see cref="CieLabColor"/> for this color.
        /// </summary>
        /// <returns><see cref="CieLabColor"/> for this color.</returns>
        /// <remarks>
        /// The transformation is based on ITU-R recommendation BT.709 and
        /// uses D65 as reference white point.
        /// <para>
        /// The algorithmic error for the transformation is in the magnitude
        /// of 1e-5. Note that due to rounding to integers the error in the output may become 1.
        /// </para>
        /// </remarks>
        /// <see cref="CieLabColor.ToRGB"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CieLabColor ToCieLab()
        {
            // Based on https://kaizoudou.com/from-rgb-to-lab-color-space/

            Color rgbLinear = color.GammaCorrection(gammaExpansion: true);

            return Vector256.IsHardwareAccelerated
                ? CieHelper.ToCieLabVector256(rgbLinear)
                : CieHelper.ToCieLabScalar   (rgbLinear);
        }

        /// <summary>
        /// Returns the <see cref="CieXYZColor"/> for this color.
        /// </summary>
        /// <returns><see cref="CieXYZColor"/> for this color.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CieXYZColor ToCieXYZ()
        {
            // Based on http://www.brucelindbloom.com/index.html?Eqn_RGB_to_XYZ.html

            Color rgbLinear = color.GammaCorrection(gammaExpansion: true);

            return Vector256.IsHardwareAccelerated
                ? CieHelper.ToCieXYZVector256(rgbLinear)
                : CieHelper.ToCieXYZScalar   (rgbLinear);
        }
    }
}
