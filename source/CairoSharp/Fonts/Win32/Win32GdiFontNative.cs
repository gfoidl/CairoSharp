// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using unsafe LOGFONTW = void*;
using unsafe HFONT    = void*;
using unsafe HDC      = void*;

namespace Cairo.Fonts.Win32;

// https://www.cairographics.org/manual/cairo-Win32-GDI-Fonts.html

internal static unsafe partial class Win32GdiFontNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_win32_font_face_create_for_logfontw(LOGFONTW logfont);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_win32_font_face_create_for_hfont(HFONT font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_win32_font_face_create_for_logfontw_hfont(LOGFONTW logfont, HFONT font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_win32_scaled_font_select_font(void* scaled_font, HDC hdc);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_win32_scaled_font_done_font(void* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial double cairo_win32_scaled_font_get_metrics_factor(void* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_win32_scaled_font_get_logical_to_device(void* scaled_font, out Matrix logical_to_device);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_win32_scaled_font_get_device_to_logical(void* scaled_font, out Matrix device_to_logical);
}
