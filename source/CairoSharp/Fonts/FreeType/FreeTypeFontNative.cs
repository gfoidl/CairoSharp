// (c) gfoidl, all rights reserved

global using unsafe FT_Face = Cairo.Fonts.FreeType.FT_FaceRec_*;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Cairo.Fonts.Scaled;

namespace Cairo.Fonts.FreeType;

// https://www.cairographics.org/manual/cairo-FreeType-Fonts.html

/// <summary>
/// FreeType's native face object.
/// </summary>
/// <remarks>
/// See <a href="https://freetype.org/freetype2/docs/reference/ft2-face_creation.html#ft_facerec">FreeType docs</a>
/// for the meaning of the fields.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
[EditorBrowsable(EditorBrowsableState.Never)]
public unsafe struct FT_FaceRec_
{
    public CLong  num_faces;
    public CLong  face_index;
    public CLong  face_flags;
    public CLong  style_flags;
    public CLong  num_glyphs;
    public sbyte* family_name;
    public sbyte* style_name;
    // others left out
}

internal struct FcPattern;

internal static unsafe partial class FreeTypeFontNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_font_face_t* cairo_ft_font_face_create_for_ft_face(FT_Face face, int load_flags);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_font_face_t* cairo_ft_font_face_create_for_pattern(FcPattern* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_ft_font_options_substitute(cairo_font_options_t* options, FcPattern* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FT_Face cairo_ft_scaled_font_lock_face(cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_ft_scaled_font_unlock_face(cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial Synthesize cairo_ft_font_face_get_synthesize(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_ft_font_face_set_synthesize(cairo_font_face_t* font_face, Synthesize synth_flags);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_ft_font_face_unset_synthesize(cairo_font_face_t* font_face, Synthesize synth_flags);
}
