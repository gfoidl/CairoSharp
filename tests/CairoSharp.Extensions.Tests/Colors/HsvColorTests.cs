// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Colors;

namespace CairoSharp.Extensions.Tests.Colors;

[TestFixture]
public class HsvColorTests
{
    [Test]
    public void RGB_black_color_to_HSV___OK()
    {
        Color rgb    = new(0, 0, 0);
        HsvColor hsv = rgb.ToHSV();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hsv.Hue       , Is.Zero);
            Assert.That(hsv.Saturation, Is.Zero);
            Assert.That(hsv.Value     , Is.Zero);
        }

        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero);
            Assert.That(actual.Green, Is.Zero);
            Assert.That(actual.Blue , Is.Zero);
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_white_color_to_HSV___OK()
    {
        Color rgb    = new(1, 1, 1);
        HsvColor hsv = rgb.ToHSV();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hsv.Hue       , Is.Zero);
            Assert.That(hsv.Saturation, Is.Zero);
            Assert.That(hsv.Value     , Is.EqualTo(1));
        }

        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1));
            Assert.That(actual.Green, Is.EqualTo(1));
            Assert.That(actual.Blue , Is.EqualTo(1));
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_red_color_to_HSV___OK()
    {
        Color rgb    = new(1, 0, 0);
        HsvColor hsv = rgb.ToHSV();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hsv.Hue       , Is.Zero);
            Assert.That(hsv.Saturation, Is.EqualTo(1));
            Assert.That(hsv.Value     , Is.EqualTo(1));
        }

        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1));
            Assert.That(actual.Green, Is.Zero);
            Assert.That(actual.Blue , Is.Zero);
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_green_color_to_HSV___OK()
    {
        Color rgb = new(0, 1, 0);
        HsvColor hsv = rgb.ToHSV();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hsv.Hue       , Is.EqualTo(120));
            Assert.That(hsv.Saturation, Is.EqualTo(1));
            Assert.That(hsv.Value     , Is.EqualTo(1));
        }

        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero);
            Assert.That(actual.Green, Is.EqualTo(1));
            Assert.That(actual.Blue , Is.Zero);
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_blue_color_to_HSV___OK()
    {
        Color rgb    = new(0, 0, 1);
        HsvColor hsv = rgb.ToHSV();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hsv.Hue       , Is.EqualTo(240));
            Assert.That(hsv.Saturation, Is.EqualTo(1));
            Assert.That(hsv.Value     , Is.EqualTo(1));
        }

        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero);
            Assert.That(actual.Green, Is.Zero);
            Assert.That(actual.Blue , Is.EqualTo(1));
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_yellow_color_to_HSV___OK()
    {
        Color rgb    = new(1, 1, 0);
        HsvColor hsv = rgb.ToHSV();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hsv.Hue       , Is.EqualTo(60));
            Assert.That(hsv.Saturation, Is.EqualTo(1));
            Assert.That(hsv.Value     , Is.EqualTo(1));
        }

        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1));
            Assert.That(actual.Green, Is.EqualTo(1));
            Assert.That(actual.Blue , Is.Zero);
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_cyan_color_to_HSV___OK()
    {
        Color rgb    = new(0, 1, 1);
        HsvColor hsv = rgb.ToHSV();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hsv.Hue       , Is.EqualTo(180));
            Assert.That(hsv.Saturation, Is.EqualTo(1));
            Assert.That(hsv.Value     , Is.EqualTo(1));
        }

        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero);
            Assert.That(actual.Green, Is.EqualTo(1));
            Assert.That(actual.Blue , Is.EqualTo(1));
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_magenta_color_to_HSV___OK()
    {
        Color rgb    = new(1, 0, 1);
        HsvColor hsv = rgb.ToHSV();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hsv.Hue       , Is.EqualTo(300));
            Assert.That(hsv.Saturation, Is.EqualTo(1));
            Assert.That(hsv.Value     , Is.EqualTo(1));
        }

        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1));
            Assert.That(actual.Green, Is.Zero);
            Assert.That(actual.Blue , Is.EqualTo(1));
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_87_93_171_color_to_HSV___OK()
    {
        Color rgb    = Color.FromRgbBytes(87, 93, 171);
        HsvColor hsv = rgb.ToHSV();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hsv.Hue       , Is.EqualTo(235.71)   .Within(1e-2));
            Assert.That(hsv.Saturation, Is.EqualTo(  0.49123).Within(1e-2));
            Assert.That(hsv.Value     , Is.EqualTo(  0.67059).Within(1e-2));
        }

        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo( 87 / 255d).Within(1e-2));
            Assert.That(actual.Green, Is.EqualTo( 93 / 255d).Within(1e-2));
            Assert.That(actual.Blue , Is.EqualTo(171 / 255d).Within(1e-2));
            Assert.That(actual.Alpha, Is.EqualTo(1));
        }
    }

    [Test]
    public void RGB_to_HSV_and_back()
    {
        double r = TestContext.CurrentContext.Random.NextDouble();
        double g = TestContext.CurrentContext.Random.NextDouble();
        double b = TestContext.CurrentContext.Random.NextDouble();

        Color rgb    = new(r, g, b);
        HsvColor hsv = rgb.ToHSV();
        Color actual = hsv.ToRGB();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(rgb.Red)  .Within(1e-5));
            Assert.That(actual.Green, Is.EqualTo(rgb.Green).Within(1e-5));
            Assert.That(actual.Blue , Is.EqualTo(rgb.Blue) .Within(1e-5));
            Assert.That(actual.Alpha, Is.EqualTo(rgb.Alpha));
        }
    }
}
