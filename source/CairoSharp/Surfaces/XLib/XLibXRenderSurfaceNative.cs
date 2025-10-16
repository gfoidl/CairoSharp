// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using unsafe Display           = void*;
using Drawable                 = uint;
using unsafe Screen            = void*;
using unsafe XRenderPictFormat = void*;

namespace Cairo.Surfaces.XLib;

// https://www.cairographics.org/manual/cairo-XLib-XRender-Backend.html

internal static unsafe partial class XLibXRenderSurfaceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_xlib_surface_create_with_xrender_format(Display dpy, Drawable drawable, Screen screen, XRenderPictFormat format, int widht, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial XRenderPictFormat* cairo_xlib_surface_get_xrender_format(cairo_surface_t* surface);
}
