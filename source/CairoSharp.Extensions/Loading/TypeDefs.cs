// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Extensions.Loading;

internal struct GFile;
internal struct GInputStream;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct GError
{
    public int Domain;
    public int Code;
    public sbyte* Message;
}

internal struct RsvgHandle;

internal struct PopplerDocument;
internal struct PopplerPage;
