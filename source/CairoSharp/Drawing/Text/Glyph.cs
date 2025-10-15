// (c) gfoidl, all rights reserved

using static Cairo.Drawing.Text.TextNative;

using System.Runtime.InteropServices;

namespace Cairo.Drawing.Text;

/// <summary>
/// The <see cref="Glyph"/> structure holds information about a single glyph when drawing or
/// measuring text. A font is (in simple terms) a collection of shapes used to draw text.
/// A glyph is one of these shapes. There can be multiple glyphs for a single character (alternates
/// to be used in different contexts, for example), or a glyph can be a ligature of multiple characters.
/// Cairo doesn't expose any way of converting input text into glyphs, so in order to use the
/// Cairo interfaces that take arrays of glyphs, you must directly access the appropriate
/// underlying font system.
/// </summary>
/// <param name="Index">
/// glyph index in the font. The exact interpretation of the glyph index depends on the font technology being used.
/// </param>
/// <param name="X">
/// the offset in the X direction between the origin used for drawing or measuring the string and the origin of this glyph.
/// </param>
/// <param name="Y">
/// the offset in the Y direction between the origin used for drawing or measuring the string and the origin of this glyph.
/// </param>
/// <remarks>
/// Note that the offsets given by x and y are not cumulative. When drawing or measuring text, each glyph
/// is individually positioned with respect to the overall origin
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Glyph(CULong Index, double X, double Y)
{
    /// <summary>
    /// Allocates an array of <see cref="Glyph"/>'s. This method is only useful in implementations
    /// of cairo_user_scaled_font_text_to_glyphs_func_t where the user needs to allocate an array of glyphs
    /// that cairo will free. For all other uses, user can use their own allocation method for glyphs.
    /// </summary>
    /// <param name="numberOfGlyphs">number of glyphs to allocate</param>
    /// <returns> the newly allocated array of glyphs that should be freed using <see cref="CairoObject.Dispose()"/></returns>
    /// <remarks>
    /// This method returns <c>null</c> if <paramref name="numberOfGlyphs"/> is not positive, or if out of memory.
    /// That means, the <c>null</c> return value signals out-of-memory only if <paramref name="numberOfGlyphs"/> was positive.
    /// </remarks>
    public static unsafe GlyphArray Allocate(int numberOfGlyphs)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfGlyphs);

        Glyph* glyphs = cairo_glyph_allocate(numberOfGlyphs);

        if (glyphs is null)
        {
            CairoException.ThrowOutOfMemory();
        }

        return new GlyphArray(glyphs, numberOfGlyphs);
    }
}

/// <summary>
/// A <see cref="Glyph"/>-array.
/// </summary>
public sealed unsafe class GlyphArray : CairoObject<Glyph>
{
    private readonly int _numberOfGlyphs;

    internal GlyphArray(Glyph* glyphs, int numberOfGlyphs)
        : base(glyphs)
        => _numberOfGlyphs = numberOfGlyphs;

    protected override void DisposeCore(Glyph* glyph) => cairo_glyph_free(glyph);

    /// <summary>
    /// The span representation of the <see cref="Glyph"/>s.
    /// </summary>
    public ReadOnlySpan<Glyph> Span => new(this.Handle, _numberOfGlyphs);
}
