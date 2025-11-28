// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Cairo.Extensions.Pixels;

internal static class PixelHelper
{
    // In Format.Argb32 pre-multiplied alpha is used.

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetColor(ReadOnlySpan<byte> data, int idx)
    {
        if (idx > data.Length - 4)
        {
            ThrowIndexOutOfRange(data.Length, idx);
        }

        ref byte ptr = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), (uint)idx);

        byte b = Unsafe.Add(ref ptr, 0);
        byte g = Unsafe.Add(ref ptr, 1);
        byte r = Unsafe.Add(ref ptr, 2);
        byte a = Unsafe.Add(ref ptr, 3);

        const double OneBy255 = 1d / 255;

        if (a == 0xFF)
        {
            double alpha = 1d;
            double red   = r * OneBy255;
            double green = g * OneBy255;
            double blue  = b * OneBy255;

            return new Color(red, green, blue, alpha);
        }
        else
        {
            return GetForAlpha(a, r, g, b);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        static Color GetForAlpha(byte a, byte r, byte g, byte b)
        {
            double alpha  = a * OneBy255;
            double oneByA = 1d / a;

            double red   = r * oneByA;
            double green = g * oneByA;
            double blue  = b * oneByA;

            return new Color(red, green, blue, alpha);
        }
    }
    //-------------------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetColor(Span<byte> data, int idx, Color color)
    {
        if (idx > data.Length - 4)
        {
            ThrowIndexOutOfRange(data.Length, idx);
        }

        if (color.Alpha == 1d)
        {
            byte r, g, b, a;

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

            a = 0xFF;
            Write(data, idx, a, r, g, b);
        }
        else
        {
            SetColorForAlpha(data, idx, color);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void SetColorForAlpha(Span<byte> data, int idx, Color color)
        {
            byte r, g, b, a;
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
            Write(data, idx, a, r, g, b);
        }
        //---------------------------------------------------------------------
        static byte ToByte(double x)
        {
#if NET9_0_OR_GREATER
            return double.ConvertToIntegerNative<byte>(x);
#else
            return (byte)x;
#endif
        }
        //---------------------------------------------------------------------
        static void Write(Span<byte> data, int idx, byte a, byte r, byte g, byte b)
        {
            ref byte ptr = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), (uint)idx);

            Unsafe.Add(ref ptr, 0) = b;
            Unsafe.Add(ref ptr, 1) = g;
            Unsafe.Add(ref ptr, 2) = r;
            Unsafe.Add(ref ptr, 3) = a;
        }
    }
    //-------------------------------------------------------------------------
    private static void ThrowIndexOutOfRange(int length, int idx)
    {
        throw new ArgumentOutOfRangeException(
            nameof(idx),
            $"Index {idx} is out of range for span of length {length} in order to read or write a ARGB color in bytes");
    }
}
