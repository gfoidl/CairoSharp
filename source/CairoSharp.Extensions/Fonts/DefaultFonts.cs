// (c) gfoidl, all rights reserved

using System.IO;
using System.Reflection;
using Cairo.Fonts;
using Cairo.Fonts.FreeType;

namespace Cairo.Extensions.Fonts;

/// <summary>
/// Provides default <see cref="FontFace"/>s.
/// </summary>
public static class DefaultFonts
{
    /// <summary>
    /// Helvetica
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
    public static FontFace SansSerif => s_sansSerif.Value;
    private static readonly Lazy<FontFace> s_sansSerif = new(
        () => new ToyFontFace("Helvetica"),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Helvetica (bold)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
    public static FontFace SansSerifBold => s_sansSerifBold.Value;
    private static readonly Lazy<FontFace> s_sansSerifBold = new(
        () => new ToyFontFace("Helvetica", weight: Drawing.Text.FontWeight.Bold),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Helvetica (italic)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
    public static FontFace SansSerifItalic => s_sansSerifItalic.Value;
    private static readonly Lazy<FontFace> s_sansSerifItalic = new(
        () => new ToyFontFace("Helvetica", slant: Drawing.Text.FontSlant.Italic),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Helvetica (bold italic)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
    public static FontFace SansSerifBoldItalic => s_sansSerifBoldItalic.Value;
    private static readonly Lazy<FontFace> s_sansSerifBoldItalic = new(
        () => new ToyFontFace("Helvetica", slant: Drawing.Text.FontSlant.Italic, weight: Drawing.Text.FontWeight.Bold),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Source Code Pro
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Source Code Pro.
    /// </returns>
    /// <remarks>
    /// Source Code Pro is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpace => s_monoSpace.Value;
    private static readonly Lazy<FontFace> s_monoSpace = new(
        () =>
        {
            using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.SourceCodePro-Regular.ttf")!;
            return FreeTypeFont.LoadFromStream(fontStream);
        },
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Source Code Pro (bold)
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Source Code Pro.
    /// </returns>
    /// <remarks>
    /// Source Code Pro is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpaceBold => s_monoSpaceBold.Value;
    private static readonly Lazy<FontFace> s_monoSpaceBold = new(
        () =>
        {
            using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.SourceCodePro-Bold.ttf")!;
            return FreeTypeFont.LoadFromStream(fontStream);
        },
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Source Code Pro (italic)
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Source Code Pro.
    /// </returns>
    /// <remarks>
    /// Source Code Pro is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpaceItalic => s_monoSpaceItalic.Value;
    private static readonly Lazy<FontFace> s_monoSpaceItalic = new(
        () =>
        {
            using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.SourceCodePro-Italic.ttf")!;
            return FreeTypeFont.LoadFromStream(fontStream);
        },
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Source Code Pro (bold italic)
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Source Code Pro.
    /// </returns>
    /// <remarks>
    /// Source Code Pro is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpaceBoldItalic => s_monoSpaceBoldItalic.Value;
    private static readonly Lazy<FontFace> s_monoSpaceBoldItalic = new(
        () =>
        {
            using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.SourceCodePro-BoldItalic.ttf")!;
            return FreeTypeFont.LoadFromStream(fontStream);
        },
        LazyThreadSafetyMode.ExecutionAndPublication);
}
