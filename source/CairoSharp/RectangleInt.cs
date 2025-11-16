// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo;

[StructLayout(LayoutKind.Sequential)]
public struct RectangleInt
{
    public int X;
    public int Y;
    public int Width;
    public int Height;

    public static implicit operator Rectangle(RectangleInt rectangleInt)
        => new Rectangle(rectangleInt.X, rectangleInt.Y, rectangleInt.Width, rectangleInt.Height);
}
