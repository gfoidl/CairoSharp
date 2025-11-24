// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        /// <exception cref="ArgumentNullException">when <paramref name="text"/> is <c>null</c></exception>
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
        /// Gets the width of <paramref name="utf8"/> for the current font.
        /// </summary>
        /// <param name="utf8">a NUL-terminated string of text, encoded in UTF-8</param>
        /// <returns>the width of the text</returns>
        /// <remarks>
        /// See <see cref="Cairo.TextExtensions.TextExtents(CairoContext, string?, out TextExtents)"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">when <paramref name="text"/> is <c>null</c></exception>
        public double GetTextWidth(ReadOnlySpan<byte> utf8)
        {
            if (utf8.IsEmpty)
            {
                Throw();

                [DoesNotReturn]
                static void Throw() => throw new ArgumentNullException(nameof(utf8));
            }

            cr.TextExtents(utf8, out TextExtents textExtents);
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
        /// <exception cref="ArgumentNullException">when <paramref name="text"/> is <c>null</c></exception>
        public PointD TextAlignCenter(string text, double baseWidth, double baseHeight, out TextExtents textExtents, bool moveCurrentPoint = false)
        {
            ArgumentNullException.ThrowIfNull(text);

            // Based on https://www.cairographics.org/samples/text_align_center/

            cr.TextExtents(text, out textExtents);

            return cr.TextAlignCenter(baseWidth, baseHeight, textExtents, moveCurrentPoint);
        }

        /// <summary>
        /// Computes the origin for <paramref name="utf8"/> when aligned centered in both x and y
        /// direction.
        /// </summary>
        /// <param name="utf8">a NUL-terminated string of text, encoded in UTF-8</param>
        /// <param name="baseWidth">the widht of the base element where the text should be centered</param>
        /// <param name="baseHeight">the height of the base element where the text should be centered</param>
        /// <param name="textExtents">the computed <see cref="TextExtents"/></param>
        /// <param name="moveCurrentPoint">
        /// when <c>true</c> moves cairos current point to the calculated position
        /// </param>
        /// <returns>the point to which <see cref="PathExtensions.MoveTo(CairoContext, PointD)"/></returns>
        /// <exception cref="ArgumentNullException">when <paramref name="text"/> is <c>null</c></exception>
        public PointD TextAlignCenter(ReadOnlySpan<byte> utf8, double baseWidth, double baseHeight, out TextExtents textExtents, bool moveCurrentPoint = false)
        {
            if (utf8.IsEmpty)
            {
                Throw();

                [DoesNotReturn]
                static void Throw()=> throw new ArgumentNullException(nameof(utf8));
            }

            // Based on https://www.cairographics.org/samples/text_align_center/

            cr.TextExtents(utf8, out textExtents);

            return cr.TextAlignCenter(baseWidth, baseHeight, textExtents, moveCurrentPoint);
        }

        private PointD TextAlignCenter(double baseWidth, double baseHeight, in TextExtents textExtents, bool moveCurrentPoint)
        {
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
