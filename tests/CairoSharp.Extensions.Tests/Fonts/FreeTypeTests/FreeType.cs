// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Fonts.FreeType;
using Cairo.Fonts;
using Cairo.Fonts.FreeType;
using Cairo.Fonts.Scaled;
using Cairo.Surfaces.SVG;

namespace CairoSharp.Extensions.Tests.Fonts.FreeTypeTests;

[TestFixture]
public class FreeType
{
    [Test, Explicit("Testhost will crash")]
    public void DoneFreeType_then_assign_a_font_and_draw___crashes_test_host()
    {
        using FreeTypeFont sanRemoFont = Helper.LoadFreeTypeFontFromFile("SanRemo.ttf");
        FreeTypeFont.DoneFreeType();

        using SvgSurface svg  = new(500, 500);
        using CairoContext cr = new(svg);

        cr.FontFace = sanRemoFont;

        cr.MoveTo(10, 100);
        cr.ShowText("Test"u8);
    }
    //-------------------------------------------------------------------------
    [Test]
    public unsafe void Face_info_via_non_standard_property___OK()
    {
        using FreeTypeFont sanRemoFont = Helper.LoadFreeTypeFontFromFile("SanRemo.ttf");
        FT_FaceRec_* face              = sanRemoFont.FreeTypeFontFace;

        using (Assert.EnterMultipleScope())
        {
            int facesCount    = (int)(face->num_faces).Value;
            string familyName = new  (face->family_name);
            string styleName  = new  (face->style_name);

            Assert.That(facesCount, Is.EqualTo(1));
            Assert.That(familyName, Is.EqualTo("San Remo"));
            Assert.That(styleName , Is.EqualTo("Regular"));
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public unsafe void Face_info_via_LockFace___OK()
    {
        Matrix fontMatrix = default;
        Matrix ctm        = default;
        fontMatrix.InitIdentity();
        ctm.InitIdentity();

        using FontOptions fontOptions  = new();
        using FreeTypeFont sanRemoFont = Helper.LoadFreeTypeFontFromFile("SanRemo.ttf");
        using ScaledFont scaledFont    = new(sanRemoFont, ref fontMatrix, ref ctm, fontOptions);
        FT_FaceRec_* face              = FreeTypeFont.LockFace(scaledFont);

        var type = scaledFont.FontType;

        using (Assert.EnterMultipleScope())
        {
            int facesCount    = (int)(face->num_faces).Value;
            string familyName = new  (face->family_name);
            string styleName  = new  (face->style_name);

            Assert.That(facesCount, Is.EqualTo(1));
            Assert.That(familyName, Is.EqualTo("San Remo"));
            Assert.That(styleName , Is.EqualTo("Regular"));
        }

        FreeTypeFont.UnlockFace(scaledFont);
    }
    //-------------------------------------------------------------------------
    [Test]
    public unsafe void LockFace_on_non_FreeType_font___throws_InvalidOperation()
    {
        Matrix fontMatrix = default;
        Matrix ctm        = default;
        fontMatrix.InitIdentity();
        ctm.InitIdentity();

        using FontOptions fontOptions = new();
        using ToyFontFace fontFace    = new("Arial");
        using ScaledFont scaledFont   = new(fontFace, ref fontMatrix, ref ctm, fontOptions);

        InvalidOperationException actual = Assert.Throws<InvalidOperationException>(() => FreeTypeFont.LockFace(scaledFont));

        if (OperatingSystem.IsWindows())
        {
            Assert.That(actual.Message, Is.EqualTo("LockFace can only be called on FreeType fonts. Actual font type = Dwrite"));
        }
    }
}
