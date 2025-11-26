// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Cairo.Utilities;

// https://www.cairographics.org/manual/cairo-Version-Information.html
// https://www.cairographics.org/manual/cairo-Types.html

internal static unsafe partial class UtilitiesNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial int cairo_version();

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string? cairo_version_string();
}
