// (c) gfoidl, all rights reserved

using static Cairo.Fonts.Quartz.QuartzFontNative;

namespace Cairo.Fonts.Quartz;

/// <summary>
/// Quartz (CGFont) Fonts — Font support via Core Text on Apple operating systems.
/// </summary>
/// <remarks>
/// Provide support for font faces via Core Text.
/// </remarks>
public sealed unsafe class QuartzFont : FontFace
{
    private QuartzFont(void* handle) : base(handle) { }

    /// <summary>
    /// Creates a new font for the Quartz font backend based on a CGFontRef.
    /// </summary>
    /// <param name="font">a CGFontRef obtained through a method external to cairo.</param>
    /// <remarks>
    /// This font can then be used with <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/> or
    /// <see cref="Scaled.ScaledFont.ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// </remarks>
    public static QuartzFont CreateForCgFont(IntPtr font)
    {
        void* handle = cairo_quartz_font_face_create_for_cgfont(font.ToPointer());
        return new QuartzFont(handle);
    }

    /// <summary>
    /// Creates a new font for the Quartz font backend based on an ATSUFontID.
    /// </summary>
    /// <param name="fontId">an ATSUFontID for the font.</param>
    /// <remarks>
    /// This font can then be used with <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/> or
    /// <see cref="Scaled.ScaledFont.ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// </remarks>
    public static QuartzFont CreateForAtsuFontId(uint fontId)
    {
        void* handle = cairo_quartz_font_face_create_for_atsu_font_id(fontId);
        return new QuartzFont(handle);
    }
}
