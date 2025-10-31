// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Colors;

namespace CairoSharp.Extensions.Tests.Colors;

// https://colordesigner.io/convert/rgbtolab

[TestFixture]
public class CieLabColorTests
{
    [Test]
    public void RGB_black_color_to_CieLab___OK()
    {
        Color rgb          = new(0, 0, 0);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.Zero);
            Assert.That(cieLab.A, Is.Zero);
            Assert.That(cieLab.B, Is.Zero);
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero);
            Assert.That(actual.Green, Is.Zero);
            Assert.That(actual.Blue , Is.Zero);
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_white_color_to_CieLab___OK()
    {
        Color rgb          = new(1, 1, 1);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.EqualTo(100).Within(1e-2));
            Assert.That(cieLab.A, Is.Zero        .Within(1e-2));
            Assert.That(cieLab.B, Is.Zero        .Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Green, Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Blue , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_red_color_to_CieLab___OK()
    {
        Color rgb          = new(1, 0, 0);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.EqualTo(53.24).Within(1e-2));
            Assert.That(cieLab.A, Is.EqualTo(80.09).Within(1e-2));
            Assert.That(cieLab.B, Is.EqualTo(67.20).Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Green, Is.Zero      .Within(1e-2));
            Assert.That(actual.Blue , Is.Zero      .Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-2));
        }
    }

    [Test]
    public void RGB_green_color_to_CieLab___OK()
    {
        Color rgb          = new(0, 1, 0);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.EqualTo( 87.73).Within(1e-2));
            Assert.That(cieLab.A, Is.EqualTo(-86.18).Within(1e-2));
            Assert.That(cieLab.B, Is.EqualTo( 83.18).Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero      .Within(1e-2));
            Assert.That(actual.Green, Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Blue , Is.Zero      .Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-2));
        }
    }

    [Test]
    public void RGB_blue_color_to_CieLab___OK()
    {
        Color rgb          = new(0, 0, 1);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.EqualTo(  32.30).Within(1e-2));
            Assert.That(cieLab.A, Is.EqualTo(  79.19).Within(1e-2));
            Assert.That(cieLab.B, Is.EqualTo(-107.86).Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero      .Within(1e-2));
            Assert.That(actual.Green, Is.Zero      .Within(1e-2));
            Assert.That(actual.Blue , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-2));
        }
    }

    [Test]
    public void RGB_to_CieLab_and_back()
    {
        double r = TestContext.CurrentContext.Random.NextDouble();
        double g = TestContext.CurrentContext.Random.NextDouble();
        double b = TestContext.CurrentContext.Random.NextDouble();

        Color rgb          = new(r, g, b);
        CieLabColor cieLab = rgb.ToCieLab();
        Color actual       = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(rgb.Red)  .Within(1e-2));
            Assert.That(actual.Green, Is.EqualTo(rgb.Green).Within(1e-2));
            Assert.That(actual.Blue , Is.EqualTo(rgb.Blue) .Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(rgb.Alpha));
        }
    }
}
