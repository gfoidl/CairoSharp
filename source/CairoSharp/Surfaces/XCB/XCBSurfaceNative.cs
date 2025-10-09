// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using xcb_drawable_t = uint;
using xcb_pixmap_t   = uint;

namespace Cairo.Surfaces.XCB;

// https://www.cairographics.org/manual/cairo-XCB-Surfaces.html

internal static unsafe partial class XCBSurfaceNative
{
    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_xcb_surface_create(void* connection, xcb_drawable_t drawable, void* visual, int width, int height);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_xcb_surface_create_for_bitmap(void* connection, xcb_pixmap_t bitmap, void* screen, int width, int height);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_xcb_surface_create_with_xrender_format(void* connection, void* screen, xcb_drawable_t drawable, void* format, int width, int height);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_surface_set_size(void* surface, int width, int height);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_surface_set_drawable(void* surface, xcb_drawable_t drawable, int width, int height);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_xcb_device_get_connection(void* device);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_device_debug_cap_xrender_version(void* device, int major_version, int minor_version);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_device_debug_cap_xshm_version(void* device, int major_version, int minor_version);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial int cairo_xcb_device_debug_get_precision(void* device);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_xcb_device_debug_set_precision(void* device, int precision);
}
