// (c) gfoidl, all rights reserved

using System.ComponentModel;
using static Cairo.Drawing.Regions.RegionNative;

namespace Cairo.Drawing.Regions;

[EditorBrowsable(EditorBrowsableState.Never)]
public struct cairo_region_t;

/// <summary>
/// Regions — Representing a pixel-aligned area
/// </summary>
/// <remarks>
/// Regions are a simple graphical data type representing an area of integer-aligned rectangles.
/// They are often used on raster surfaces to track areas of interest, such as change or clip areas.
/// </remarks>
public sealed unsafe class Region : CairoObject<cairo_region_t>, IEquatable<Region>
{
    private Region(cairo_region_t* region, bool isOwnedByCairo = false) : base(region, isOwnedByCairo)
        => this.Status.ThrowIfNotSuccess();

    /// <summary>
    /// Allocates a new empty region object.
    /// </summary>
    public Region() : this(cairo_region_create()) { }

    /// <summary>
    /// Allocates a new region object containing rectangle.
    /// </summary>
    /// <param name="rectangle">a cairo <see cref="RectangleInt"/></param>
    public Region(RectangleInt rectangle) : this(cairo_region_create_rectangle(ref rectangle)) { }

    /// <summary>
    /// Allocates a new region object containing the union of all given rects.
    /// </summary>
    /// <param name="rectangles">an array of rectangles</param>
    public Region(params ReadOnlySpan<RectangleInt> rectangles) : this(CreateForRectangles(rectangles)) { }

    private static cairo_region_t* CreateForRectangles(ReadOnlySpan<RectangleInt> rectangles)
    {
        fixed (RectangleInt* ptr = rectangles)
        {
            return cairo_region_create_rectangles(ptr, rectangles.Length);
        }
    }

    protected override void DisposeCore(cairo_region_t* region) => cairo_region_destroy(region);

    /// <summary>
    /// Allocates a new region object copying the area from this one.
    /// </summary>
    /// <returns>A newly allocated <see cref="Region"/>.</returns>
    /// <remarks>
    /// Free with <see cref="CairoObject.Dispose()"/>.
    /// </remarks>
    public Region Copy()
    {
        this.CheckDisposed();

        cairo_region_t* copy = cairo_region_copy(this.Handle);
        Region region        = new(copy, isOwnedByCairo: false);

        region.Status.ThrowIfNotSuccess();
        return region;
    }

    /// <summary>
    /// Increases the reference count on region by one. This prevents region from being destroyed
    /// until a matching call to cairo_region_destroy() is made.
    /// </summary>
    internal void Reference()
    {
        this.CheckDisposed();
        cairo_region_reference(this.Handle);
    }

    /// <summary>
    /// Checks whether an error has previous occurred for this region object.
    /// </summary>
    public Status Status
    {
        get
        {
            this.CheckDisposed();
            return cairo_region_status(this.Handle);
        }
    }

    /// <summary>
    /// Gets the bounding rectangle of region as a <see cref="RectangleInt"/>.
    /// </summary>
    /// <param name="rectangle">rectangle into which to store the extents</param>
    public void GetExtents(out RectangleInt rectangle)
    {
        this.CheckDisposed();
        cairo_region_get_extents(this.Handle, out rectangle);
    }

    /// <summary>
    /// Returns the number of rectangles contained in region.
    /// </summary>
    public int Rectangles
    {
        get
        {
            this.CheckDisposed();
            return cairo_region_num_rectangles(this.Handle);
        }
    }

    /// <summary>
    /// Stores the nth rectangle from the region in rectangle.
    /// </summary>
    /// <param name="rectangleIndex">a number indicating which rectangle should be returned</param>
    /// <returns>the nth rectangle</returns>
    public RectangleInt this[int rectangleIndex]
    {
        get
        {
            this.CheckDisposed();

            cairo_region_get_rectangle(this.Handle, rectangleIndex, out RectangleInt rectangle);
            return rectangle;
        }
    }

    /// <summary>
    /// Checks whether region is empty.
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            this.CheckDisposed();
            return cairo_region_is_empty(this.Handle);
        }
    }

    /// <summary>
    /// Checks whether (x , y ) is contained in region.
    /// </summary>
    /// <param name="x">the x coordinate of a point</param>
    /// <param name="y">the y coordinate of a point</param>
    public bool ContainsPoint(int x, int y)
    {
        this.CheckDisposed();
        return cairo_region_contains_point(this.Handle, x, y);
    }

    /// <summary>
    /// Checks whether <paramref name="point"/> is contained in region.
    /// </summary>
    /// <param name="point">a point</param>
    public bool ContainsPoint(Point point) => this.ContainsPoint(point.X, point.Y);

    /// <summary>
    /// Checks whether rectangle is inside, outside or partially contained in region 
    /// </summary>
    /// <param name="rectangle">a cairo <see cref="RectangleInt"/></param>
    /// <returns>
    /// <see cref="RegionOverlap.In"/> if rectangle is entirely inside region, <see cref="RegionOverlap.Out"/>
    /// if rectangle is entirely outside region, or <see cref="RegionOverlap.Part"/> if rectangle is partially
    /// inside and partially outside region.
    /// </returns>
    public RegionOverlap ContainsRectangle(RectangleInt rectangle)
    {
        this.CheckDisposed();
        return cairo_region_contains_rectangle(this.Handle, ref rectangle);
    }

    /// <summary>
    /// Compares whether region_a is equivalent to region_b. <c>null</c> as an argument is equal
    /// to itself, but not to any non-<c>null</c> region.
    /// </summary>
    /// <param name="other">a cairo <see cref="Region"/> or <c>null</c></param>
    /// <returns>
    /// <c>true</c> if both regions contained the same coverage, <c>false</c> if it is not or
    /// any region is in an error status.
    /// </returns>
    public bool Equals(Region? other)
    {
        this.CheckDisposed();

        if (other is null)
        {
            return false;
        }

        return cairo_region_equal(this.Handle, other.Handle);
    }

    /// <summary>
    /// Translates region by (dx , dy ).
    /// </summary>
    /// <param name="dx">Amount to translate in the x direction</param>
    /// <param name="dy">Amount to translate in the y direction</param>
    public void Translate(int dx, int dy)
    {
        this.CheckDisposed();
        cairo_region_translate(this.Handle, dx, dy);
    }

    /// <summary>
    /// Translates region by <paramref name="offset"/>.
    /// </summary>
    /// <param name="offset">The offset for the translation</param>
    public void Translate(Point offset) => this.Translate(offset.X, offset.Y);

    /// <summary>
    /// Computes the intersection of dst with other and places the result in dst
    /// </summary>
    /// <param name="other">another <see cref="Region"/></param>
    public void Intersect(Region other)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(other);

        cairo_region_intersect(this.Handle, other.Handle).ThrowIfStatus(Status.NoMemory);
    }

    /// <summary>
    /// Computes the intersection of dst with rectangle and places the result in dst
    /// </summary>
    /// <param name="rectangle">a cairo <see cref="RectangleInt"/></param>
    public void Intersect(RectangleInt rectangle)
    {
        this.CheckDisposed();
        cairo_region_intersect_rectangle(this.Handle, ref rectangle).ThrowIfStatus(Status.NoMemory);
    }

    /// <summary>
    /// Subtracts other from dst and places the result in dst
    /// </summary>
    /// <param name="other">another <see cref="Region"/></param>
    public void Subtract(Region other)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(other);

        cairo_region_subtract(this.Handle, other.Handle).ThrowIfStatus(Status.NoMemory);
    }

    /// <summary>
    /// Subtracts rectangle from dst and places the result in dst
    /// </summary>
    /// <param name="rectangle">a <see cref="RectangleInt"/></param>
    public void Subtract(RectangleInt rectangle)
    {
        this.CheckDisposed();
        cairo_region_subtract_rectangle(this.Handle, ref rectangle).ThrowIfStatus(Status.NoMemory);
    }

    /// <summary>
    /// Computes the union of dst with other and places the result in dst
    /// </summary>
    /// <param name="other">another <see cref="Region"/></param>
    public void Union(Region other)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(other);

        cairo_region_union(this.Handle, other.Handle).ThrowIfStatus(Status.NoMemory);
    }

    /// <summary>
    /// Computes the union of dst with rectangle and places the result in dst.
    /// </summary>
    /// <param name="rectangle">a <see cref="RectangleInt"/></param>
    public void Union(RectangleInt rectangle)
    {
        this.CheckDisposed();
        cairo_region_union_rectangle(this.Handle, ref rectangle).ThrowIfStatus(Status.NoMemory);
    }

    /// <summary>
    /// Computes the exclusive difference of dst with other and places the result in dst. That is,
    /// dst will be set to contain all areas that are either in dst or in other, but not in both.
    /// </summary>
    /// <param name="other">another <see cref="Region"/></param>
    public void Xor(Region other)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(other);

        cairo_region_xor(this.Handle, other.Handle).ThrowIfStatus(Status.NoMemory);
    }

    /// <summary>
    /// Computes the exclusive difference of dst with rectangle and places the result in dst. That is,
    /// dst will be set to contain all areas that are either in dst or in rectangle, but not in both.
    /// </summary>
    /// <param name="rectangle">a <see cref="RectangleInt"/></param>
    public void Xor(RectangleInt rectangle)
    {
        this.CheckDisposed();
        cairo_region_xor_rectangle(this.Handle, ref rectangle).ThrowIfStatus(Status.NoMemory);
    }
}
