// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using unsafe CGContextRef = void*;

namespace Cairo.Surfaces.Quartz;

// https://www.cairographics.org/manual/cairo-Quartz-Surfaces.html

internal static unsafe partial class QuartzSurfaceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_quartz_surface_create(Format format, uint width, uint height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_quartz_surface_create_for_cg_context(CGContextRef cgContext, uint width, uint height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_quartz_surface_get_cg_context(void* surface);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_quartz_image_surface_create(void* image_surface);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_quartz_image_surface_get_image(void* surface);
}
