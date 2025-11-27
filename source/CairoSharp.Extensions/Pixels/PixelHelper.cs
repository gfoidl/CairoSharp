// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Cairo.Extensions.Pixels;

internal static class PixelHelper
{
    // In Format.Argb32 pre-multiplied alpha is used.

    public static Color GetColor(ReadOnlySpan<byte> data, int idx)
    {
        data   = data.Slice(idx);
        byte a = data[3];
        byte r = data[2];
        byte g = data[1];
        byte b = data[0];

        const double OneBy255 = 1d / 255;

        double red, green, blue, alpha;

        if (a == 0xFF)
        {
            alpha = 1d;
            red   = r * OneBy255;
            green = g * OneBy255;
            blue  = b * OneBy255;
        }
        else
        {
            alpha         = a * OneBy255;
            double oneByA = 1d / a;

            red   = r * oneByA;
            green = g * oneByA;
            blue  = b * oneByA;

        }

        return new Color(red, green, blue, alpha);
    }
    //-------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetColor(Span<byte> data, int idx, Color color)
    {
        byte r, g, b, a;

        if (color.Alpha == 1d)
        {
            a = 0xFF;

            if (!Vector256.IsHardwareAccelerated)
            {
                r = ToByte(color.Red   * 255d);
                g = ToByte(color.Green * 255d);
                b = ToByte(color.Blue  * 255d);
            }
            else
            {
                Vector256<double> vec = color.AsVector256 * Vector256.Create(255d);
                r = ToByte(vec[0]);
                g = ToByte(vec[1]);
                b = ToByte(vec[2]);
            }
        }
        else
        {
            double premult = color.Alpha * 255d;

            if (!Vector256.IsHardwareAccelerated)
            {
                r = ToByte(color.Red   * premult);
                g = ToByte(color.Green * premult);
                b = ToByte(color.Blue  * premult);
            }
            else
            {
                Vector256<double> vec = color.AsVector256 * Vector256.Create(color.Alpha * 255d);
                r = ToByte(vec[0]);
                g = ToByte(vec[1]);
                b = ToByte(vec[2]);
            }

            a = ToByte(premult);
        }

        data    = data.Slice(idx);
        data[3] = a;
        data[2] = r;
        data[1] = g;
        data[0] = b;
        //---------------------------------------------------------------------
        static byte ToByte(double x)
        {
#if NET9_0_OR_GREATER
            return double.ConvertToIntegerNative<byte>(x);
#else
            return (byte)x;
#endif
        }
    }
}
