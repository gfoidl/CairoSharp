// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Colors;

namespace CairoSharp.Extensions.Tests.Colors.ColorExtensionsTests;

[TestFixture]
public class FromRgbaBytes
{
    [Test, TestCaseSource(nameof(Vectorized_conversion___OK_TestCases))]
    public Color Vectorized_conversion___OK(int red, int green, int blue, int alpha)
    {
        return Color.FromRgbaBytes((byte)red, (byte)green, (byte)blue, (byte)alpha);
    }

    private static IEnumerable<TestCaseData> Vectorized_conversion___OK_TestCases()
    {
        yield return new TestCaseData(0xFF,    0,    0, 0xFF).Returns(new Color(1, 0, 0, 1));
        yield return new TestCaseData(   0, 0xFF,    0, 0xFF).Returns(new Color(0, 1, 0, 1));
        yield return new TestCaseData(   0,    0, 0xFF, 0xFF).Returns(new Color(0, 0, 1, 1));
        yield return new TestCaseData(0xFF, 0xFF, 0xFF, 0xFF).Returns(new Color(1, 1, 1, 1));
    }

    [Test, Repeat(100)]
    public void Random_color_components___OK()
    {
        byte red   = TestContext.CurrentContext.Random.NextByte();
        byte green = TestContext.CurrentContext.Random.NextByte();
        byte blue  = TestContext.CurrentContext.Random.NextByte();
        byte alpha = TestContext.CurrentContext.Random.NextByte();

        Color expected = new(red / 255d, green / 255d, blue / 255d, alpha / 255d);
        Color actual   = Color.FromRgbaBytes(red, green, blue, alpha);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(expected.Red)  .Within(1e-5));
            Assert.That(actual.Green, Is.EqualTo(expected.Green).Within(1e-5));
            Assert.That(actual.Blue , Is.EqualTo(expected.Blue) .Within(1e-5));
            Assert.That(actual.Alpha, Is.EqualTo(expected.Alpha).Within(1e-5));
        }
    }
}
