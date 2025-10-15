// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using unsafe HDC = void*;

namespace Cairo.Surfaces.Win32;

// https://www.cairographics.org/manual/cairo-Win32-Surfaces.html

internal static unsafe partial class Win32SurfaceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_win32_surface_create(HDC hdc);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_win32_surface_create_with_dib(Format format, int width, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_win32_surface_create_with_ddb(HDC hdc, Format format, int width, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_win32_surface_create_with_format(HDC hdc, Format format);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_win32_printing_surface_create(HDC hdc);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial HDC cairo_win32_surface_get_dc(cairo_surface_t surface);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_win32_surface_get_image(cairo_surface_t surface);
}
