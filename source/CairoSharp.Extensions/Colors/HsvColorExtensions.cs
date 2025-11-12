// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Colors;

/// <summary>
/// Extensions for <see cref="Color"/>.
/// </summary>
public static class HsvColorExtensions
{
    extension(Color color)
    {
        /// <summary>
        /// Returns the <see cref="HsvColor"/> for this color.
        /// </summary>
        /// <returns>
        /// <see cref="HsvColor"/> for this color.
        /// </returns>
        /// <seealso cref="HsvColor.ToRGB"/>
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
