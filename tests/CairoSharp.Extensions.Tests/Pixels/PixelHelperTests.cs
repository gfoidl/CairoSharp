// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Pixels;

namespace CairoSharp.Extensions.Tests.Pixels;

[TestFixture]
public class PixelHelperTests
{
    [Test]
    public void SetColor_GetColor_alpha_is_1___OK()
    {
        Color color     = new(0.85, 0.12, 0.23);
        Span<byte> data = stackalloc byte[4];

        PixelHelper.SetColor(data, 0, color);
        Color actual = PixelHelper.GetColor(data, 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Alpha, Is.EqualTo(1));
            Assert.That(actual.Red  , Is.EqualTo(color.Red)  .Within(1e-2));
            Assert.That(actual.Green, Is.EqualTo(color.Green).Within(1e-2));
            Assert.That(actual.Blue , Is.EqualTo(color.Blue) .Within(1e-2));
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public void SetColor_GetColor_alpha_set___OK()
    {
        Color color     = new(0.85, 0.12, 0.23, 0.43);
        Span<byte> data = stackalloc byte[4];

        PixelHelper.SetColor(data, 0, color);
        Color actual = PixelHelper.GetColor(data, 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Alpha, Is.EqualTo(color.Alpha).Within(1e-2));
            Assert.That(actual.Red  , Is.EqualTo(color.Red)  .Within(1e-2));
            Assert.That(actual.Green, Is.EqualTo(color.Green).Within(1e-2));
            Assert.That(actual.Blue , Is.EqualTo(color.Blue) .Within(1e-2));
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public void SetColor_index_out_of_range___throws_ArgumentOutOfRange([Values(1, 2, 3, 4, 5)] int idx)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PixelHelper.SetColor(stackalloc byte[4], idx, Color.Default));
    }
    //-------------------------------------------------------------------------
    [Test]
    public void GetColor_index_out_of_range___throws_ArgumentOutOfRange([Values(1, 2, 3, 4, 5)] int idx)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PixelHelper.GetColor(stackalloc byte[4], idx));
    }
}
