// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using static Cairo.CairoContextNative;

namespace Cairo;

/// <summary>
/// A data structure for holding a dynamically allocated array of rectangles.
/// </summary>
public sealed unsafe class RectangleList : CairoObject
{
    internal RectangleList(RectangleListRaw* handle) : base(handle) { }

    protected override void DisposeCore(void* handle) => cairo_rectangle_list_destroy(handle);

    /// <summary>
    /// Status of the current clip region.
    /// </summary>
    public Status Status => ((RectangleListRaw*)this.Handle)->Status;

    /// <summary>
    /// The <see cref="Rectangle"/> in the clip region.
    /// </summary>
    /// <remarks>
    /// See <see cref="CairoContext.CopyClipRectangleList"/> for more information.
    /// </remarks>
    public ReadOnlySpan<Rectangle> Rectangles
    {
        get
        {
            RectangleListRaw* ptr = (RectangleListRaw*)this.Handle;
            return new ReadOnlySpan<Rectangle>(ptr->Rectangles, ptr->Count);
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct RectangleListRaw
{
    public Status Status;
    public Rectangle* Rectangles;
    public int Count;
}
