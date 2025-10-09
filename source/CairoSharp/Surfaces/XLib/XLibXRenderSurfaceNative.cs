// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using Drawable = uint;

namespace Cairo.Surfaces.XLib;

// https://www.cairographics.org/manual/cairo-XLib-XRender-Backend.html

internal static unsafe partial class XLibXRenderSurfaceNative
{
    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_xlib_surface_create_with_xrender_format(void* dpy, Drawable drawable, void* screen, void* format, int widht, int height);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_xlib_surface_get_xrender_format(void* surface);
}
