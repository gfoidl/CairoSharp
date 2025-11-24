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
    internal static partial int cairo_version();

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(StaticNativeStringMarshaller))]
    internal static partial string? cairo_version_string();
}
