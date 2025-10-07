// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using Cairo.Surfaces;

namespace Cairo.Drawing.Patterns;

// https://www.cairographics.org/manual/cairo-Raster-Sources.html

internal static unsafe partial class RasterSourceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_pattern_create_raster_source(void* user_data, Content content, int width, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_callback_data(void* pattern, void* data);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_raster_source_pattern_get_callback_data(void* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_acquire(void* pattern, cairo_raster_source_acquire_func_t acquire, cairo_raster_source_release_func_t release);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_get_acquire(void* pattern, out cairo_raster_source_acquire_func_t acquire, out cairo_raster_source_release_func_t release);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_snapshot(void* pattern, cairo_raster_source_snapshot_func_t snapshot);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_raster_source_snapshot_func_t cairo_raster_source_pattern_get_snapshot(void* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_copy(void* pattern, cairo_raster_source_copy_func_t copy);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_raster_source_copy_func_t cairo_raster_source_pattern_get_copy(void* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_finish(void* pattern, cairo_raster_source_finish_func_t finish);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_raster_source_finish_func_t cairo_raster_source_pattern_get_finish(void* pattern);
}
