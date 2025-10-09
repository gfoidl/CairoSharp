// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Surfaces.Observer;

// https://www.cairographics.org/manual/cairo-Surface-Observer.html

internal static unsafe partial class ObserverSurfaceNative
{
    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_surface_create_observer(void* target, ObserverMode mode);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_fill_callback(void* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_finish_callback(void* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_flush_callback(void* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_glyphs_callback(void* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_mask_callback(void* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_paint_callback(void* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_add_stroke_callback(void* abstract_surface, cairo_surface_observer_callback_t func, void* data);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial double cairo_surface_observer_elapsed(void* abstract_surface);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_surface_observer_print(void* abstract_surface, cairo_write_func_t write_func, void* closure);
}
