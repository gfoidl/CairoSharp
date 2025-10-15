// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using static Cairo.CairoContextNative;

namespace Cairo;

/// <summary>
/// A data structure for holding a dynamically allocated array of rectangles.
/// </summary>
public sealed unsafe class RectangleList : CairoObject
{
    internal RectangleList(cairo_rectangle_list_t* handle) : base(handle) { }

    protected override void DisposeCore(void* handle) => cairo_rectangle_list_destroy(handle);

    /// <summary>
    /// Status of the current clip region.
    /// </summary>
    public Status Status => ((cairo_rectangle_list_t*)this.Handle)->status;

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
            cairo_rectangle_list_t* ptr = (cairo_rectangle_list_t*)this.Handle;
            return new ReadOnlySpan<Rectangle>(ptr->rectangles, ptr->num_rectangles);
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct cairo_rectangle_list_t
{
    public Status     status;
    public Rectangle* rectangles;
    public int        num_rectangles;
}
