// (c) gfoidl, all rights reserved

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Cairo.Extensions.Colors;

/// <summary>
/// Extensions for <see cref="Color"/>.
/// </summary>
public static class ColorExtensions
{
    private const double By255 = 1d / 255d;

    extension(Color color)
    {
        /// <summary>
        /// Creates a <see cref="Color"/> where the red, green, and blue components are
        /// given in byte values [0, 255].
        /// </summary>
        /// <param name="red">red component of color</param>
        /// <param name="green">green component of color</param>
        /// <param name="blue">blue component of color</param>
        /// <returns>the <see cref="Color"/></returns>
        public static Color FromRgbBytes(byte red, byte green, byte blue)
            => FromRgbaBytes(red, green, blue, 0xFF);

        /// <summary>
        /// Creates a <see cref="Color"/> where the red, green, and blue components are
        /// given in byte values [0, 255].
        /// </summary>
        /// <param name="red">red component of color</param>
        /// <param name="green">green component of color</param>
        /// <param name="blue">blue component of color</param>
        /// <param name="alpha">alpha component of color</param>
        /// <returns>the <see cref="Color"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromRgbaBytes(byte red, byte green, byte blue, byte alpha)
        {
            if (!Vector256.IsHardwareAccelerated)
            {
                return new(red * By255, green * By255, blue * By255, alpha * By255);
            }

            Vector256<long> tmp        = Vector256.Create(red, green, blue, alpha);
            Vector256<double> vec      = Vector256.ConvertToDouble(tmp);
            Vector256<double> by255Vec = Vector256.Create(By255);
            vec *= by255Vec;

            return Color.FromVector256(vec);
        }

        /// <summary>
        /// Creates a <see cref="Color"/> where the red, green, blue, and alpha components
        /// are given as hex-string in the form <c>#RRGGBBAA</c> or <c>#RRGGBB</c>.
        /// </summary>
        /// <param name="hexColor">
        /// hex-string of the color in the form <c>#RRGGBBAA</c> or <c>#RRGGBB</c>
        /// </param>
        /// <returns>the <see cref="Color"/></returns>
        /// <exception cref="ArgumentException">
        /// The given <paramref name="hexColor"/> is invalid to the expected formats.
        /// </exception>
        public static Color FromHex(ReadOnlySpan<char> hexColor)
        {
#if NET9_0_OR_GREATER
            if (!hexColor.StartsWith('#'))
#else
            if (hexColor.IsEmpty || hexColor[0] != '#')
#endif
            {
                Throw(hexColor);

                [DoesNotReturn]
                static void Throw(ReadOnlySpan<char> hexColor)
                    => throw new ArgumentException($"The given hex color '{hexColor}' does not start with '#'");
            }

            // Slice off #
            ReadOnlySpan<char> hexValues = hexColor.Slice(1);

            if (hexValues.Length is not (8 or 6))
            {
                ThrowInvalidFormat(hexColor);
            }

#if NET9_0_OR_GREATER
            // stackalloc is quite heavy here with the stack cookie, etc.
            int tmp           = 0;
            Span<byte> buffer = MemoryMarshal.CreateSpan(ref Unsafe.As<int, byte>(ref tmp), sizeof(int));

            OperationStatus status = Convert.FromHexString(hexValues, buffer, out int consumed, out int written);

            if (status != OperationStatus.Done)
            {
                ThrowInvalidFormat(hexColor);
            }

            Debug.Assert(consumed == hexValues.Length);
            Debug.Assert(written is 3 or 4);

            byte r = buffer[0];
            byte g = buffer[1];
            byte b = buffer[2];
            byte a = written == 4 ? buffer[3] : (byte)0xFF;
#else
            byte[] buffer = Convert.FromHexString(hexValues);

            Debug.Assert(buffer.Length is 3 or 4);

            byte r = buffer[0];
            byte g = buffer[1];
            byte b = buffer[2];
            byte a = buffer.Length == 4 ? buffer[3] : (byte)0xFF;
#endif

            return Color.FromRgbaBytes(r, g, b, a);

            [DoesNotReturn]
            static void ThrowInvalidFormat(ReadOnlySpan<char> hexColor)
                => throw new ArgumentException($"The given hex color '{hexColor}' does not expect the format #RRGGBBAA or #RRGGBB");
        }

        /// <summary>
        /// Gets the inverse color, that is for each component <c>1 - component</c>.
        /// </summary>
        /// <returns>The inverse color</returns>
        public Color GetInverseColor()
        {
            if (color == KnownColors.Gray)
            {
                return new Color(1, 1, 1);
            }

            if (!Vector256.IsHardwareAccelerated)
            {
                return new Color(1 - color.Red, 1 - color.Green, 1 - color.Blue);
            }

            Vector256<double> vec  = color.AsVector256;
            Vector256<double> ones = Vector256.Create(1d);

            vec = ones - vec;
            vec = vec.WithElement(3, color.Alpha);

            return Color.FromVector256(vec);
        }

        /// <summary>
        /// Applies gamma correction to the color (sRGB &lt;-&gt; RGB).
        /// </summary>
        /// <param name="gammaExpansion">
        /// When <c>true</c> performs gamma expansion, i.e. undoes gamma compression and thus
        /// transforms from sRGB to linearized RGB (physically linear properties).<br />
        /// When <c>false</c> performs gamma compression, i.e. transforms linearized RGB to sRGB.
        /// </param>
        /// <returns>gamma corrected color depending on <paramref name="gammaExpansion"/></returns>
        /// <remarks>
        /// We are able to see small differences when luminance is low, but at high luminance levels, we are
        /// much less sensitive to them. In order to avoid wasting effort representing imperceptible differences
        /// at high luminance, the color scale is warped, so that it concentrates more values in the lower
        /// end of the range, and spreads them out more widely in the higher end. This is called
        /// gamma compression (<paramref name="gammaExpansion"/> set to <c>false</c>).
        /// <para>
        /// To undo the effects of gamma compression, it's necessary to apply the inverse operation, gamma expansion
        /// (<paramref name="gammaExpansion"/> set to <c>true</c>).
        /// </para>
        /// </remarks>
        public Color GammaCorrection(bool gammaExpansion = true)
        {
            if (gammaExpansion)
            {
                double r = GammaExpansion(color.Red);
                double g = GammaExpansion(color.Green);
                double b = GammaExpansion(color.Blue);

                return new Color(r, g, b);
            }
            else
            {
                double sr = GammaCompression(color.Red);
                double sg = GammaCompression(color.Green);
                double sb = GammaCompression(color.Blue);

                return new Color(sr, sg, sb);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static double GammaExpansion(double value)
            {
                if (value > 0.04045)
                {
                    value = (value + 0.055) / 1.055;
                    return Math.Pow(value, 2.4);
                }

                return value / 12.92;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static double GammaCompression(double value)
            {
                if (value > 0.04045 / 12.92)
                {
                    return 1.055 * Math.Pow(value, 1d / 2.4) - 0.055;
                }

                return value * 12.92;
            }
        }
    }
}
