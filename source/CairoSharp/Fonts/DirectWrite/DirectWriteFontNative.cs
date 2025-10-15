// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Fonts.DirectWrite;

// https://www.cairographics.org/manual/cairo-DWrite-Fonts.html

internal static unsafe partial class DirectWriteFontNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_dwrite_font_face_create_for_dwrite_fontface(void* dwrite_font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_dwrite_font_face_get_rendering_params(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_dwrite_font_face_set_rendering_params(cairo_font_face_t* font_face, void* @params);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial MeasuringMode cairo_dwrite_font_face_get_measuring_mode(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_dwrite_font_face_set_measuring_mode(cairo_font_face_t* font_face, MeasuringMode mode);
}
