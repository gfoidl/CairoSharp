// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Colors;

internal static class MathExtensions
{
    extension(Math)
    {
        public static double Max(double a, double b, double c) => Math.Max(a, Math.Max(b, c));
        public static double Min(double a, double b, double c) => Math.Min(a, Math.Min(b, c));
    }
}
