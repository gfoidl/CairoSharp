// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Extensions.Loading.SVG;

internal struct GFile;
internal struct GInputStream;

internal struct RsvgHandle;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct GError
{
    public int Domain;
    public int Code;
    public sbyte* Message;
}
