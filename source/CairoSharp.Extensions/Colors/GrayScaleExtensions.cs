// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.Intrinsics;

namespace Cairo.Extensions.Colors;

/// <summary>
/// Extensions for <see cref="Color"/>.
/// </summary>
public static class GrayScaleExtensions
{
    extension(Color color)
    {
        /// <summary>
        /// Converts the color to gray scale by the method given in <paramref name="grayScaleMode"/>.
        /// </summary>
        /// <param name="grayScaleMode">The mode for gray scale conversion</param>
        /// <returns>The color in gray scale.</returns>
        public Color ToGrayScale(GrayScaleMode grayScaleMode = GrayScaleMode.Luminosity)
        {
            double yLinear;

            switch (grayScaleMode)
            {
                case GrayScaleMode.Lightness:
                {
                    double most  = Math.Max(color.Red, color.Green, color.Blue);
                    double least = Math.Min(color.Red, color.Green, color.Blue);

                    Debug.Assert(0 <= most  && most  <= 1);
                    Debug.Assert(0 <= least && least <= 1);

                    yLinear = (most + least) * 0.5;
                    break;
                }
                case GrayScaleMode.Average:
                {
                    const double OneBy3 = 1d / 3d;

                    yLinear = (color.Red + color.Green + color.Blue) * OneBy3;
                    break;
                }
                case GrayScaleMode.Luminosity:
                {
                    const double WeightRed   = 0.2126;
                    const double WeightGreen = 0.7152;
                    const double WeightBlue  = 0.0722;

                    Debug.Assert(WeightRed + WeightGreen + WeightBlue == 1);

                    if (!Vector256.IsHardwareAccelerated)
                    {
                        yLinear = WeightRed   * color.Red
                                + WeightGreen * color.Green
                                + WeightBlue  * color.Blue;
                    }
                    else
                    {
                        Vector256<double> colorVec = color.AsVector256;
                        Vector256<double> weights  = Vector256.Create(WeightRed, WeightGreen, WeightBlue, 0);

                        colorVec *= weights;
                        yLinear   = colorVec[0] + colorVec[1] + colorVec[2];
                    }

                    break;
                }
                case GrayScaleMode.CieLab:
                {
                    CieLabColor cieLab = color.ToCieLab();

                    // L* is [0,100], scale to [0,1]
                    yLinear = cieLab.L * 1e-2;
                    break;
                }
                case GrayScaleMode.GammaExpandedAverage:
                {
                    Color rgbLinear = color.GammaCorrection(gammaExpansion: true);
                    return rgbLinear.ToGrayScale(GrayScaleMode.Average);
                }
                default:
                    throw new InvalidOperationException();
            }

            AssertInBounds(yLinear);
            return new Color(yLinear, yLinear, yLinear);

            [Conditional("DEBUG")]
            static void AssertInBounds(double yLinear)
            {
                if (yLinear < 0)
                {
                    Debug.Assert(-yLinear < 1e-3);
                }

                if (yLinear > 1)
                {
                    Debug.Assert(yLinear - 1 < 1e-3);
                }
            }
        }
    }
}
