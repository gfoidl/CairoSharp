// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Drawing.TagsAndLinks;

internal static unsafe partial class TagsAndLinksNative
{
    [LibraryImport(NativeMethods.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_tag_begin(void* cr, string tag_name, string attributes);

    [LibraryImport(NativeMethods.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_tag_end(void* cr, string tag_name);
}
