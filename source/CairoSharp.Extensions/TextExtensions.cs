// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Fonts;

namespace Cairo.Extensions;

/// <summary>
/// Extensions for the <see cref="CairoContext"/>.
/// </summary>
public static class TextExtensions
{
    extension(CairoContext cr)
    {
        /// <summary>
        /// Gets the width of <paramref name="text"/> for the current font.
        /// </summary>
        /// <param name="text">the text</param>
        /// <returns>the width of the text</returns>
        /// <remarks>
        /// See <see cref="Cairo.TextExtensions.TextExtents(CairoContext, string?, out TextExtents)"/>.
        /// </remarks>
        public double GetTextWidth(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            cr.TextExtents(text, out TextExtents textExtents);

#if DEBUG
            double textWidth    = 0;
            double textXAdvance = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                string s = text.Substring(i, 1);
                cr.TextExtents(s, out TextExtents tmp);

                textWidth    += tmp.Width;
                textXAdvance += tmp.XAdvance;
            }

            Debug.WriteLine($"""
                Text: '{text}'
                Width:    {textExtents.Width}
                XAdvance: {textExtents.XAdvance}
                sum single char width:    {textWidth}
                sum single char xAdvance: {textXAdvance}
                """);
#endif

            return textExtents.Width;
        }

        /// <summary>
        /// Computes the origin for <paramref name="text"/> when aligned centered in both x and y
        /// direction.
        /// </summary>
        /// <param name="text">the text to center align</param>
        /// <param name="baseWidth">the widht of the base element where the text should be centered</param>
        /// <param name="baseHeight">the height of the base element where the text should be centered</param>
        /// <param name="textExtents">the computed <see cref="TextExtents"/></param>
        /// <param name="moveCurrentPoint">
        /// when <c>true</c> moves cairos current point to the calculated position
        /// </param>
        /// <returns>the point to which <see cref="PathExtensions.MoveTo(CairoContext, PointD)"/></returns>
        public PointD TextAlignCenter(string text, double baseWidth, double baseHeight, out TextExtents textExtents, bool moveCurrentPoint = false)
        {
            // Based on https://www.cairographics.org/samples/text_align_center/

            cr.TextExtents(text, out textExtents);

            double x = baseWidth  / 2 - (textExtents.Width  / 2 + textExtents.XBearing);
            double y = baseHeight / 2 - (textExtents.Height / 2 + textExtents.YBearing);

            if (moveCurrentPoint)
            {
                cr.MoveTo(x, y);
            }

            return new PointD(x, y);
        }
    }
}
