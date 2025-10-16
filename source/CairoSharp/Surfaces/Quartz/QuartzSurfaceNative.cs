// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using unsafe CGContextRef = void*;

namespace Cairo.Surfaces.Quartz;

// https://www.cairographics.org/manual/cairo-Quartz-Surfaces.html

internal static unsafe partial class QuartzSurfaceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_quartz_surface_create(Format format, uint width, uint height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_quartz_surface_create_for_cg_context(CGContextRef cgContext, uint width, uint height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial CGContextRef cairo_quartz_surface_get_cg_context(cairo_surface_t* surface);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_quartz_image_surface_create(cairo_surface_t* image_surface);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_quartz_image_surface_get_image(cairo_surface_t* surface);
}
