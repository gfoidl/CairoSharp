// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Cairo.Extensions.Colors;

internal static class CieLabHelper
{
    public static CieLabColor ToCieLabScalar(Color rgbLinear)
    {
        double r = rgbLinear.Red;
        double g = rgbLinear.Green;
        double b = rgbLinear.Blue;

        // RGB -> XYZ, thereby use the same reference white D65.
        double x = 0.4124564 * r + 0.3575761 * g + 0.1804375 * b;
        double y = 0.2126729 * r + 0.7151522 * g + 0.0721750 * b;
        double z = 0.0193339 * r + 0.1191920 * g + 0.9503041 * b;

        // Normalize to reference white
        //x /= CieLabColor.Xn.L;
        //y /= CieLabColor.Xn.A;    // = 1, so skip
        //z /= CieLabColor.Xn.B;
        x /= 0.950456;
        z /= 1.088754;

        double fx = f_of_t(x);
        double fy = f_of_t(y);
        double fz = f_of_t(z);

        return CreateFromFValues(fx, fy, fz);
    }

    public static CieLabColor ToCieLabVector256(Color rgbLinear)
    {
        Debug.Assert(Vector256.IsHardwareAccelerated);

        // RGB -> XYZ, thereby use the same reference white D65.
        Vector256<double> xyzVec;
        xyzVec  = Vector256.Create(0.4124564, 0.2126729, 0.0193339, 0) * Vector256.Create(rgbLinear.Red);
        xyzVec += Vector256.Create(0.3575761, 0.7151522, 0.1191920, 0) * Vector256.Create(rgbLinear.Green);
        xyzVec += Vector256.Create(0.1804375, 0.0721750, 0.9503041, 0) * Vector256.Create(rgbLinear.Blue);

        // Normalize to reference white
        xyzVec /= CieLabColor.XnVec;

        double fx = f_of_t(xyzVec[0]);
        double fy = f_of_t(xyzVec[1]);
        double fz = f_of_t(xyzVec[2]);

        return CreateFromFValues(fx, fy, fz);
    }

    public static Color ToRGB(CieLabColor cieLabColor)
    {
        double y = (cieLabColor.L + 16d) / 116d;
        double x = cieLabColor.A / 500d + y;
        double z = y - cieLabColor.B / 200d;

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
        //x *= CieLabColor.Xn.L;
        //y *= CieLabColor.Xn.A;    // = 1, so skip
        //z *= CieLabColor.Xn.B;
        x *= 0.950456;
        z *= 1.088754;

        // XYZ -> RGB, thereby use the same reference white D65.
        // This is the inverse matrix from above.
        double r, g, b;
        if (!Vector256.IsHardwareAccelerated)
        {
            r =  3.24045480 * x + -1.53713890 * y + -0.49853155 * z;
            g = -0.96926639 * x +  1.87601093 * y +  0.04155608 * z;
            b =  0.05564342 * x + -0.20402585 * y +  1.05722516 * z;

            r = Math.Clamp(r, 0, 1);
            g = Math.Clamp(g, 0, 1);
            b = Math.Clamp(b, 0, 1);
        }
        else
        {
            Vector256<double> rgbVec;
            rgbVec  = Vector256.Create( 3.24045480, -0.96926639,  0.05564342, 0) * Vector256.Create(x);
            rgbVec += Vector256.Create(-1.53713890,  1.87601093, -0.20402585, 0) * Vector256.Create(y);
            rgbVec += Vector256.Create(-0.49853155,  0.04155608,  1.05722516, 0) * Vector256.Create(z);

#if NET9_0_OR_GREATER
            rgbVec = Vector256.ClampNative(rgbVec, Vector256<double>.Zero, Vector256<double>.One);
#endif
            r = rgbVec[0];
            g = rgbVec[1];
            b = rgbVec[2];

#if !NET9_0_OR_GREATER
            r = Math.Clamp(r, 0, 1);
            g = Math.Clamp(g, 0, 1);
            b = Math.Clamp(b, 0, 1);
#endif
        }

        Color color = new(r, g, b);
        return color.GammaCorrection(gammaExpansion: false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double f_of_t(double t)
    {
        if (t > 0.008856)
        {
            return Math.Pow(t, 1d / 3d);
        }

        return 7.787 * t + 16d / 116d;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double Inv_f_of_t(double value)
    {
        double value3 = value * value * value;

        if (value3 > 0.008856)
        {
            return value3;
        }

        return (value - 16d / 116d) / 7.787;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CieLabColor CreateFromFValues(double fx, double fy, double fz)
    {
        double cie_L = 116 * fy - 16;
        double cie_a = 500 * (fx - fy);
        double cie_b = 200 * (fy - fz);

        return new CieLabColor(cie_L, cie_a, cie_b);
    }
}
