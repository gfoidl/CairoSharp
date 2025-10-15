// (c) gfoidl, all rights reserved

using Cairo.Drawing.Path;
using static Cairo.Drawing.Patterns.PatternNative;

namespace Cairo.Drawing.Patterns;

/// <summary>
/// Mesh patterns are tensor-product patch meshes (type 7 shadings in PDF). Mesh patterns may also be
/// used to create other types of shadings that are special cases of tensor-product patch meshes such
/// as Coons patch meshes (type 6 shading in PDF) and Gouraud-shaded triangle meshes (type 4 and 5
/// shadings in PDF).
/// </summary>
/// <remarks>
/// Mesh patterns consist of one or more tensor-product patches, which should be defined before using
/// the mesh pattern. Using a mesh pattern with a partially defined patch as source or mask will put
/// the context in an error status with a status of <see cref="Status.InvalidMeshConstruction"/>.
/// <para>
/// See <a href="https://www.cairographics.org/manual/cairo-cairo-pattern-t.html#cairo-pattern-create-mesh">cairo docs</a>
/// for further information. For an example see <a href="https://gist.github.com/mgdm/3159434">mesh_pattern.c gist</a>.
/// </para>
/// <para>
/// Note: The coordinates are always in pattern space. For a new pattern, pattern space is identical
/// to user space, but the relationship between the spaces can be changed with <see cref="Pattern.GetMatrix(out Matrix)"/>.
/// </para>
/// </remarks>
public sealed unsafe class Mesh : Pattern
{
    internal Mesh(cairo_pattern_t* pattern, bool isOwnedByCairo, bool needsDestroy = true)
        : base(pattern, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Create a new mesh pattern.
    /// </summary>
    public Mesh() : base(cairo_pattern_create_mesh()) { }

    /// <summary>
    /// Begin a patch in a mesh pattern.
    /// </summary>
    /// <returns>
    /// A <see cref="PatchScope"/>, which when <see cref="PatchScope.Dispose"/>d calls
    /// <see cref="EndPatch"/>.
    /// <para>
    /// So instead of writing code like
    /// <code>
    /// mesh.BeginPatch();
    /// // ...
    /// mesh.EndPatch();
    /// </code>
    /// one can write
    /// <code>
    /// using (mesh.BeginPatch())
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </para>
    /// </returns>
    /// <remarks>
    /// After calling this method, the patch shape should be defined with <see cref="MoveTo(PointD)"/>,
    /// <see cref="LineTo(PointD)"/> and <see cref="CurveTo(PointD, PointD, PointD)"/>.
    /// <para>
    /// After defining the patch, <see cref="EndPatch"/> must be called before using pattern as a
    /// source or mask.
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">If the mesh already has a current patch.</exception>
    public PatchScope BeginPatch()
    {
        this.CheckDisposed();
        cairo_mesh_pattern_begin_patch(this.Handle);

        this.Status.ThrowIfStatus(Status.InvalidMeshConstruction);

        return new PatchScope(this);
    }

    /// <summary>
    /// Indicates the end of the current patch in a mesh pattern.
    /// </summary>
    /// <remarks>
    /// If the current patch has less than 4 sides, it is closed with a straight line from the
    /// current point to the first point of the patch as if <see cref="LineTo(PointD)"/> was used.
    /// </remarks>
    /// <exception cref="CairoException">
    /// If the mesh has no current patch or the current patch has no current point.
    /// </exception>
    public void EndPatch()
    {
        this.CheckDisposed();

        cairo_mesh_pattern_end_patch(this.Handle);

        this.Status.ThrowIfStatus(Status.InvalidMeshConstruction);
    }

    /// <summary>
    /// Define the first point of the current patch in a mesh pattern.
    /// </summary>
    /// <param name="x">the X coordinate of the new position</param>
    /// <param name="y">the Y coordinate of the new position</param>
    /// <remarks>
    /// After this call the current point will be (x , y ).
    /// </remarks>
    /// <exception cref="CairoException">
    /// If pattern has no current patch or the current patch already has at least one side.
    /// </exception>
    public void MoveTo(double x, double y)
    {
        this.CheckDisposed();

        cairo_mesh_pattern_move_to(this.Handle, x, y);

        this.Status.ThrowIfStatus(Status.InvalidMeshConstruction);
    }

    /// <summary>
    /// Define the first point of the current patch in a mesh pattern.
    /// </summary>
    /// <param name="point">the new position</param>
    /// <remarks>
    /// After this call the current point will be (x , y ).
    /// </remarks>
    /// <exception cref="CairoException">
    /// If pattern has no current patch or the current patch already has at least one side.
    /// </exception>
    public void MoveTo(PointD point) => this.MoveTo(point.X, point.Y);

    /// <summary>
    /// Adds a line to the current patch from the current point to position (x , y )
    /// in pattern-space coordinates.
    /// </summary>
    /// <param name="x">the X coordinate of the end of the new line</param>
    /// <param name="y">the Y coordinate of the end of the new line</param>
    /// <remarks>
    /// If there is no current point before the call to <see cref="LineTo(double, double)"/> this method
    /// will behave as <see cref="MoveTo(double, double)"/>.
    /// <para>
    /// After this call the current point will be (x , y ).
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">
    /// If pattern has no current patch or the current patch already has 4 sides.
    /// </exception>
    public void LineTo(double x, double y)
    {
        this.CheckDisposed();

        cairo_mesh_pattern_line_to(this.Handle, x, y);

        this.Status.ThrowIfStatus(Status.InvalidMeshConstruction);
    }

    /// <summary>
    /// Adds a line to the current patch from the current point to position (x , y )
    /// in pattern-space coordinates.
    /// </summary>
    /// <param name="point">the end of the new line</param>
    /// <remarks>
    /// If there is no current point before the call to <see cref="LineTo(double, double)"/> this method
    /// will behave as <see cref="MoveTo(double, double)"/>.
    /// <para>
    /// After this call the current point will be (x , y ).
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">
    /// If pattern has no current patch or the current patch already has 4 sides.
    /// </exception>
    public void LineTo(PointD point) => this.LineTo(point.X, point.Y);

    /// <summary>
    /// Adds a cubic Bézier spline to the current patch from the current point to position
    /// (x3 , y3 ) in pattern-space coordinates, using (x1 , y1 ) and (x2 , y2 ) as the
    /// control points.
    /// </summary>
    /// <param name="x1">the X coordinate of the first control point</param>
    /// <param name="y1">the Y coordinate of the first control point</param>
    /// <param name="x2">the X coordinate of the second control point</param>
    /// <param name="y2">the Y coordinate of the second control point</param>
    /// <param name="x3">the X coordinate of the end of the curve</param>
    /// <param name="y3">the Y coordinate of the end of the curve</param>
    /// <remarks>
    /// If the current patch has no current point before the call to <see cref="CurveTo(double, double, double, double, double, double)"/>,
    /// this method will behave as if preceded by a call to <see cref="MoveTo(double, double)"/>.
    /// <para>
    /// After this call the current point will be (x3 , y3 ).
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">
    /// If pattern has no current patch or the current patch already has 4 sides.
    /// </exception>
    public void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        this.CheckDisposed();

        cairo_mesh_pattern_curve_to(this.Handle, x1, y1, x2, y2, x3, y3);

        this.Status.ThrowIfStatus(Status.InvalidMeshConstruction);
    }

    /// <summary>
    /// Adds a cubic Bézier spline to the current patch from the current point to position
    /// (x3 , y3 ) in pattern-space coordinates, using (x1 , y1 ) and (x2 , y2 ) as the
    /// control points.
    /// </summary>
    /// <param name="p1">the first control point</param>
    /// <param name="p2">the second control point</param>
    /// <param name="p3">the end of the curve</param>
    /// <remarks>
    /// If the current patch has no current point before the call to <see cref="CurveTo(double, double, double, double, double, double)"/>,
    /// this method will behave as if preceded by a call to <see cref="MoveTo(double, double)"/>.
    /// <para>
    /// After this call the current point will be (x3 , y3 ).
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">
    /// If pattern has no current patch or the current patch already has 4 sides.
    /// </exception>
    public void CurveTo(PointD p1, PointD p2, PointD p3) => this.CurveTo(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

    /// <summary>
    /// Set an internal control point of the current patch.
    /// </summary>
    /// <param name="pointIndex">the control point to set the position for</param>
    /// <param name="x">the X coordinate of the control point</param>
    /// <param name="y">the Y coordinate of the control point</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="pointIndex"/> is not in the range [0, 3].
    /// </exception>
    /// <exception cref="CairoException"> If pattern has no current patch</exception>
    public void SetControlPoint(int pointIndex, double x, double y)
    {
        this.CheckDisposed();

        ArgumentOutOfRangeException.ThrowIfLessThan(pointIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pointIndex, 3);

        cairo_mesh_pattern_set_control_point(this.Handle, (uint)pointIndex, x, y);

        Status status = this.Status;
        status.ThrowIfStatus(Status.InvalidIndex);
        status.ThrowIfStatus(Status.InvalidMeshConstruction);
    }

    /// <summary>
    /// Set an internal control point of the current patch.
    /// </summary>
    /// <param name="pointIndex">the control point to set the position for</param>
    /// <param name="point">the control point</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="pointIndex"/> is not in the range [0, 3].
    /// </exception>
    /// <exception cref="CairoException"> If pattern has no current patch</exception>
    public void SetControlPoint(int pointIndex, PointD point) => this.SetControlPoint(pointIndex, point.X, point.Y);

    /// <summary>
    /// Sets the color of a corner of the current patch in a mesh pattern.
    /// </summary>
    /// <param name="cornerIndex">the corner to set the color for</param>
    /// <param name="red">red component of color</param>
    /// <param name="green">green component of color</param>
    /// <param name="blue">blue component of color</param>
    /// <remarks>
    /// The color is specified in the same way as in <see cref="CairoContext.SetSourceRgb(double, double, double)"/>.
    /// <para>
    /// Valid values for corner_num are from 0 to 3 and identify the corners as explained in <see cref="Mesh()"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="cornerIndex"/> is not in the range [0, 3].
    /// </exception>
    /// <exception cref="CairoException"> If pattern has no current patch</exception>
    public void SetCornerColorRgb(int cornerIndex, double red, double green, double blue)
    {
        this.CheckDisposed();

        ArgumentOutOfRangeException.ThrowIfLessThan(cornerIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(cornerIndex, 3);

        cairo_mesh_pattern_set_corner_color_rgb(this.Handle, (uint)cornerIndex, red, green, blue);

        Status status = this.Status;
        status.ThrowIfStatus(Status.InvalidIndex);
        status.ThrowIfStatus(Status.InvalidMeshConstruction);
    }

    /// <summary>
    /// Sets the color of a corner of the current patch in a mesh pattern.
    /// </summary>
    /// <param name="cornerIndex">the corner to set the color for</param>
    /// <param name="red">red component of color</param>
    /// <param name="green">green component of color</param>
    /// <param name="blue">blue component of color</param>
    /// <param name="alpha">alpha component of color</param>
    /// <remarks>
    /// The color is specified in the same way as in <see cref="CairoContext.SetSourceRgba(double, double, double, double)"/>.
    /// <para>
    /// Valid values for corner_num are from 0 to 3 and identify the corners as explained in <see cref="Mesh()"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="cornerIndex"/> is not in the range [0, 3].
    /// </exception>
    /// <exception cref="CairoException"> If pattern has no current patch</exception>
    public void SetCornerColorRgba(int cornerIndex, double red, double green, double blue, double alpha)
    {
        this.CheckDisposed();

        ArgumentOutOfRangeException.ThrowIfLessThan(cornerIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(cornerIndex, 3);

        cairo_mesh_pattern_set_corner_color_rgba(this.Handle, (uint)cornerIndex, red, green, blue, alpha);

        Status status = this.Status;
        status.ThrowIfStatus(Status.InvalidIndex);
        status.ThrowIfStatus(Status.InvalidMeshConstruction);
    }

    /// <summary>
    /// Sets the color of a corner of the current patch in a mesh pattern.
    /// </summary>
    /// <param name="cornerIndex">the corner to set the color for</param>
    /// <param name="color">color</param>
    /// <remarks>
    /// The color is specified in the same way as in <see cref="CairoContext.SetSourceRgba(double, double, double, double)"/>.
    /// <para>
    /// Valid values for corner_num are from 0 to 3 and identify the corners as explained in <see cref="Mesh()"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="cornerIndex"/> is not in the range [0, 3].
    /// </exception>
    /// <exception cref="CairoException"> If pattern has no current patch</exception>
    public void SetCornerColor(int cornerIndex, Color color)
        => this.SetCornerColorRgba(cornerIndex, color.Red, color.Green, color.Blue, color.Alpha);

    /// <summary>
    /// Gets the number of patches specified in the given mesh pattern.
    /// </summary>
    /// <remarks>
    /// The number only includes patches which have been finished by calling <see cref="EndPatch"/>.
    /// For example it will be 0 during the definition of the first patch.
    /// </remarks>
    public int PatchCount
    {
        get
        {
            this.CheckDisposed();

            Status status = cairo_mesh_pattern_get_patch_count(this.Handle, out uint count);

            status.ThrowIfStatus(Status.PatternTypeMismatch);

            return (int)count;
        }
    }

    /// <summary>
    /// Gets path defining the patch <paramref name="patchIndex"/> for a mesh pattern.
    /// </summary>
    /// <param name="patchIndex">the patch number to return data for</param>
    /// <returns>the path defining the patch</returns>
    /// <remarks>
    /// <paramref name="patchIndex"/> can range from 0 to n-1 where n is the number returned by <see cref="PatchCount"/>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="patchIndex"/> is not in the range [0, n).
    /// </exception>
    public Path.Path GetPath(int patchIndex)
    {
        this.CheckDisposed();

        PathRaw* path = cairo_mesh_pattern_get_path(this.Handle, (uint)patchIndex);

        Status status = this.Status;

        if (status == Status.InvalidIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(patchIndex), $"Index must be in the range [0, {this.PatchCount})");
        }

        status.ThrowIfStatus(Status.InvalidMeshConstruction);

        return new Path.Path(path);
    }

    /// <summary>
    /// Gets the control point <paramref name="pointIndex"/> of patch <paramref name="patchIndex"/>
    /// for a mesh pattern.
    /// </summary>
    /// <param name="patchIndex">the patch number to return data for</param>
    /// <param name="pointIndex">the control point number to return data for</param>
    /// <param name="point">the control point</param>
    /// <returns><c>true</c> on success, <c>false</c> otherwise</returns>
    /// <remarks>
    /// <paramref name="patchIndex"/> can range from 0 to n-1 where n is the number returned by
    /// <see cref="PatchCount"/>.
    /// <para>
    /// Valid values for <paramref name="pointIndex"/> are from 0 to 3 and identify the control points
    /// as explained in <see cref="Mesh()"/>.
    /// </para>
    /// </remarks>
    public bool TryGetControlPoint(int patchIndex, int pointIndex, out PointD point)
    {
        this.CheckDisposed();

        double x, y;

        double* xNative = &x;
        double* yNative = &y;

        Status status = cairo_mesh_pattern_get_control_point(this.Handle, (uint)patchIndex, (uint)pointIndex, xNative, yNative);

        if (status == Status.InvalidIndex || xNative is null || yNative is null)
        {
            point = default;
            return false;
        }

        point = new PointD(x, y);
        return true;
    }

    /// <summary>
    /// Gets the color information in corner <paramref name="cornerIndex"/> of patch <paramref name="patchIndex"/>
    /// for a mesh pattern.
    /// </summary>
    /// <param name="patchIndex">the patch number to return data for</param>
    /// <param name="cornerIndex">the corner number to return data for</param>
    /// <param name="color">the color</param>
    /// <returns><c>true</c> on success, <c>false</c> otherwise</returns>
    /// <remarks>
    /// <paramref name="patchIndex"/> can range from 0 to n-1 where n is the number returned by <see cref="PatchCount"/>.
    /// <para>
    /// Valid values for <paramref name="cornerIndex"/> are from 0 to 3 and identify the corners as explained in <see cref="Mesh()"/>.
    /// </para>
    /// <para>
    /// Note that the color and alpha values are not premultiplied.
    /// </para>
    /// </remarks>
    public bool TryGetCornerColor(int patchIndex, int cornerIndex, out Color color)
    {
        this.CheckDisposed();

        double red, green, blue, alpha;

        double* redNative   = &red;
        double* greenNative = &green;
        double* blueNative  = &blue;
        double* alphaNative = &alpha;

        Status status = cairo_mesh_pattern_get_corner_color_rgba(this.Handle, (uint)patchIndex, (uint)cornerIndex, redNative, greenNative, blueNative, alphaNative);

        if (status == Status.InvalidIndex || redNative is null || greenNative is null || blueNative is null || alphaNative is null)
        {
            color = default;
            return false;
        }

        color = new Color(red, green, blue, alpha);
        return true;
    }
}

/// <summary>
/// A helper type for <see cref="Mesh.BeginPatch"/> / <see cref="Mesh.EndPatch"/>.
/// </summary>
public ref struct PatchScope
{
    private Mesh? _mesh;

    internal PatchScope(Mesh mesh) => _mesh = mesh;

    public void Dispose()
    {
        _mesh?.EndPatch();
        _mesh = null;
    }
}
