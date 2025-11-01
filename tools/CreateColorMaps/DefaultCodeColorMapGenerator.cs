// (c) gfoidl, all rights reserved

namespace CreateColorMaps;

internal sealed class DefaultCodeColorMapGenerator(string name, string? comment = null)
    : ColorMapGenerator(name, comment)
{
    private const int Entries = 256;

    protected override string Type => "Default";
    //-------------------------------------------------------------------------
    protected override IEnumerable<(double Red, double Green, double Blue)> GetColors()
        => _name switch
        {
            "Autumn"  => CreateAutumn(),
            "Gray"    => CreateGray(),
            "Rainbow" => CreateRainbow(),
            "Sine"    => CreateSine(),
            _         => throw new InvalidOperationException()
        };
    //-------------------------------------------------------------------------
    private static IEnumerable<(double Red, double Green, double Blue)> CreateAutumn()
    {
        for (int i = 0; i < Entries; ++i)
        {
            double r = 1;
            double g = i / (Entries - 1d);
            double b = 0;

            yield return (r, g, b);
        }
    }
    //-------------------------------------------------------------------------
    private static IEnumerable<(double Red, double Green, double Blue)> CreateGray()
    {
        for (int i = 0; i < Entries; ++i)
        {
            double val = i / (Entries - 1d);

            yield return (val, val, val);
        }
    }
    //-------------------------------------------------------------------------
    private static IEnumerable<(double Red, double Green, double Blue)> CreateRainbow()
    {
        for (int i = 0; i < Entries; ++i)
        {
            const int Middle = Entries / 2;

            double r, g, b;

            if (i > Middle)
            {
                int value = (int)Scale(i, Middle, 255, 0, 255);
                r         = value;
                g         = 255 - value;
                b         = 0;
            }
            else
            {
                int value = (int)Scale(i, 0, Middle, 0, 255);
                r         = 0;
                g         = value;
                b         = 255 - value;
            }

            r /= (Entries - 1d);
            g /= (Entries - 1d);
            b /= (Entries - 1d);

            yield return (r, g, b);
        }

        static double Scale(double value, double a, double b, double c, double d)
        {
            // Naive implementation of scaling. [a,b] -> [0,1] -> [c,d]
            return (value - a) / (b - a) * (d - c) + c;
        }
    }
    //-------------------------------------------------------------------------
    private static IEnumerable<(double Red, double Green, double Blue)> CreateSine()
    {
        for (int i = 0; i < Entries; ++i)
        {
            // new color in [-1,1]
            double r = Math.Sin(i * 2 * Math.PI / 255d - Math.PI);
            double g = Math.Sin(i * 2 * Math.PI / 255d - Math.PI / 2);
            double b = Math.Sin(i * 2 * Math.PI / 255d);

            // [-1,1] -> [0,1]
            r = (r + 1) * 0.5;
            g = (g + 1) * 0.5;
            b = (b + 1) * 0.5;

            yield return (r, g, b);
        }
    }
}
