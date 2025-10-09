// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Utilities;

// https://www.cairographics.org/manual/cairo-Version-Information.html
// https://www.cairographics.org/manual/cairo-Types.html

internal static unsafe partial class UtilitiesNative
{
    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial int cairo_version();

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial sbyte* cairo_version_string();
}
