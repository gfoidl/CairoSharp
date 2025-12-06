// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Extensions.GObject;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct GError
{
    public int Domain;
    public int Code;
    public sbyte* Message;
}
