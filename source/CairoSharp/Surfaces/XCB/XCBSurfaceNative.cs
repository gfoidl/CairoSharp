// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using xcb_connection_t          = uint;
using xcb_drawable_t            = uint;
using xcb_screen_t              = uint;
using xcb_visualtype_t          = uint;
using xcb_pixmap_t              = uint;
using xcb_render_pictforminfo_t = uint;

namespace Cairo.Surfaces.XCB;

// https://www.cairographics.org/manual/cairo-XCB-Surfaces.html

internal static unsafe partial class XCBSurfaceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_xcb_surface_create(xcb_connection_t* connection, xcb_drawable_t drawable, xcb_visualtype_t* visual, int width, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_xcb_surface_create_for_bitmap(xcb_connection_t* connection, xcb_screen_t* screen, xcb_pixmap_t bitmap, int width, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_xcb_surface_create_with_xrender_format(xcb_connection_t* connection, xcb_screen_t* screen, xcb_drawable_t drawable, xcb_render_pictforminfo_t* format, int width, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_surface_set_size(cairo_surface_t* surface, int width, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_surface_set_drawable(cairo_surface_t* surface, xcb_drawable_t drawable, int width, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial xcb_connection_t* cairo_xcb_device_get_connection(cairo_device_t* device);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_device_debug_cap_xrender_version(cairo_device_t* device, int major_version, int minor_version);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_device_debug_cap_xshm_version(cairo_device_t* device, int major_version, int minor_version);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial int cairo_xcb_device_debug_get_precision(cairo_device_t* device);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_device_debug_set_precision(cairo_device_t* device, int precision);
}
