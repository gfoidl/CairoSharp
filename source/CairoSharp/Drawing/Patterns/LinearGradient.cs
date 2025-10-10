// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using static Cairo.Drawing.Patterns.PatternNative;

namespace Cairo.Drawing.Patterns;

/// <summary>
/// A linear gradient <see cref="Pattern"/>.
/// </summary>
public sealed unsafe class LinearGradient : Gradient
{
    internal LinearGradient(void* handle, bool isOwnedByCairo, bool needsDestroy = true)
        : base(handle, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Create a new linear gradient <see cref="Pattern"/> along the line defined by (x0, y0) and (x1, y1).
    /// Before using the gradient pattern, a number of color stops should be defined using
    /// <see cref="Gradient.AddColorStop(double, Color)"/> or one of the other overloads available.
    /// </summary>
    /// <param name="x0">x coordinate of the start point</param>
    /// <param name="y0">y coordinate of the start point</param>
    /// <param name="x1">x coordinate of the end point</param>
    /// <param name="y1">y coordinate of the end point</param>
    /// <remarks>
    /// Note: The coordinates here are in pattern space. For a new pattern, pattern space is identical
    /// to user space, but the relationship between the spaces can be changed with <see cref="Pattern.SetMatrix(ref Matrix)"/>.
    /// </remarks>
    public LinearGradient(double x0, double y0, double x1, double y1) : base(cairo_pattern_create_linear(x0, y0, x1, y1)) { }

    /// <summary>
    /// Create a new linear gradient <see cref="Pattern"/> along the line defined by <paramref name="point0"/>
    /// and <paramref name="point1"/>.
    /// Before using the gradient pattern, a number of color stops should be defined using
    /// <see cref="Gradient.AddColorStop(double, Color)"/> or one of the other overloads available.
    /// </summary>
    /// <param name="point0">coordinate of the start point</param>
    /// <param name="point1">coordinate of the end point</param>
    /// <remarks>
    /// Note: The coordinates here are in pattern space. For a new pattern, pattern space is identical
    /// to user space, but the relationship between the spaces can be changed with <see cref="Pattern.SetMatrix(ref Matrix)"/>.
    /// </remarks>
    public LinearGradient(PointD point0, PointD point1) : this(point0.X, point0.Y, point1.X, point1.Y) { }

    /// <summary>
    /// Gets the gradient endpoints for a linear gradient.
    /// </summary>
    public LinearPoints LinearPoints
    {
        get
        {
            this.CheckDisposed();

            double x0, y0, x1, y1;

            double* x0Native = &x0;
            double* y0Native = &y0;
            double* x1Native = &x1;
            double* y1Native = &y1;

            Status status = cairo_pattern_get_linear_points(this.Handle, x0Native, y0Native, x1Native, y1Native);

            status.ThrowIfStatus(Status.PatternTypeMismatch);

            if (x0Native is null || y0Native is null || x1Native is null || y1Native is null)
            {
                throw new InvalidOperationException();
            }

            LinearPoints linearPoints = default;
            linearPoints[0]           = new PointD(x0, y0);
            linearPoints[1]           = new PointD(x1, y1);

            return linearPoints;
        }
    }
}

[InlineArray(2)]
public struct LinearPoints
{
    private PointD _point;
}
