// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Colors;

namespace CairoSharp.Extensions.Tests.Colors;

// https://www.easyrgb.com/en/convert.php#inputFORM
// https://colordesigner.io/convert/rgbtolab

[TestFixture]
public class CieXYZColorTests
{
    [Test]
    public void RGB_black_color_to_CieXYZ___OK()
    {
        Color rgb          = new(0, 0, 0);
        CieXYZColor cieXYZ = rgb.ToCieXYZ();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieXYZ.X, Is.Zero);
            Assert.That(cieXYZ.Y, Is.Zero);
            Assert.That(cieXYZ.Z, Is.Zero);
        }

        Color actual = cieXYZ.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero);
            Assert.That(actual.Green, Is.Zero);
            Assert.That(actual.Blue , Is.Zero);
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_white_color_to_CieXYZ___OK()
    {
        Color rgb          = new(1, 1, 1);
        CieXYZColor cieXYZ = rgb.ToCieXYZ();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieXYZ.X, Is.EqualTo( 95.047 / 100).Within(1e-3));
            Assert.That(cieXYZ.Y, Is.EqualTo(100.000 / 100).Within(1e-3));
            Assert.That(cieXYZ.Z, Is.EqualTo(108.883 / 100).Within(1e-3));
        }

        Color actual = cieXYZ.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Green, Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Blue , Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_red_color_to_CieXYZ___OK()
    {
        Color rgb          = new(1, 0, 0);
        CieXYZColor cieXYZ = rgb.ToCieXYZ();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieXYZ.X, Is.EqualTo(41.246 / 100).Within(1e-3));
            Assert.That(cieXYZ.Y, Is.EqualTo(21.267 / 100).Within(1e-3));
            Assert.That(cieXYZ.Z, Is.EqualTo( 1.933 / 100).Within(1e-3));
        }

        Color actual = cieXYZ.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Green, Is.Zero      .Within(1e-3));
            Assert.That(actual.Blue , Is.Zero      .Within(1e-3));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_green_color_to_CieXYZ___OK()
    {
        Color rgb          = new(0, 1, 0);
        CieXYZColor cieXYZ = rgb.ToCieXYZ();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieXYZ.X, Is.EqualTo(35.758 / 100).Within(1e-3));
            Assert.That(cieXYZ.Y, Is.EqualTo(71.515 / 100).Within(1e-3));
            Assert.That(cieXYZ.Z, Is.EqualTo(11.919 / 100).Within(1e-3));
        }

        Color actual = cieXYZ.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero      .Within(1e-3));
            Assert.That(actual.Green, Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Blue , Is.Zero      .Within(1e-3));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_blue_color_to_CieXYZ___OK()
    {
        Color rgb          = new(0, 0, 1);
        CieXYZColor cieXYZ = rgb.ToCieXYZ();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieXYZ.X, Is.EqualTo(18.044 / 100).Within(1e-3));
            Assert.That(cieXYZ.Y, Is.EqualTo( 7.217 / 100).Within(1e-3));
            Assert.That(cieXYZ.Z, Is.EqualTo(95.030 / 100).Within(1e-3));
        }

        Color actual = cieXYZ.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero      .Within(1e-3));
            Assert.That(actual.Green, Is.Zero      .Within(1e-3));
            Assert.That(actual.Blue , Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_yellow_color_to_CieXYZ___OK()
    {
        Color rgb          = new(1, 1, 0);
        CieXYZColor cieXYZ = rgb.ToCieXYZ();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieXYZ.X, Is.EqualTo(77.003 / 100).Within(1e-3));
            Assert.That(cieXYZ.Y, Is.EqualTo(92.783 / 100).Within(1e-3));
            Assert.That(cieXYZ.Z, Is.EqualTo(13.853 / 100).Within(1e-3));
        }

        Color actual = cieXYZ.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Green, Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Blue , Is.Zero      .Within(1e-3));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_cyan_color_to_CieXYZ___OK()
    {
        Color rgb          = new(0, 1, 1);
        CieXYZColor cieXYZ = rgb.ToCieXYZ();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieXYZ.X, Is.EqualTo( 53.801 / 100).Within(1e-3));
            Assert.That(cieXYZ.Y, Is.EqualTo( 78.733 / 100).Within(1e-3));
            Assert.That(cieXYZ.Z, Is.EqualTo(106.950 / 100).Within(1e-3));
        }

        Color actual = cieXYZ.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero      .Within(1e-3));
            Assert.That(actual.Green, Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Blue , Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_magenta_color_to_CieXYZ___OK()
    {
        Color rgb          = new(1, 0, 1);
        CieXYZColor cieXYZ = rgb.ToCieXYZ();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieXYZ.X, Is.EqualTo(59.289 / 100).Within(1e-3));
            Assert.That(cieXYZ.Y, Is.EqualTo(28.485 / 100).Within(1e-3));
            Assert.That(cieXYZ.Z, Is.EqualTo(96.964 / 100).Within(1e-3));
        }

        Color actual = cieXYZ.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Green, Is.Zero      .Within(1e-3));
            Assert.That(actual.Blue , Is.EqualTo(1).Within(1e-3));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_to_CieXYZ_and_back()
    {
        double r = TestContext.CurrentContext.Random.NextDouble();
        double g = TestContext.CurrentContext.Random.NextDouble();
        double b = TestContext.CurrentContext.Random.NextDouble();

        Color rgb          = new(r, g, b);
        CieXYZColor cieXYZ = rgb.ToCieXYZ();
        Color actual       = cieXYZ.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(rgb.Red)  .Within(1e-3));
            Assert.That(actual.Green, Is.EqualTo(rgb.Green).Within(1e-3));
            Assert.That(actual.Blue , Is.EqualTo(rgb.Blue) .Within(1e-3));
            Assert.That(actual.Alpha, Is.EqualTo(rgb.Alpha));
        }
    }
}
