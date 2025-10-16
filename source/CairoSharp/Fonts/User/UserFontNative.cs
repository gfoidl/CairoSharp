// (c) gfoidl, all rights reserved

global using unsafe cairo_user_scaled_font_init_func_t             = delegate*<Cairo.Fonts.Scaled.cairo_scaled_font_t*, Cairo.cairo_t*, ref Cairo.Fonts.FontExtents, Cairo.Status>;
global using unsafe cairo_user_scaled_font_render_glyph_func_t     = delegate*<Cairo.Fonts.Scaled.cairo_scaled_font_t*, System.Runtime.InteropServices.CULong, Cairo.cairo_t*, ref Cairo.Fonts.TextExtents, Cairo.Status>;
global using unsafe cairo_user_scaled_font_text_to_glyphs_func_t   = delegate*<Cairo.Fonts.Scaled.cairo_scaled_font_t*, byte*, int, Cairo.Drawing.Text.Glyph**, ref int, Cairo.Drawing.Text.TextCluster**, ref int, out Cairo.Drawing.Text.ClusterFlags, Cairo.Status>;
global using unsafe cairo_user_scaled_font_unicode_to_glyph_func_t = delegate*<Cairo.Fonts.Scaled.cairo_scaled_font_t*, System.Runtime.InteropServices.CULong, out System.Runtime.InteropServices.CULong, Cairo.Status>;

using System.Runtime.InteropServices;
using Cairo.Drawing.Patterns;
using Cairo.Fonts.Scaled;

namespace Cairo.Fonts.User;

// https://www.cairographics.org/manual/cairo-User-Fonts.html

internal static unsafe partial class UserFontNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_font_face_t* cairo_user_font_face_create();

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_user_font_face_set_init_func(cairo_font_face_t* font_face, cairo_user_scaled_font_init_func_t init_func);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_user_scaled_font_init_func_t cairo_user_font_face_get_init_func(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_user_font_face_set_render_glyph_func(cairo_font_face_t* font_face, cairo_user_scaled_font_render_glyph_func_t render_glyph_func);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_user_scaled_font_render_glyph_func_t cairo_user_font_face_get_render_glyph_func(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_user_font_face_set_render_color_glyph_func(cairo_font_face_t* font_face, cairo_user_scaled_font_render_glyph_func_t render_glyph_func);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_user_scaled_font_render_glyph_func_t cairo_user_font_face_get_render_color_glyph_func(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_user_font_face_set_unicode_to_glyph_func(cairo_font_face_t* font_face, cairo_user_scaled_font_unicode_to_glyph_func_t unicode_to_glyph_func);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_user_scaled_font_unicode_to_glyph_func_t cairo_user_font_face_get_unicode_to_glyph_func(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_user_font_face_set_text_to_glyphs_func(cairo_font_face_t* font_face, cairo_user_scaled_font_text_to_glyphs_func_t text_to_glyphs_func);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_user_scaled_font_text_to_glyphs_func_t cairo_user_font_face_get_text_to_glyphs_func(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_user_scaled_font_get_foreground_marker(cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_user_scaled_font_get_foreground_source(cairo_scaled_font_t* scaled_font);
}
