// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Surfaces.Recording;

// https://www.cairographics.org/manual/cairo-Recording-Surfaces.html

internal static unsafe partial class RecordingSurfaceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_recording_surface_create(Content content, Rectangle* extents);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_recording_surface_ink_extents(cairo_surface_t* surface, out double x0, out double y0, out double width, out double height);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool cairo_recording_surface_get_extents(cairo_surface_t* surface, out Rectangle extents);
}
