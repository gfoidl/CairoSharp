// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Drawing;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct Point(int X, int Y);
