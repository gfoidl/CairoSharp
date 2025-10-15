// (c) gfoidl, all rights reserved

using System.ComponentModel;
using System.Runtime.InteropServices;
using static Cairo.CairoContextNative;

namespace Cairo;

/// <summary>
/// A data structure for holding a dynamically allocated array of rectangles.
/// </summary>
public sealed unsafe class RectangleList : CairoObject<RectangleListRaw>
{
    internal RectangleList(RectangleListRaw* rectangleList) : base(rectangleList) { }

    protected override void DisposeCore(RectangleListRaw* rectangleList)
        => cairo_rectangle_list_destroy(rectangleList);

    /// <summary>
    /// Status of the current clip region.
    /// </summary>
    public Status Status => (this.Handle)->Status;

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
            RectangleListRaw* ptr = this.Handle;
            return new ReadOnlySpan<Rectangle>(ptr->Rectangles, ptr->Count);
        }
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct RectangleListRaw
{
    public Status     Status;
    public Rectangle* Rectangles;
    public int        Count;
}
