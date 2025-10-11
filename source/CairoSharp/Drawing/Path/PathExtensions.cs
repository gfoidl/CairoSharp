// (c) gfoidl, all rights reserved

using Cairo.Drawing.Path;
using Cairo.Drawing.Text;
using static Cairo.Drawing.Path.PathNative;

namespace Cairo;

/// <summary>
/// Creating <see cref="Path"/>es for <see cref="CairoContext"/>.
/// </summary>
public static unsafe class PathExtensions
{
    extension(double value)
    {
        /// <summary>
        /// Converts degrees to radians. rad = deg * Pi / 180
        /// </summary>
        /// <returns>Radians.</returns>
        public double DegreesToRadians() => value * Math.PI / 180d;
    }

    extension(int value)
    {
        /// <summary>
        /// Converts degrees to radians. rad = deg * Pi / 180
        /// </summary>
        /// <returns>Radians.</returns>
        public double DegreesToRadians() => value * Math.PI / 180d;
    }

    extension(CairoContext cr)
    {
        /// <summary>
        /// Creates a copy of the current path and returns it to the user as a <see cref="Path"/>.
        /// Use <c>foreach</c> to iterate over the returned data structure.
        /// </summary>
        /// <returns>
        /// the copy of the current path. The caller owns the returned object and should call
        /// <see cref="CairoObject.Dispose()"/> when finished with it.
        /// </returns>
        /// <remarks>
        /// This method will always return a valid pointer, but the result will have no data
        /// (data==NULL and num_data==0), if either of the following conditions hold:
        /// <list type="number">
        /// <item>
        /// If there is insufficient memory to copy the path. In this case path->status will be set to
        /// <see cref="Status.NoMemory"/>.
        /// </item>
        /// <item>
        /// If cr is already in an error state. In this case path->status will contain the same status that would
        /// be returned by <see cref="CairoContext.Status"/>.
        /// </item>
        /// </list>
        /// </remarks>
        public Path CopyPath()
        {
            cr.CheckDisposed();

            PathRaw* handle = cairo_copy_path(cr.Handle);
            return new Path(handle);
        }

        /// <summary>
        /// Gets a flattened copy of the current path and returns it to the user as a <see cref="Path"/>.
        /// Use <c>foreach</c> to iterate over the returned data structure.
        /// </summary>
        /// <returns>
        /// the copy of the current path. The caller owns the returned object and should call
        /// <see cref="CairoObject.Dispose()"/> when finished with it.
        /// </returns>
        /// <remarks>
        /// This method is like <see cref="CopyPath(CairoContext)"/> except that any curves in the path
        /// will be approximated with piecewise-linear approximations, (accurate to within the current
        /// tolerance value). That is, the result is guaranteed to not have any elements of type
        /// <see cref="DataType.CurveTo"/> which will instead be replaced by a series of <see cref="DataType.LineTo"/>
        /// elements.
        /// <para>
        /// This method will always return a valid pointer, but the result will have no data
        /// (data==NULL and num_data==0), if either of the following conditions hold:
        /// <list type="number">
        /// <item>
        /// If there is insufficient memory to copy the path. In this case path->status will be set to
        /// <see cref="Status.NoMemory"/>.
        /// </item>
        /// <item>
        /// If cr is already in an error state. In this case path->status will contain the same status that would
        /// be returned by <see cref="CairoContext.Status"/>.
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        public Path CopyPathFlat()
        {
            cr.CheckDisposed();

            PathRaw* handle = cairo_copy_path_flat(cr.Handle);
            return new Path(handle);
        }

        /// <summary>
        /// Append the path onto the current path
        /// </summary>
        /// <param name="path">path to be appended</param>
        /// <remarks>
        /// The path may be either the return value from one of <see cref="CopyPath(CairoContext)"/> or
        /// <see cref="CopyPathFlat(CairoContext)"/> or it may be constructed manually. See <see cref="Path"/>
        /// for details on how the path data structure should be initialized, and note that path->status must
        /// be initialized to <see cref="Status.Success"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <c>null</c></exception>
        public void AppendPath(Path path)
        {
            cr.CheckDisposed();
            ArgumentNullException.ThrowIfNull(path);

            cairo_append_path(cr.Handle, (PathRaw*)path.Handle);
        }

        /// <summary>
        /// Returns whether a current point is defined on the current path. See <see cref="get_CurrentPoint(CairoContext)"/>
        /// for details on the current point.
        /// </summary>
        public bool HasCurrentPoint
        {
            get
            {
                cr.CheckDisposed();
                return cairo_has_current_point(cr.Handle);
            }
        }

        /// <summary>
        /// Gets the current point of the current path, which is conceptually the final point
        /// reached by the path so far.
        /// </summary>
        /// <remarks>
        /// The current point is returned in the user-space coordinate system. If there is no defined current
        /// point or if cr is in an error status, x and y will both be set to 0.0. It is possible to check this
        /// in advance with <see cref="get_HasCurrentPoint(CairoContext)"/>.
        /// <para>
        /// Most path construction functions alter the current point. See the following for details on how they
        /// affect the current point: cairo_new_path(), cairo_new_sub_path(), cairo_append_path(), cairo_close_path(),
        /// cairo_move_to(), cairo_line_to(), cairo_curve_to(), cairo_rel_move_to(), cairo_rel_line_to(),
        /// cairo_rel_curve_to(), cairo_arc(), cairo_arc_negative(), cairo_rectangle(), cairo_text_path(),
        /// cairo_glyph_path().
        /// </para>
        /// <para>
        /// Some functions use and alter the current point but do not otherwise change current path: cairo_show_text().
        /// </para>
        /// <para>
        /// Some functions unset the current path and as a result, current point: cairo_fill(), cairo_stroke().
        /// </para>
        /// </remarks>
        public PointD CurrentPoint
        {
            get
            {
                cr.CheckDisposed();

                cairo_get_current_point(cr.Handle, out double x, out double y);
                return new PointD(x, y);
            }
        }

        /// <summary>
        /// Clears the current path. After this call there will be no path and no current point.
        /// </summary>
        public void NewPath()
        {
            cr.CheckDisposed();
            cairo_new_path(cr.Handle);
        }

        /// <summary>
        /// Begin a new sub-path. Note that the existing path is not affected. After this call there
        /// will be no current point.
        /// </summary>
        /// <remarks>
        /// In many cases, this call is not needed since new sub-paths are frequently started with <see cref="MoveTo(CairoContext, double, double)"/>.
        /// <para>
        /// A call to <see cref="NewSubPath(CairoContext)"/> is particularly useful when beginning a
        /// new sub-path with one of the <see cref="Arc(CairoContext, PointD, double, double, double)"/> calls. This makes things easier
        /// as it is no longer necessary to manually compute the arc's initial coordinates for a call
        /// to <see cref="MoveTo(CairoContext, PointD)"/>.
        /// </para>
        /// </remarks>
        public void NewSubPath()
        {
            cr.CheckDisposed();
            cairo_new_sub_path(cr.Handle);
        }

        /// <summary>
        /// Adds a line segment to the path from the current point to the beginning of the current sub-path,
        /// (the most recent point passed to <see cref="MoveTo(CairoContext, PointD)"/>), and closes this sub-path.
        /// After this call the current point will be at the joined endpoint of the sub-path.
        /// </summary>
        /// <remarks>
        /// The behavior of <see cref="ClosePath(CairoContext)"/> is distinct from simply calling <see cref="LineTo(CairoContext, PointD)"/>
        /// with the equivalent coordinate in the case of stroking. When a closed sub-path is stroked, there are no caps on
        /// the ends of the sub-path. Instead, there is a line join connecting the final and initial segments
        /// of the sub-path.
        /// <para>
        /// If there is no current point before the call to <see cref="ClosePath(CairoContext)"/>, this method
        /// will have no effect.
        /// </para>
        /// <para>
        /// Note: As of cairo version 1.2.4 any call to <see cref="ClosePath(CairoContext)"/> will place an
        /// explicit MOVE_TO element into the path immediately after the CLOSE_PATH element,
        /// (which can be seen in <see cref="CopyPath(CairoContext)"/> for example). This can simplify path
        /// processing in some cases as it may not be necessary to save the "last move_to point" during
        /// processing as the MOVE_TO immediately after the CLOSE_PATH will provide that point.
        /// </para>
        /// </remarks>
        public void ClosePath()
        {
            cr.CheckDisposed();
            cairo_close_path(cr.Handle);
        }

        /// <summary>
        /// Adds a circular arc of the given radius to the current path. The arc is centered at (xc , yc ),
        /// begins at angle1 and proceeds in the direction of increasing angles to end at angle2.
        /// If angle2 is less than angle1 it will be progressively increased by 2*M_PI until it is
        /// greater than angle1.
        /// </summary>
        /// <param name="xc">X position of the center of the arc</param>
        /// <param name="yc">Y position of the center of the arc</param>
        /// <param name="radius">the radius of the arc</param>
        /// <param name="angle1">the start angle, in radians</param>
        /// <param name="angle2">the end angle, in radians</param>
        /// <remarks>
        /// If there is a current point, an initial line segment will be added to the path to connect the
        /// current point to the beginning of the arc. If this initial line is undesired, it can be avoided by
        /// calling <see cref="NewSubPath(CairoContext)"/>() before <see cref="Arc(CairoContext, double, double, double, double, double)"/>.
        /// <para>
        /// Angles are measured in radians. An angle of 0.0 is in the direction of the positive X axis
        /// (in user space). An angle of M_PI/2.0 radians (90 degrees) is in the direction of the positive
        /// Y axis (in user space). Angles increase in the direction from the positive X axis toward the
        /// positive Y axis. So with the default transformation matrix, angles increase in a clockwise direction.
        /// </para>
        /// <para>
        /// (To convert from degrees to radians, use <c>degrees * (M_PI / 180.)</c>.)
        /// </para>
        /// <para>
        /// This method gives the arc in the direction of increasing angles; see
        /// <see cref="ArcNegative(CairoContext, double, double, double, double, double)"/>
        /// to get the arc in the direction of decreasing angles.
        /// </para>
        /// <para>
        /// The arc is circular in user space. To achieve an elliptical arc, you can scale the current
        /// transformation matrix by different amounts in the X and Y directions. For example, to draw an
        /// ellipse in the box given by x , y , width , height:
        /// <code>
        /// context.Save();
        /// context.Translate(x + width / 2, y + height / 2);
        /// context.Scale(width / 2, height / 2);
        /// context.Arc(0, 0, 1, 0, 2 * Math.Pi);
        /// context.Restore();
        /// </code>
        /// </para>
        /// </remarks>
        public void Arc(double xc, double yc, double radius, double angle1, double angle2)
        {
            cr.CheckDisposed();
            cairo_arc(cr.Handle, xc, yc, radius, angle1, angle2);
        }

        /// <summary>
        /// Adds a circular arc of the given radius to the current path. The arc is centered at (xc , yc ),
        /// begins at angle1 and proceeds in the direction of increasing angles to end at angle2.
        /// If angle2 is less than angle1 it will be progressively increased by 2*M_PI until it is
        /// greater than angle1.
        /// </summary>
        /// <param name="center">position of the center of the arc</param>
        /// <param name="radius">the radius of the arc</param>
        /// <param name="angle1">the start angle, in radians</param>
        /// <param name="angle2">the end angle, in radians</param>
        /// <remarks>
        /// See <see cref="Arc(CairoContext, double, double, double, double, double)"/> for more details.
        /// </remarks>
        public void Arc(PointD center, double radius, double angle1, double angle2)
            => Arc(cr, center.X, center.Y, radius, angle1, angle2);

        /// <summary>
        /// Adds a circular arc of the given radius to the current path. The arc is centered at (xc , yc ),
        /// begins at angle1 and proceeds in the direction of decreasing angles to end at angle2.
        /// If angle2 is greater than angle1 it will be progressively decreased by 2*M_PI until it is
        /// less than angle1.
        /// </summary>
        /// <param name="xc">X position of the center of the arc</param>
        /// <param name="yc">Y position of the center of the arc</param>
        /// <param name="radius">the radius of the arc</param>
        /// <param name="angle1">the start angle, in radians</param>
        /// <param name="angle2">the end angle, in radians</param>
        /// <remarks>
        /// See <see cref="Arc(CairoContext, double, double, double, double, double)"/> for more details.
        /// This method differs only in the direction of the arc between the two angles.
        /// </remarks>
        public void ArcNegative(double xc, double yc, double radius, double angle1, double angle2)
        {
            cr.CheckDisposed();
            cairo_arc_negative(cr.Handle, xc, yc, radius, angle1, angle2);
        }

        /// <summary>
        /// Adds a circular arc of the given radius to the current path. The arc is centered at (xc , yc ),
        /// begins at angle1 and proceeds in the direction of decreasing angles to end at angle2.
        /// If angle2 is greater than angle1 it will be progressively decreased by 2*M_PI until it is
        /// less than angle1.
        /// </summary>
        /// <param name="center">position of the center of the arc</param>
        /// <param name="radius">the radius of the arc</param>
        /// <param name="angle1">the start angle, in radians</param>
        /// <param name="angle2">the end angle, in radians</param>
        /// <remarks>
        /// See <see cref="Arc(CairoContext, double, double, double, double, double)"/> for more details.
        /// </remarks>
        public void ArcNegative(PointD center, double radius, double angle1, double angle2)
            => ArcNegative(cr, center.X, center.Y, radius, angle1, angle2);

        /// <summary>
        /// Adds a cubic Bézier spline to the path from the current point to position (x3 , y3 )
        /// in user-space coordinates, using (x1 , y1 ) and (x2 , y2 ) as the control points.
        /// After this call the current point will be (x3 , y3 ).
        /// </summary>
        /// <param name="x1">the X coordinate of the first control point</param>
        /// <param name="y1">the Y coordinate of the first control point</param>
        /// <param name="x2">the X coordinate of the second control point</param>
        /// <param name="y2">the Y coordinate of the second control point</param>
        /// <param name="x3">the X coordinate of the end of the curve</param>
        /// <param name="y3">the Y coordinate of the end of the curve</param>
        /// <remarks>
        /// If there is no current point before the call to cairo_curve_to() this method will
        /// behave as if preceded by a call to cairo_move_to(cr , x1 , y1 ).
        /// </remarks>
        public void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            cr.CheckDisposed();
            cairo_curve_to(cr.Handle, x1, y1, x2, y2, x3, y3);
        }

        /// <summary>
        /// Adds a cubic Bézier spline to the path from the current point to position (x3 , y3 )
        /// in user-space coordinates, using (x1 , y1 ) and (x2 , y2 ) as the control points.
        /// After this call the current point will be (x3 , y3 ).
        /// </summary>
        /// <param name="p1">the first control point</param>
        /// <param name="p2">the second control point</param>
        /// <param name="p3">the end of the curve</param>
        /// <remarks>
        /// If there is no current point before the call to cairo_curve_to() this method will
        /// behave as if preceded by a call to cairo_move_to(cr , x1 , y1 ).
        /// </remarks>
        public void CurveTo(PointD p1, PointD p2, PointD p3)
            => CurveTo(cr, p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

        /// <summary>
        /// Adds a line to the path from the current point to position (x , y ) in user-space
        /// coordinates. After this call the current point will be (x , y ).
        /// </summary>
        /// <param name="x">the X coordinate of the end of the new line</param>
        /// <param name="y">the Y coordinate of the end of the new line</param>
        /// <remarks>
        /// If there is no current point before the call to cairo_line_to() this method will
        /// behave as cairo_move_to(cr , x , y ).
        /// </remarks>
        public void LineTo(double x, double y)
        {
            cr.CheckDisposed();
            cairo_line_to(cr.Handle, x, y);
        }

        /// <summary>
        /// Adds a line to the path from the current point to position (x , y ) in user-space
        /// coordinates. After this call the current point will be (x , y ).
        /// </summary>
        /// <param name="point">the end of the new line</param>
        /// <remarks>
        /// If there is no current point before the call to cairo_line_to() this method will
        /// behave as cairo_move_to(cr , x , y ).
        /// </remarks>
        public void LineTo(PointD point) => LineTo(cr, point.X, point.Y);

        /// <summary>
        /// Begin a new sub-path. After this call the current point will be (x , y ).
        /// </summary>
        /// <param name="x">the X coordinate of the new position</param>
        /// <param name="y">the Y coordinate of the new position</param>
        public void MoveTo(double x, double y)
        {
            cr.CheckDisposed();
            cairo_move_to(cr.Handle, x, y);
        }

        /// <summary>
        /// Begin a new sub-path. After this call the current point will be (x , y ).
        /// </summary>
        /// <param name="point">point of the new position</param>
        public void MoveTo(PointD point) => MoveTo(cr, point.X, point.Y);

        /// <summary>
        /// Adds a closed sub-path rectangle of the given size to the current path at
        /// position (x , y ) in user-space coordinates.
        /// </summary>
        /// <param name="x">the X coordinate of the top left corner of the rectangle</param>
        /// <param name="y">the Y coordinate to the top left corner of the rectangle</param>
        /// <param name="width">the width of the rectangle</param>
        /// <param name="height">the height of the rectangle</param>
        /// <remarks>
        /// This method is logically equivalent to:
        /// <code>
        /// context.MoveTo(x, y);
        /// context.RelLineTo(width, 0);
        /// context.RelLineTo(0, height);
        /// context.RelLineTo(-width, 0);
        /// context.ClosePath();
        /// </code>
        /// </remarks>
        public void Rectangle(double x, double y, double width, double height)
        {
            cr.CheckDisposed();
            cairo_rectangle(cr.Handle, x, y, width, height);
        }

        /// <summary>
        /// Adds a closed sub-path rectangle of the given size to the current path at
        /// position (x , y ) in user-space coordinates.
        /// </summary>
        /// <param name="rectangle">the rectangle</param>
        public void Rectangle(Rectangle rectangle) => Rectangle(cr, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        /// <summary>
        /// Adds a closed sub-path rectangle of the given size to the current path at
        /// position (x , y ) in user-space coordinates.
        /// </summary>
        /// <param name="point">the top left corner of the rectangle</param>
        /// <param name="width">the width of the rectangle</param>
        /// <param name="height">the height of the rectangle</param>
        public void Rectangle(PointD point, double width, double height) => Rectangle(cr, point.X, point.Y, width, height);

        /// <summary>
        /// Adds closed paths for the glyphs to the current path. The generated path if filled, achieves
        /// an effect similar to that of cairo_show_glyphs().
        /// </summary>
        /// <param name="glyphs">array of glyphs to show</param>
        public void GlyphPath(params ReadOnlySpan<Glyph> glyphs)
        {
            if (glyphs.IsEmpty)
            {
                return;
            }

            cr.CheckDisposed();

            fixed (Glyph* ptr = glyphs)
            {
                cairo_glyph_path(cr.Handle, ptr, glyphs.Length);
            }
        }

        /// <summary>
        /// Adds closed paths for text to the current path. The generated path if filled, achieves
        /// an effect similar to that of cairo_show_text().
        /// </summary>
        /// <param name="text">a NUL-terminated string of text encoded in UTF-8, or NULL</param>
        /// <remarks>
        /// Text conversion and positioning is done similar to cairo_show_text().
        /// <para>
        /// Like cairo_show_text(), after this call the current point is moved to the origin of where the
        /// next glyph would be placed in this same progression. That is, the current point will be at the
        /// origin of the final glyph offset by its advance values. This allows for chaining multiple calls
        /// to to cairo_text_path() without having to set current point in between.
        /// </para>
        /// <para>
        /// Note: The cairo_text_path() function call is part of what the cairo designers call the
        /// "toy" text API. It is convenient for short demos and simple programs, but it is not expected to
        /// be adequate for serious text-using applications. See <see cref="GlyphPath(CairoContext, ReadOnlySpan{Glyph})"/>
        /// for the "real" text path API in cairo.
        /// </para>
        /// </remarks>
        public void TextPath(string? text)
        {
            if (text is null)
            {
                return;
            }

            cr.CheckDisposed();
            cairo_text_path(cr.Handle, text);
        }

        /// <summary>
        /// Relative-coordinate version of cairo_curve_to(). All offsets are relative to the current
        /// point. Adds a cubic Bézier spline to the path from the current point to a point offset from
        /// the current point by (dx3 , dy3 ), using points offset by (dx1 , dy1 ) and (dx2 , dy2 ) as
        /// the control points. After this call the current point will be offset by (dx3 , dy3 ).
        /// </summary>
        /// <param name="dx1">the X offset to the first control point</param>
        /// <param name="dy1">the Y offset to the first control point</param>
        /// <param name="dx2">the X offset to the second control point</param>
        /// <param name="dy2">the Y offset to the second control point</param>
        /// <param name="dx3">the X offset to the end of the curve</param>
        /// <param name="dy3">the Y offset to the end of the curve</param>
        /// <remarks>
        /// Given a current point of (x, y), cairo_rel_curve_to(cr , dx1 , dy1 , dx2 , dy2 , dx3 , dy3 ) is
        /// logically equivalent to cairo_curve_to(cr , x+dx1 , y+dy1 , x+dx2 , y+dy2 , x+dx3 , y+dy3 ).
        /// </remarks>
        /// <exception cref="CairoException">when there is no current point</exception>
        public void RelCurveTo(double dx1, double dy1, double dx2, double dy2, double dx3, double dy3)
        {
            cr.CheckDisposed();

            cairo_rel_curve_to(cr.Handle, dx1, dy1, dx2, dy2, dx3, dy3);

            cr.Status.ThrowIfStatus(Status.NoCurrentPoint);
        }

        /// <summary>
        /// Relative-coordinate version of cairo_curve_to(). All offsets are relative to the current
        /// point. Adds a cubic Bézier spline to the path from the current point to a point offset from
        /// the current point by (dx3 , dy3 ), using points offset by (dx1 , dy1 ) and (dx2 , dy2 ) as
        /// the control points. After this call the current point will be offset by (dx3 , dy3 ).
        /// </summary>
        /// <param name="d1">the offset to the first control point</param>
        /// <param name="d2">the offset to the second control point</param>
        /// <param name="d3">the offset to the end of the curve</param>
        /// <remarks>
        /// Given a current point of (x, y), cairo_rel_curve_to(cr , dx1 , dy1 , dx2 , dy2 , dx3 , dy3 ) is
        /// logically equivalent to cairo_curve_to(cr , x+dx1 , y+dy1 , x+dx2 , y+dy2 , x+dx3 , y+dy3 ).
        /// </remarks>
        /// <exception cref="CairoException">when there is no current point</exception>
        public void RelCurveTo(Distance d1, Distance d2, Distance d3) => RelCurveTo(cr, d1.Dx, d1.Dy, d2.Dx, d2.Dy, d3.Dx, d3.Dy);

        /// <summary>
        /// Relative-coordinate version of cairo_line_to(). Adds a line to the path from the current
        /// point to a point that is offset from the current point by (dx , dy ) in user space.
        /// After this call the current point will be offset by (dx , dy ).
        /// </summary>
        /// <param name="dx">the X offset to the end of the new line</param>
        /// <param name="dy">the Y offset to the end of the new line</param>
        /// <remarks>
        /// Given a current point of (x, y), cairo_rel_line_to(cr , dx , dy ) is logically equivalent to
        /// cairo_line_to(cr , x + dx , y + dy ).
        /// </remarks>
        /// <exception cref="CairoException">when there is no current point</exception>
        public void RelLineTo(double dx, double dy)
        {
            cr.CheckDisposed();

            cairo_rel_line_to(cr.Handle, dx, dy);

            cr.Status.ThrowIfStatus(Status.NoCurrentPoint);
        }

        /// <summary>
        /// Relative-coordinate version of cairo_line_to(). Adds a line to the path from the current
        /// point to a point that is offset from the current point by (dx , dy ) in user space.
        /// After this call the current point will be offset by (dx , dy ).
        /// </summary>
        /// <param name="distance">the offset to the end of the new line</param>
        /// <remarks>
        /// Given a current point of (x, y), cairo_rel_line_to(cr , dx , dy ) is logically equivalent to
        /// cairo_line_to(cr , x + dx , y + dy ).
        /// </remarks>
        /// <exception cref="CairoException">when there is no current point</exception>
        public void RelLineTo(Distance distance) => RelLineTo(cr, distance.Dx, distance.Dy);

        /// <summary>
        /// Begin a new sub-path. After this call the current point will offset by (x , y ).
        /// </summary>
        /// <param name="dx">the X offset</param>
        /// <param name="dy">the Y offset</param>
        /// <remarks>
        /// Given a current point of (x, y), cairo_rel_move_to(cr , dx , dy ) is logically equivalent
        /// to cairo_move_to(cr , x + dx , y + dy ).
        /// </remarks>
        /// <exception cref="CairoException">when there is no current point</exception>
        public void RelMoveTo(double dx, double dy)
        {
            cr.CheckDisposed();

            cairo_rel_move_to(cr.Handle, dx, dy);

            cr.Status.ThrowIfStatus(Status.NoCurrentPoint);
        }

        /// <summary>
        /// Begin a new sub-path. After this call the current point will offset by (x , y ).
        /// </summary>
        /// <param name="distance">the offset</param>
        /// <remarks>
        /// Given a current point of (x, y), cairo_rel_move_to(cr , dx , dy ) is logically equivalent
        /// to cairo_move_to(cr , x + dx , y + dy ).
        /// </remarks>
        /// <exception cref="CairoException">when there is no current point</exception>
        public void RelMoveTo(Distance distance) => RelMoveTo(cr, distance.Dx, distance.Dy);

        /// <summary>
        /// Computes a bounding box in user-space coordinates covering the points on the current
        /// path. If the current path is empty, returns an empty rectangle ((0,0), (0,0)).
        /// Stroke parameters, fill rule, surface dimensions and clipping are not taken into account.
        /// </summary>
        /// <param name="x1">left of the resulting extents</param>
        /// <param name="y1">top of the resulting extents</param>
        /// <param name="x2">right of the resulting extents</param>
        /// <param name="y2">bottom of the resulting extents</param>
        /// <remarks>
        /// Contrast with cairo_fill_extents() and cairo_stroke_extents() which return the extents
        /// of only the area that would be "inked" by the corresponding drawing operations.
        /// <para>
        /// The result of cairo_path_extents() is defined as equivalent to the limit of cairo_stroke_extents()
        /// with <see cref="Drawing.LineCap.Round"/> as the line width approaches 0.0, (but never reaching
        /// the empty-rectangle returned by cairo_stroke_extents() for a line width of 0.0).
        /// </para>
        /// <para>
        /// Specifically, this means that zero-area sub-paths such as cairo_move_to();cairo_line_to()
        /// segments, (even degenerate cases where the coordinates to both calls are identical),
        /// will be considered as contributing to the extents. However, a lone cairo_move_to() will
        /// not contribute to the results of cairo_path_extents().
        /// </para>
        /// </remarks>
        public void PathExtents(out double x1, out double y1, out double x2, out double y2)
        {
            cr.CheckDisposed();
            cairo_path_extents(cr.Handle, out x1, out y1, out x2, out y2);
        }
    }
}
