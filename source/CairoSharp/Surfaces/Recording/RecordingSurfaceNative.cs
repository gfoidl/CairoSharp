// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Surfaces.Recording;

// https://www.cairographics.org/manual/cairo-Recording-Surfaces.html

internal static unsafe partial class RecordingSurfaceNative
{
    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_recording_surface_create(Content content, Rectangle* extents);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_recording_surface_ink_extents(void* surface, out double x0, out double y0, out double width, out double height);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool cairo_recording_surface_get_extents(void* surface, out Rectangle extents);
}
