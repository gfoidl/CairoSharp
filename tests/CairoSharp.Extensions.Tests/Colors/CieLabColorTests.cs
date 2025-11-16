// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Colors;

namespace CairoSharp.Extensions.Tests.Colors;

// https://www.easyrgb.com/en/convert.php#inputFORM
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
            Assert.That(cieLab.L, Is.EqualTo(53.241).Within(1e-2));
            Assert.That(cieLab.A, Is.EqualTo(80.092).Within(1e-2));
            Assert.That(cieLab.B, Is.EqualTo(67.203).Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Green, Is.Zero      .Within(1e-2));
            Assert.That(actual.Blue , Is.Zero      .Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_green_color_to_CieLab___OK()
    {
        Color rgb          = new(0, 1, 0);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.EqualTo( 87.735).Within(1e-2));
            Assert.That(cieLab.A, Is.EqualTo(-86.183).Within(1e-2));
            Assert.That(cieLab.B, Is.EqualTo( 83.179).Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero      .Within(1e-2));
            Assert.That(actual.Green, Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Blue , Is.Zero      .Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_blue_color_to_CieLab___OK()
    {
        Color rgb          = new(0, 0, 1);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.EqualTo(  32.297).Within(1e-2));
            Assert.That(cieLab.A, Is.EqualTo(  79.188).Within(1e-2));
            Assert.That(cieLab.B, Is.EqualTo(-107.860).Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero      .Within(1e-2));
            Assert.That(actual.Green, Is.Zero      .Within(1e-2));
            Assert.That(actual.Blue , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_yellow_color_to_CieLab___OK()
    {
        Color rgb          = new(1, 1, 0);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.EqualTo( 97.139).Within(1e-2));
            Assert.That(cieLab.A, Is.EqualTo(-21.554).Within(1e-2));
            Assert.That(cieLab.B, Is.EqualTo( 94.478).Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Green, Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Blue , Is.Zero      .Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_cyan_color_to_CieLab___OK()
    {
        Color rgb          = new(0, 1, 1);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.EqualTo( 91.113).Within(1e-2));
            Assert.That(cieLab.A, Is.EqualTo(-48.088).Within(1e-2));
            Assert.That(cieLab.B, Is.EqualTo(-14.131).Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero      .Within(1e-2));
            Assert.That(actual.Green, Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Blue , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
        }
    }

    [Test]
    public void RGB_magenta_color_to_CieLab___OK()
    {
        Color rgb          = new(1, 0, 1);
        CieLabColor cieLab = rgb.ToCieLab();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cieLab.L, Is.EqualTo( 60.324).Within(1e-2));
            Assert.That(cieLab.A, Is.EqualTo( 98.234).Within(1e-2));
            Assert.That(cieLab.B, Is.EqualTo(-60.825).Within(1e-2));
        }

        Color actual = cieLab.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Green, Is.Zero      .Within(1e-2));
            Assert.That(actual.Blue , Is.EqualTo(1).Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1).Within(1e-5));
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

    [Test]
    public void ColorDistance_with_same_color___0()
    {
        double r = TestContext.CurrentContext.Random.NextDouble();
        double g = TestContext.CurrentContext.Random.NextDouble();
        double b = TestContext.CurrentContext.Random.NextDouble();

        Color rgb           = new(r, g, b);
        CieLabColor cieLab0 = rgb.ToCieLab();
        CieLabColor cieLab1 = rgb.ToCieLab();

        double actual = cieLab0.ColorDistance(cieLab1);

        Assert.That(actual, Is.Zero);
    }

    [Test]
    public void ColorDistance___OK()
    {
        CieLabColor c0 = Color.FromRgbBytes(100, 100, 100).ToCieLab();
        CieLabColor c1 = Color.FromRgbBytes(120, 120, 120).ToCieLab();

        double actual = c0.ColorDistance(c1);

        // https://colormine.org/delta-e-calculator
        Assert.That(actual, Is.EqualTo(8.0567).Within(1e-3));
    }
}
