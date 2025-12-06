// (c) gfoidl, all rights reserved

using static Cairo.Extensions.Pango.FontFamilyNative;

namespace Cairo.Extensions.Pango;

/// <summary>
/// A <see cref="FontFamily"/> is used to represent a family of related font faces.
/// </summary>
/// <remarks>
/// The font faces in a family share a common design, but differ in slant, weight, width
/// or other aspects.
/// </remarks>
public sealed unsafe class FontFamily : CairoObject<pango_font_family>
{
    internal FontFamily(pango_font_family* family) : base(family, isOwnedByCairo: true, needsDestroy: false) { }

    protected override void DisposeCore(pango_font_family* handle)
        => throw new InvalidOperationException("PangoFontFamily must not be freed");

    /// <summary>
    /// Gets the name of the family.
    /// </summary>
    /// <remarks>
    /// The name is unique among all fonts for the font backend and can be used in a
    /// PangoFontDescription to specify that a face from this family is desired.
    /// </remarks>
    public string Name
    {
        get
        {
            this.CheckDisposed();
            return pango_font_family_get_name(this.Handle);
        }
    }

    /// <summary>
    /// A monospace font is a font designed for text display where the characters form a regular grid.
    /// </summary>
    /// <remarks>
    /// For Western languages this would mean that the advance width of all characters are the same, but
    /// this categorization also includes Asian fonts which include double-width characters: characters
    /// that occupy two grid cells. g_unichar_iswide() returns a result that indicates whether a character
    /// is typically double-width in a monospace font.
    /// </remarks>
    public bool IsMonospace
    {
        get
        {
            this.CheckDisposed();
            return pango_font_family_is_monospace(this.Handle);
        }
    }

    /// <summary>
    /// A variable font is a font which has axes that can be modified to produce different faces.
    /// </summary>
    /// <remarks>
    /// Such axes are also known as variations.
    /// </remarks>
    public bool IsVariable
    {
        get
        {
            this.CheckDisposed();
            return pango_font_family_is_variable(this.Handle);
        }
    }

    public override string ToString() => $"""
        Name:        {this.Name}
        IsMonospace: {this.IsMonospace}
        IsVariable:  {this.IsVariable}
        """;
}
