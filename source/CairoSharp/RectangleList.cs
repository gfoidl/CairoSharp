// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using static Cairo.CairoContextNative;

namespace Cairo;

/// <summary>
/// A data structure for holding a dynamically allocated array of rectangles.
/// </summary>
public sealed unsafe class RectangleList : CairoObject<cairo_rectangle_list_t>
{
    internal RectangleList(cairo_rectangle_list_t* rectangleList) : base(rectangleList) { }

    protected override void DisposeCore(cairo_rectangle_list_t* rectangleList)
        => cairo_rectangle_list_destroy(rectangleList);

    /// <summary>
    /// Status of the current clip region.
    /// </summary>
    public Status Status => (this.Handle)->status;

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
            cairo_rectangle_list_t* ptr = this.Handle;
            return new ReadOnlySpan<Rectangle>(ptr->rectangles, ptr->num_rectangles);
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct cairo_rectangle_list_t
{
    public Status     status;
    public Rectangle* rectangles;
    public int        num_rectangles;
}
