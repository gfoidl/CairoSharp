// (c) gfoidl, all rights reserved

using Cairo.Fonts.Scaled;
using static Cairo.Fonts.FreeType.FreeTypeFontNative;

namespace Cairo.Fonts.FreeType;

/// <summary>
/// FreeType Fonts — Font support for FreeType
/// </summary>
/// <remarks>
/// The FreeType font backend is primarily used to render text on GNU/Linux systems,
/// but can be used on other platforms too.
/// </remarks>
public sealed unsafe class FreeTypeFont : FontFace
{
    private readonly FT_Face _freeTypeFace;

    internal FreeTypeFont(cairo_font_face_t* fontFace, bool isOwnedByCairo = false, bool needsDestroy = true)
        : base(fontFace, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Creates a new font face for the FreeType font backend from a pre-opened
    /// FreeType face.
    /// </summary>
    /// <param name="face">
    /// A FreeType face object, already opened. This must be kept around until the face's ref_count
    /// drops to zero and it is freed. Since the face may be referenced internally to Cairo, the best
    /// way to determine when it is safe to free the face is to pass a cairo_destroy_func_t to
    /// cairo_font_face_set_user_data()
    /// </param>
    /// <param name="loadFlags">
    /// flags to pass to <a href="https://freetype.org/freetype2/docs/reference/ft2-glyph_retrieval.html#ft_load_glyph">FT_Load_Glyph</a>
    /// when loading glyphs from the font. These flags are OR'ed together with the flags derived from the
    /// <see cref="FontOptions"/> to <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>,
    /// so only a few values such as FT_LOAD_VERTICAL_LAYOUT, and FT_LOAD_FORCE_AUTOHINT are useful.
    /// You should not pass any of the flags affecting the load target, such as FT_LOAD_TARGET_LIGHT.
    /// <para>
    /// See <a href="https://freetype.org/freetype2/docs/reference/ft2-glyph_retrieval.html#ft_load_xxx">FT_LOAD_XXX</a>
    /// for values.
    /// </para>
    /// </param>
    /// <remarks>
    /// This font can then be used with <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/> or
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// The <see cref="ScaledFont"/> returned from <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>
    /// is also for the FreeType backend and can be used with functions such as <see cref="LockFace"/>.
    /// Note that Cairo may keep a reference to the FT_Face alive in a font-cache and the exact lifetime
    /// of the reference depends highly upon the exact usage pattern and is subject to external factors.
    /// You must not call FT_Done_Face() before the last reference to the <see cref="FontFace"/> has been dropped.
    /// <para>
    /// See <a href="https://www.cairographics.org/manual/cairo-FreeType-Fonts.html#cairo-ft-font-face-create-for-ft-face">cairo docs</a>
    /// for an example on how one might correctly couple the lifetime of the FreeType face object to the <see cref="FontFace"/>.
    /// </para>
    /// </remarks>
    public FreeTypeFont(FT_Face face, int loadFlags) : base(cairo_ft_font_face_create_for_ft_face(face, loadFlags))
        => _freeTypeFace = face;

    /// <summary>
    /// Creates a new font face for the FreeType font backend from a pre-opened
    /// FreeType face.
    /// </summary>
    /// <param name="face">
    /// A FreeType face object, already opened. This must be kept around until the face's ref_count
    /// drops to zero and it is freed. Since the face may be referenced internally to Cairo, the best
    /// way to determine when it is safe to free the face is to pass a cairo_destroy_func_t to
    /// cairo_font_face_set_user_data()
    /// </param>
    /// <param name="loadFlags">
    /// flags to pass to <a href="https://freetype.org/freetype2/docs/reference/ft2-glyph_retrieval.html#ft_load_glyph">FT_Load_Glyph</a>
    /// when loading glyphs from the font. These flags are OR'ed together with the flags derived from the
    /// <see cref="FontOptions"/> to <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>,
    /// so only a few values such as FT_LOAD_VERTICAL_LAYOUT, and FT_LOAD_FORCE_AUTOHINT are useful.
    /// You should not pass any of the flags affecting the load target, such as FT_LOAD_TARGET_LIGHT.
    /// <para>
    /// See <a href="https://freetype.org/freetype2/docs/reference/ft2-glyph_retrieval.html#ft_load_xxx">FT_LOAD_XXX</a>
    /// for values.
    /// </para>
    /// </param>
    /// <remarks>
    /// This font can then be used with <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/> or
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// The <see cref="ScaledFont"/> returned from <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>
    /// is also for the FreeType backend and can be used with functions such as <see cref="LockFace"/>.
    /// Note that Cairo may keep a reference to the FT_Face alive in a font-cache and the exact lifetime
    /// of the reference depends highly upon the exact usage pattern and is subject to external factors.
    /// You must not call FT_Done_Face() before the last reference to the <see cref="FontFace"/> has been dropped.
    /// <para>
    /// See <a href="https://www.cairographics.org/manual/cairo-FreeType-Fonts.html#cairo-ft-font-face-create-for-ft-face">cairo docs</a>
    /// for an example on how one might correctly couple the lifetime of the FreeType face object to the <see cref="FontFace"/>.
    /// </para>
    /// </remarks>
    public FreeTypeFont(IntPtr face, int loadFlags) : this((FT_Face)face.ToPointer(), loadFlags) { }

    /// <summary>
    /// Creates a new font face for the FreeType font backend based on a fontconfig pattern.
    /// </summary>
    /// <param name="pattern">
    /// A fontconfig pattern. Cairo makes a copy of the pattern if it needs to. You are
    /// free to modify or free pattern after this call.
    /// </param>
    /// <remarks>
    /// This font can then be used with <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/> or
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>
    /// The <see cref="ScaledFont"/> returned from <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>
    /// is also for the FreeType backend and can be used with methods such as <see cref="LockFace"/>.
    /// <para>
    /// Font rendering options are represented both here and when you call <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// Font options that have a representation in a FcPattern must be passed in here; to modify FcPattern
    /// appropriately to reflect the options in a <see cref="FontOptions"/>, call <see cref="SubstituteOptions"/>.
    /// </para>
    /// <para>
    /// The pattern's FC_FT_FACE element is inspected first and if that is set, that will be the
    /// FreeType font face associated with the returned cairo font face. Otherwise the FC_FILE element
    /// is checked. If it's set, that and the value of the FC_INDEX element (defaults to zero) of
    /// pattern are used to load a font face from file.
    /// </para>
    /// <para>
    /// If both steps from the previous paragraph fails, pattern will be passed to FcConfigSubstitute,
    /// FcDefaultSubstitute, and finally FcFontMatch, and the resulting font pattern is used.
    /// </para>
    /// <para>
    /// If the FC_FT_FACE element of pattern is set, the user is responsible for making sure
    /// that the referenced FT_Face remains valid for the life time of the returned <see cref="FontFace"/>.
    /// See <see cref="FreeTypeFont(nint, int)"/> for an example of how to couple the life time of the
    /// FT_Face to that of the cairo font-face.
    /// </para>
    /// </remarks>
    public FreeTypeFont(IntPtr pattern) : base(cairo_ft_font_face_create_for_pattern((FcPattern*)pattern.ToPointer())) { }

    /// <summary>
    /// Add options to a FcPattern based on a <see cref="FontOptions"/> font options object.
    /// </summary>
    /// <param name="options">a <see cref="FontOptions"/> object</param>
    /// <param name="pattern">an existing FcPattern</param>
    /// <remarks>
    /// Options that are already in the pattern, are not overridden, so you should call this method
    /// after calling FcConfigSubstitute() (the user's settings should override options based on the
    /// surface type), but before calling FcDefaultSubstitute().
    /// </remarks>
    public static void SubstituteOptions(FontOptions options, IntPtr pattern)
    {
        cairo_ft_font_options_substitute(options.Handle, (FcPattern*)pattern.ToPointer());
    }

    /// <summary>
    /// Gets the FT_Face object from a FreeType backend font and scales it appropriately for the font
    /// and applies OpenType font variations if applicable.
    /// </summary>
    /// <param name="scaledFont">
    /// A <see cref="ScaledFont"/> from the FreeType font backend. Such an object can be created by calling
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/> on a FreeType backend font face
    /// (see <see cref="FreeTypeFont(nint, int)"/>, <see cref="FreeTypeFont(nint)"/>).
    /// </param>
    /// <returns>
    /// The FT_Face object for font, scaled appropriately, or <c>null</c> if <see cref="ScaledFont"/>
    /// is in an error state (see <see cref="ScaledFont.Status"/>) or there is insufficient memory.
    /// </returns>
    /// <remarks>
    /// You must release the face with <see cref="UnlockFace"/> when you are done using it.
    /// Since the FT_Face object can be shared between multiple <see cref="ScaledFont"/> objects, you
    /// must not lock any other font objects until you unlock this one. A count is kept of the number
    /// of times <see cref="LockFace"/> is called. <see cref="UnlockFace"/> must be called the same number of times.
    /// <para>
    /// You must be careful when using this method in a library or in a threaded application, because
    /// FreeType's design makes it unsafe to call FreeType functions simultaneously from multiple threads,
    /// (even if using distinct FT_Face objects). Because of this, application code that acquires an
    /// FT_Face object with this call must add its own locking to protect any use of that object, (and
    /// which also must protect any other calls into cairo as almost any cairo function might result in a
    /// call into the FreeType library).
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// <see cref="ScaledFont.FontType"/> is not <see cref="FontType.FreeType"/>
    /// </exception>
    public static FT_Face LockFace(ScaledFont scaledFont)
    {
        if (scaledFont.FontType != FontType.FreeType)
        {
            throw new InvalidOperationException($"LockFace can only be called on FreeType fonts. Actual font type = {scaledFont.FontType}");
        }

        return cairo_ft_scaled_font_lock_face((cairo_scaled_font_t*)scaledFont.Handle);
    }

    /// <summary>
    /// Releases a face obtained with <see cref="LockFace"/>.
    /// </summary>
    /// <param name="scaledFont">
    /// A <see cref="ScaledFont"/> from the FreeType font backend. Such an object can be created by calling
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/> on a FreeType backend font face
    /// (see <see cref="FreeTypeFont(nint, int)"/>, <see cref="FreeTypeFont(nint)"/>).
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="ScaledFont.FontType"/> is not <see cref="FontType.FreeType"/>
    /// </exception>
    public static void UnlockFace(ScaledFont scaledFont)
    {
        if (scaledFont.FontType != FontType.FreeType)
        {
            throw new InvalidOperationException($"UnlockFace can only be called on FreeType fonts. Actual font type = {scaledFont.FontType}");
        }

        cairo_ft_scaled_font_unlock_face((cairo_scaled_font_t*)scaledFont.Handle);
    }

    /// <summary>
    /// See <see cref="Synthesize"/>.
    /// </summary>
    /// <returns>the current set of synthesis options.</returns>
    public Synthesize GetSynthesize()
    {
        this.CheckDisposed();
        return cairo_ft_font_face_get_synthesize(this.Handle);
    }

    /// <summary>
    /// FreeType provides the ability to synthesize different glyphs from a base font, which is
    /// useful if you lack those glyphs from a true bold or oblique font.
    /// </summary>
    /// <param name="synthFlags">the set of synthesis options to enable</param>
    /// <returns>
    /// A <see cref="SynthesizeScope"/>, which when <see cref="SynthesizeScope.Dispose"/>d calls
    /// <see cref="UnsetSynthesize(Synthesize)"/>.
    /// <para>
    /// So instead of writing code like
    /// <code>
    /// ftFont.SetSynthesize(Synthesize.Bold);
    /// // ...
    /// ftFont.UnsetSynthesize(Synthesize.Bold);
    /// </code>
    /// one can write
    /// <code>
    /// using (ftFont.SetSynthesize(Synthesize.Bold))
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </para>
    /// </returns>
    public SynthesizeScope SetSynthesize(Synthesize synthFlags)
    {
        this.CheckDisposed();

        if (synthFlags != Synthesize.None)
        {
            cairo_ft_font_face_set_synthesize(this.Handle, synthFlags);

            return new SynthesizeScope(this, synthFlags);
        }
        else
        {
            // Synthesize.None -> unset the current flags
            Synthesize currentSynthFlags = cairo_ft_font_face_get_synthesize(this.Handle);
            cairo_ft_font_face_unset_synthesize(this.Handle, currentSynthFlags);

            return default;
        }
    }

    /// <summary>
    /// See <see cref="SetSynthesize(Synthesize)"/>.
    /// </summary>
    /// <param name="synthFlags">the set of synthesis options to disable</param>
    public void UnsetSynthesize(Synthesize synthFlags)
    {
        this.CheckDisposed();

        if (synthFlags == Synthesize.None)
        {
            return;
        }

        cairo_ft_font_face_unset_synthesize(this.Handle, synthFlags);
    }

    /// <summary>
    /// The FT_Face object for font
    /// </summary>
    public FT_Face FreeTypeFontFace => _freeTypeFace;
}
