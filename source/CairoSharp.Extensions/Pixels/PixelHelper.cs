// (c) gfoidl, all rights reserved

using Cairo.Extensions.Colors;
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

        // Endianess is handled by the read of the int for us
        int value = Unsafe.ReadUnaligned<int>(ref ptr);

        // The machine code of this is a bit longer as compared to a (value >>= 8)-variant,
        // but here is no register dependency on (value), thus the CPU can execute this in
        // instruction level parallelism.
        byte a = (byte)(value >> 24);
        byte r = (byte)(value >> 16);
        byte g = (byte)(value >>  8);
        byte b = (byte)value;

        if (a == 0xFF)
        {
            return Color.FromRgbaBytes(r, g, b, 0xFF);
        }
        else
        {
            return GetForPremultipliedAlpha(a, r, g, b);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        static Color GetForPremultipliedAlpha(byte a, byte r, byte g, byte b)
        {
            const double OneBy255 = 1d / 255;

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
            SetColorForPremultipliedAlpha(data, idx, color);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void SetColorForPremultipliedAlpha(Span<byte> data, int idx, Color color)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Write(Span<byte> data, int idx, byte a, byte r, byte g, byte b)
        {
            ref byte ptr = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), (uint)idx);

            int value = a << 24 | r << 16 | g << 8 | b;

            // Endianess is handled by the write of the int for us.
            Unsafe.WriteUnaligned(ref ptr, value);
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
