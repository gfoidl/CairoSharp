// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices.Marshalling;
using Cairo.Extensions.GObject;

namespace Cairo.Extensions.Loading;

[CustomMarshaller(typeof(string), MarshalMode.Default, typeof(GCharMarshaller))]
internal static unsafe class GCharMarshaller
{
    public static string? ConvertToManaged(sbyte* utf8)
    {
        return utf8 is null
            ? null
            : new string(utf8);
    }
    //-------------------------------------------------------------------------
    public static void Free(sbyte* utf8) => GObjectNative.g_free(utf8);
}
