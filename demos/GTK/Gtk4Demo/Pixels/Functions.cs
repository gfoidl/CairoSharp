// (c) gfoidl, all rights reserved

namespace Gtk4Demo.Pixels;

internal interface IFunction
{
    static abstract double Calculate(double x, double y);
}

internal struct PeaksFunction : IFunction
{
    public static double Calculate(double x, double y)
    {
        //  z = f(x, y) = 3 (1 - x)^2 \cdot e^{-x^2 - (y+1)^2} - 10 \left( \frac{x}{5} - x^3 - y^5 \right) \cdot e^{-x^2 - y^2} - \frac{1}{3} \cdot e^{-(x+1)^2 - y^2}

        double z =
            3 * (1 - x) * (1 - x) *
            Math.Exp(-x * x) -
            (y + 1) * (y + 1) -
            10 * (x / 5 - x * x * x - y * y * y * y * y) *
            Math.Exp(-x * x - y * y) -
            1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);

        return z;
    }
}

internal struct MexicanHatFunction : IFunction
{
    public static double Calculate(double x, double y)
    {
        // z = f(x, y) = \frac{1}{\pi \sigma^4} \left( 1 - \frac{x^2 + y^2}{\sigma^2} \right) \cdot e^{-\frac{x^2 + y^2}{2\sigma^2}}

        const double Sigma = 0.75;

        double z =
            1d / (Math.PI * Sigma * Sigma * Sigma * Sigma) *
            (1 - (x * x + y * y) / (Sigma * Sigma)) *
            Math.Exp(-(x * x + y * y) / (2 * Sigma * Sigma));

        return z;
    }
}
