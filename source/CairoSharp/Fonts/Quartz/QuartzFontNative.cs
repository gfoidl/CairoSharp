// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using unsafe CGFontRef = void*;
using ATSUFontID       = uint;

namespace Cairo.Fonts.Quartz;

// https://www.cairographics.org/manual/cairo-Quartz-(CGFont)-Fonts.html

internal static unsafe partial class QuartzFontNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_font_face_t* cairo_quartz_font_face_create_for_cgfont(CGFontRef font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_font_face_t* cairo_quartz_font_face_create_for_atsu_font_id(ATSUFontID font_id);
}
