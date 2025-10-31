// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Colors;

namespace CairoSharp.Extensions.Tests.Colors.ColorExtensionsTests;

[TestFixture]
public class GammaCorrection
{
    [Test]
    public void Compression_and_Expansion()
    {
        double r = TestContext.CurrentContext.Random.NextDouble();
        double g = TestContext.CurrentContext.Random.NextDouble();
        double b = TestContext.CurrentContext.Random.NextDouble();

        Color sRGB   = new(r, g, b);
        Color rgb    = sRGB.GammaCorrection(gammaExpansion: true);
        Color actual = rgb.GammaCorrection(gammaExpansion: false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(sRGB.Red)  .Within(1e-5));
            Assert.That(actual.Green, Is.EqualTo(sRGB.Green).Within(1e-5));
            Assert.That(actual.Blue , Is.EqualTo(sRGB.Blue) .Within(1e-5));
            Assert.That(actual.Alpha, Is.EqualTo(sRGB.Alpha).Within(1e-5));
        }
    }
}
