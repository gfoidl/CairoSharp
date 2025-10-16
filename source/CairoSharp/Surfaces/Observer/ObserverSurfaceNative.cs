// (c) gfoidl, all rights reserved

global using unsafe cairo_surface_observer_callback_t = delegate*<void*, void*, void*, void>;

using System.Runtime.InteropServices;

namespace Cairo.Surfaces.Observer;

// https://www.cairographics.org/manual/cairo-Surface-Observer.html

internal static unsafe partial class ObserverSurfaceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_surface_create_observer(cairo_surface_t* target, ObserverMode mode);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_fill_callback(cairo_surface_t* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_finish_callback(cairo_surface_t* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_flush_callback(cairo_surface_t* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_glyphs_callback(cairo_surface_t* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_mask_callback(cairo_surface_t* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_paint_callback(cairo_surface_t* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_stroke_callback(cairo_surface_t* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial double cairo_surface_observer_elapsed(cairo_surface_t* abstract_surface);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_print(cairo_surface_t* abstract_surface, cairo_write_func_t write_func, void* closure);
}
