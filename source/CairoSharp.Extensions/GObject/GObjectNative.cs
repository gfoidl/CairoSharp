// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Extensions.GObject;

internal static unsafe partial class GObjectNative
{
    public const string LibGObjectName = "libgobject-2.0.so.0";

    [LibraryImport(LibGObjectName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void g_object_unref(void* @object);
}
