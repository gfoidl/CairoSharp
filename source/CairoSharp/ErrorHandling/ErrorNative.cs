// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Cairo.ErrorHandling;

// https://www.cairographics.org/manual/cairo-Error-handling.html#cairo-status-t

internal static unsafe partial class ErrorNative
{
    // Can't use StringMarshalling here, as that would try to free the native
    // return result, which in case here is a const char* pointing to a internal field,
    // so can't be freed at that time.
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string? cairo_status_to_string(Status status);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_debug_reset_static_data();
}
