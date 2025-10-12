// (c) gfoidl, all rights reserved

using Cairo.Extensions.Fonts.FreeType;
using Cairo.Fonts.FreeType;
using static Cairo.Extensions.Fonts.FreeType.FreeTypeNative;

#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace Cairo.Fonts.FreeType;

/// <summary>
/// Extensions for <see cref="FreeTypeFont"/>.
/// </summary>
public static unsafe class FreeTypeExtensions
{
#if NET9_0_OR_GREATER
    private static readonly Lock s_freeTypSyncRoot = new();
#else
    private static readonly object s_freeTypSyncRoot = new();
#endif

    private static UserDataKey s_destroyFuncKey;
    private static FT_Library  s_ftLibrary;

    private static FT_Library EnsureFreeTypeInitialized()
    {
        return s_ftLibrary is not null ? s_ftLibrary : Core();

        static FT_Library Core()
        {
            lock (s_freeTypSyncRoot)
            {
                if (s_ftLibrary is not null)
                {
                    return s_ftLibrary;
                }

                FT_Library library;
                FTError status = FT_Init_FreeType(&library);

                status.ThrowIfNotSuccess();

                return s_ftLibrary = library;
            }
        }
    }

    private static FreeTypeFont CreateFreeTypeFontCore(FT_Face face, int loadFlags)
    {
        // Problem: cairo doesn't know to call FT_Done_Face when its font_face object is
        // destroyed, so we have to do that for it, by attaching a cleanup callback to
        // the font_face. This only needs to be done once for each font face, while
        // cairo_ft_font_face_create_for_ft_face will return the same font_face if called
        // twice with the same FT face.
        // The following check for whether the cleanpu has been attached or not is
        // actually unnecessary in out situation, because each call to FT_New_Face
        // will return a new FT Face, but we include it here to show how to handle the
        // general case.
        // See: 
        // https://www.cairographics.org/manual/cairo-FreeType-Fonts.html#cairo-ft-font-face-create-for-ft-face
        // https://www.cairographics.org/cookbook/freetypepython/

        FreeTypeFont ftFont = new((nint)face, loadFlags);

        void* userData = ftFont.GetUserData(ref s_destroyFuncKey);

        if (userData is null)
        {
            ftFont.SetUserData(ref s_destroyFuncKey, face, &DestroyFunc);
        }

        return ftFont;

        static void DestroyFunc(void* userData)
        {
            FT_Face face = (FT_Face)userData;
            FTError status = FT_Done_Face(face);
            status.ThrowIfNotSuccess();
        }
    }

    extension(FreeTypeFont)
    {
        /// <summary>
        /// Destroy a given FreeType library object and all of its children, including resources,
        /// drivers, faces, sizes, etc.
        /// </summary>
        /// <remarks>
        /// Normally this isn't needed as the resources are freed up on app shutdown automatically.
        /// But this method can be used, when FreeType is no longer used in the app.
        /// <para>
        /// If called, and then later FreeTyped is used again, a new FreeType library will be created.
        /// </para>
        /// </remarks>
        public static void DoneFreeType()
        {
            lock (s_freeTypSyncRoot)
            {
                if (s_ftLibrary is null)
                {
                    return;
                }

                FTError status = FT_Done_FreeType(s_ftLibrary);
                s_ftLibrary    = null;

                status.ThrowIfNotSuccess();
            }
        }

        /// <summary>
        /// Gets the version of the native FreeType library.
        /// </summary>
        /// <returns>version of the native FreeType library</returns>
        public static string FreeTypeLibVersion()
        {
            FT_Library library = EnsureFreeTypeInitialized();

            FT_Library_Version(library, out int major, out int minor, out int patch);
            return $"{major}.{minor}.{patch}";
        }

        /// <summary>
        /// Loads the font given by <paramref name="fontFileName"/> and <paramref name="faceIndex"/>.
        /// </summary>
        /// <param name="fontFileName">A path to the font file</param>
        /// <param name="faceIndex">
        /// This field holds two different values. Bits 0-15 are the index of the face in the font file
        /// (starting with value 0). Set it to 0 if there is only one face in the font file.
        /// <para>
        /// Bits 16-30 are relevant to TrueType GX and OpenType Font Variations only, specifying the named
        /// instance index for the current face index (starting with value 1; value 0 makes FreeType ignore
        /// named instances). For non-variation fonts, bits 16-30 are ignored. Assuming that you want to access
        /// the third named instance in face 4, <paramref name="faceIndex"/> should be set to 0x00030004. If you
        /// want to access face 4 without variation handling, simply set face_index to value 4.
        /// </para>
        /// </param>
        /// <param name="loadFlags">See <see cref="FreeTypeFont(nint, int)"/> param <c>loadFlags</c></param>
        /// <returns>A <see cref="FreeTypeFont"/></returns>
        /// <remarks>
        /// The pathname string should be recognizable as such by a standard <c>fopen</c> call on your system;
        /// in particular, this means that pathname must not contain null bytes. If that is not sufficient to
        /// address all file name possibilities you might use <see cref="LoadFromStream(Stream, int)"/>
        /// to pass a stream object instead.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="fontFileName"/> is <c>null</c></exception>
        public static FreeTypeFont LoadFromFile(string fontFileName, int faceIndex = 0, int loadFlags = 0)
        {
            ArgumentNullException.ThrowIfNull(fontFileName);

            FT_Library library = EnsureFreeTypeInitialized();

            FT_Face face;
            FTError status = FT_New_Face(library, fontFileName, new FT_Long(faceIndex), &face);

            status.ThrowIfNotSuccess();
            return CreateFreeTypeFontCore(face, loadFlags);
        }

        // TODO: implement loading from a stream, e.g. .NET's resource stream when a font is
        // embedded as resource.
        // Cf. https://freetype.org/freetype2/docs/reference/ft2-face_creation.html#ft_open_face
        internal static FreeTypeFont LoadFromStream(Stream stream, int faceIndex)
        {
            throw new NotImplementedException();
        }
    }
}
