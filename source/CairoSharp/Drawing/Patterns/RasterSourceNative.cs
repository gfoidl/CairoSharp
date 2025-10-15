// (c) gfoidl, all rights reserved

global using unsafe cairo_raster_source_acquire_func_t  = delegate*<void*, void*, void*, ref Cairo.RectangleInt, void*>;
global using unsafe cairo_raster_source_release_func_t  = delegate*<void*, void*, void*, void>;
global using unsafe cairo_raster_source_snapshot_func_t = delegate*<void*, void*, Cairo.Status>;
global using unsafe cairo_raster_source_copy_func_t     = delegate*<void*, void*, void*, Cairo.Status>;
global using unsafe cairo_raster_source_finish_func_t   = delegate*<void*, void*, void>;

using System.Runtime.InteropServices;
using Cairo.Surfaces;

namespace Cairo.Drawing.Patterns;

// https://www.cairographics.org/manual/cairo-Raster-Sources.html

internal static unsafe partial class RasterSourceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_pattern_create_raster_source(void* user_data, Content content, int width, int height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_callback_data(cairo_pattern_t* pattern, void* data);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_raster_source_pattern_get_callback_data(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_acquire(cairo_pattern_t* pattern, cairo_raster_source_acquire_func_t acquire, cairo_raster_source_release_func_t release);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_get_acquire(cairo_pattern_t* pattern, out cairo_raster_source_acquire_func_t acquire, out cairo_raster_source_release_func_t release);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_snapshot(cairo_pattern_t* pattern, cairo_raster_source_snapshot_func_t snapshot);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_raster_source_snapshot_func_t cairo_raster_source_pattern_get_snapshot(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_copy(cairo_pattern_t* pattern, cairo_raster_source_copy_func_t copy);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_raster_source_copy_func_t cairo_raster_source_pattern_get_copy(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_raster_source_pattern_set_finish(cairo_pattern_t* pattern, cairo_raster_source_finish_func_t finish);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_raster_source_finish_func_t cairo_raster_source_pattern_get_finish(cairo_pattern_t* pattern);
}
