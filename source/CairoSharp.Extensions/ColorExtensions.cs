// (c) gfoidl, all rights reserved

using System.Diagnostics;

namespace Cairo.Extensions;

/// <summary>
/// A color given in the <a href="https://en.wikipedia.org/wiki/HSL_and_HSV">HSV</a> color space.
/// </summary>
/// <param name="Hue">hue component of color</param>
/// <param name="Saturation">saturation component of color</param>
/// <param name="Value">value component of color</param>
[DebuggerNonUserCode]
public readonly record struct HsvColor(double Hue, double Saturation, double Value);

/// <summary>
/// Extensions for <see cref="Color"/>.
/// </summary>
public static class ColorExtensions
{
    private const double By255 = 1d / 255d;

    extension(Color color)
    {
        /// <summary>
        /// Creates a cairo <see cref="Color"/> where the red, green, and blue components are
        /// given in byte values [0, 255].
        /// </summary>
        /// <param name="red">red component of color</param>
        /// <param name="green">green component of color</param>
        /// <param name="blue">blue component of color</param>
        public static Color FromRgbBytes(byte red, byte green, byte blue) => new(red * By255, green * By255, blue * By255);

        /// <summary>
        /// Creates a cairo <see cref="Color"/> where the red, green, and blue components are
        /// given in byte values [0, 255].
        /// </summary>
        /// <param name="red">red component of color</param>
        /// <param name="green">green component of color</param>
        /// <param name="blue">blue component of color</param>
        /// <param name="alpha">alpha component of color</param>
        public static Color FromRgbaBytes(byte red, byte green, byte blue, byte alpha) => new(red * By255, green * By255, blue * By255, alpha * By255);

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

            return new Color(1 - color.Red, 1 - color.Green, 1 - color.Blue);
        }

        /// <summary>
        /// Returns the <see cref="HsvColor"/> for this color.
        /// </summary>
        public HsvColor ToHSV()
        {
            double r = color.Red;
            double g = color.Green;
            double b = color.Blue;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double h = 0;

            if (max == r)
            {
                if (g > b)
                {
                    h = 60 * (g - b) / (max - min);
                }
                else if (g < b)
                {
                    h = 60 * (g - b) / (max - min) + 360;
                }
            }
            else if (max == g)
            {
                h = 60 * (b - r) / (max - min) + 120;
            }
            else if (max == b)
            {
                h = 60 * (r - g) / (max - min) + 240;
            }

            double s = (max == 0) ? 0 : 1 - min / max;

            return new HsvColor(h, s, max);
        }
    }
}
