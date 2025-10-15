// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using Cairo.Fonts.Scaled;
using unsafe FT_Face = void*;

namespace Cairo.Fonts.FreeType;

// https://www.cairographics.org/manual/cairo-FreeType-Fonts.html

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
    internal static partial uint cairo_ft_font_face_get_synthesize(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_ft_font_face_set_synthesize(cairo_font_face_t* font_face, uint synth_flags);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_ft_font_face_unset_synthesize(cairo_font_face_t* font_face, uint synth_flags);
}
