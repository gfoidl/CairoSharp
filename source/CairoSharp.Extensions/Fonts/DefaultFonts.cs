// (c) gfoidl, all rights reserved

#define USE_THREADSTATIC

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
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_sansSerif;
    public static FontFace SansSerif => t_sansSerif ??= new ToyFontFace("Helvetica");
#else
    public static FontFace SansSerif => s_sansSerif.Value;
    private static readonly Lazy<FontFace> s_sansSerif = new(
        () => new ToyFontFace("Helvetica"),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

    /// <summary>
    /// Helvetica (bold)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_sansSerifBold;
    public static FontFace SansSerifBold => t_sansSerifBold ??= new ToyFontFace("Helvetica", weight: Drawing.Text.FontWeight.Bold);
#else
    public static FontFace SansSerifBold => s_sansSerifBold.Value;
    private static readonly Lazy<FontFace> s_sansSerifBold = new(
        () => new ToyFontFace("Helvetica", weight: Drawing.Text.FontWeight.Bold),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

    /// <summary>
    /// Helvetica (italic)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_sansSerifItalic;
    public static FontFace SansSerifItalic => t_sansSerifItalic ??= new ToyFontFace("Helvetica", slant: Drawing.Text.FontSlant.Italic);
#else
    public static FontFace SansSerifItalic => s_sansSerifItalic.Value;
    private static readonly Lazy<FontFace> s_sansSerifItalic = new(
        () => new ToyFontFace("Helvetica", slant: Drawing.Text.FontSlant.Italic),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

    /// <summary>
    /// Helvetica (bold italic)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_sansSerifBoldItalic;
    public static FontFace SansSerifBoldItalic => t_sansSerifBoldItalic ??= new ToyFontFace("Helvetica", slant: Drawing.Text.FontSlant.Italic, weight: Drawing.Text.FontWeight.Bold);
#else
    public static FontFace SansSerifBoldItalic => s_sansSerifBoldItalic.Value;
    private static readonly Lazy<FontFace> s_sansSerifBoldItalic = new(
        () => new ToyFontFace("Helvetica", slant: Drawing.Text.FontSlant.Italic, weight: Drawing.Text.FontWeight.Bold),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

    /// <summary>
    /// DejaVu Serif
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for DejaVu Serif.
    /// </returns>
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_serif;
    public static FontFace Serif => t_serif ??= new ToyFontFace("DejaVu Serif");
#else
    public static FontFace Serif => s_serif.Value;
    private static readonly Lazy<FontFace> s_serif = new(
        () => new ToyFontFace("DejaVu Serif"),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

    /// <summary>
    /// DejaVu Serif (bold)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for DejaVu Serif.
    /// </returns>
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_serifBold;
    public static FontFace SerifBold => t_serifBold ??= new ToyFontFace("DejaVu Serif", weight: Drawing.Text.FontWeight.Bold);
#else
    public static FontFace SerifBold => s_serifBold.Value;
    private static readonly Lazy<FontFace> s_serifBold = new(
        () => new ToyFontFace("DejaVu Serif", weight: Drawing.Text.FontWeight.Bold),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

    /// <summary>
    /// DejaVu Serif (italic)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for DejaVu Serif.
    /// </returns>
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_serifItalic;
    public static FontFace SerifItalic => t_serifItalic ??= new ToyFontFace("DejaVu Serif", slant: Drawing.Text.FontSlant.Italic);
#else
    public static FontFace SerifItalic => s_serifItalic.Value;
    private static readonly Lazy<FontFace> s_serifItalic = new(
        () => new ToyFontFace("DejaVu Serif", slant: Drawing.Text.FontSlant.Italic),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

    /// <summary>
    /// DejaVu Serif (bold italic)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for DejaVu Serif.
    /// </returns>
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_serifBoldItalic;
    public static FontFace SerifBoldItalic => t_serifBoldItalic ??= new ToyFontFace("DejaVu Serif", slant: Drawing.Text.FontSlant.Italic, weight: Drawing.Text.FontWeight.Bold);
#else
    public static FontFace SerifBoldItalic => s_serifBoldItalic.Value;
    private static readonly Lazy<FontFace> s_serifBoldItalic = new(
        () => new ToyFontFace("DejaVu Serif", slant: Drawing.Text.FontSlant.Italic, weight: Drawing.Text.FontWeight.Bold),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

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
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_monoSpace;
    public static FontFace MonoSpace => t_monoSpace ??= FreeTypeFontLoader("fonts.SourceCodePro-Regular.ttf");
#else
    public static FontFace MonoSpace => s_monoSpace.Value;
    private static readonly Lazy<FontFace> s_monoSpace = new(
        () => FreeTypeFontLoader("fonts.SourceCodePro-Regular.ttf"),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

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
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_monoSpaceBold;
    public static FontFace MonoSpaceBold => t_monoSpaceBold ??= FreeTypeFontLoader("fonts.SourceCodePro-Bold.ttf");
#else
    public static FontFace MonoSpaceBold => s_monoSpaceBold.Value;
    private static readonly Lazy<FontFace> s_monoSpaceBold = new(
        () => FreeTypeFontLoader("fonts.SourceCodePro-Bold.ttf"),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

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
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_monoSpaceItalic;
    public static FontFace MonoSpaceItalic => t_monoSpaceItalic ??= FreeTypeFontLoader("fonts.SourceCodePro-Italic.ttf");
#else
    public static FontFace MonoSpaceItalic => s_monoSpaceItalic.Value;
    private static readonly Lazy<FontFace> s_monoSpaceItalic = new(
        () => FreeTypeFontLoader("fonts.SourceCodePro-Italic.ttf"),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif

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
#if USE_THREADSTATIC
    [ThreadStatic] private static FontFace? t_monoSpaceBoldItalic;
    public static FontFace MonoSpaceBoldItalic => t_monoSpaceBoldItalic ??= FreeTypeFontLoader("fonts.SourceCodePro-BoldItalic.ttf");
#else
    public static FontFace MonoSpaceBoldItalic => s_monoSpaceBoldItalic.Value;
    private static readonly Lazy<FontFace> s_monoSpaceBoldItalic = new(
        () => FreeTypeFontLoader("fonts.SourceCodePro-BoldItalic.ttf"),
        LazyThreadSafetyMode.ExecutionAndPublication);
#endif
    //-------------------------------------------------------------------------
    private static FreeTypeFont FreeTypeFontLoader(string resourceName)
    {
        using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
        return FreeTypeFont.LoadFromStream(fontStream);
    }
}
