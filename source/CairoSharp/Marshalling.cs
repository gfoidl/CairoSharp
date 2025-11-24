// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices.Marshalling;

namespace Cairo;

[CustomMarshaller(typeof(string), MarshalMode.Default, typeof(StaticNativeStringMarshaller))]
internal static unsafe class StaticNativeStringMarshaller
{
    public static string? ConvertToManaged(sbyte* utf8)
    {
        return utf8 is null
            ? null
            : new string(utf8);
    }
}
