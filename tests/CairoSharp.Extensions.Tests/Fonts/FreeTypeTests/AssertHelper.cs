// (c) gfoidl, all rights reserved

using System.Text;

namespace CairoSharp.Extensions.Tests.Fonts.FreeTypeTests;

internal static class AssertHelper
{
    public static readonly byte[] s_expectedSvgDataFontOptions = File.ReadAllBytes("Fonts/font-options.svg");
    public static readonly byte[] s_expectedSvgDataText        = File.ReadAllBytes("Fonts/text.svg");

    public static readonly string s_expectedSvgStringFontOptions = Encoding.UTF8.GetString(s_expectedSvgDataFontOptions).Replace("\r\n", "\n");
    public static readonly string s_expectedSvgStringText        = Encoding.UTF8.GetString(s_expectedSvgDataText)       .Replace("\r\n", "\n");
    //-------------------------------------------------------------------------
    public static void AssertSvg(ReadOnlySpan<byte> expectedData, string expectedString, byte[] actual)
    {
        if (expectedData.SequenceEqual(actual))
        {
            return;
        }

        string actualSvg = Encoding.UTF8.GetString(actual).Replace("\r\n", "\n");
        Assert.That(actualSvg, Is.EqualTo(expectedString));
    }
}
