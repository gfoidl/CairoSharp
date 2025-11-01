// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Colors.ColorMaps.Default;

namespace CairoSharp.Extensions.Tests.Colors.ColorMaps;

[TestFixture]
public class ColorMapTests
{
    [Test]
    public void GetColor_value_0___OK()
    {
        BoneColorMap sut = new();

        Color actual = sut.GetColor(0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.Zero);
            Assert.That(actual.Green, Is.Zero);
            Assert.That(actual.Blue , Is.EqualTo(0.0013021));
        }
    }

    [Test]
    public void GetColor_value_1___OK()
    {
        BoneColorMap sut = new();

        Color actual = sut.GetColor(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1));
            Assert.That(actual.Green, Is.EqualTo(1));
            Assert.That(actual.Blue , Is.EqualTo(1));
        }
    }

    [Test]
    public void GetColorInverted_value_0___OK()
    {
        BoneColorMap sut = new();

        Color actual = sut.GetColorInverted(0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1));
            Assert.That(actual.Green, Is.EqualTo(1));
            Assert.That(actual.Blue , Is.EqualTo(1));
        }
    }

    [Test]
    public void GetColorInverted_value_1___OK()
    {
        BoneColorMap sut = new();

        Color actual = sut.GetColorInverted(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red, Is.Zero);
            Assert.That(actual.Green, Is.Zero);
            Assert.That(actual.Blue, Is.EqualTo(0.0013021));
        }
    }

    [Test]
    public void GetColor_value_0_5___OK()
    {
        AutumnColorMap sut = new();

        Color actual = sut.GetColor(0.5);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Red  , Is.EqualTo(1));
            Assert.That(actual.Green, Is.EqualTo(0.4980392156862745));
            Assert.That(actual.Blue , Is.Zero);
        }
    }
}
