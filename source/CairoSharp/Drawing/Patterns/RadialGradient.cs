// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using static Cairo.Drawing.Patterns.PatternNative;

namespace Cairo.Drawing.Patterns;

/// <summary>
/// A radial gradient <see cref="Pattern"/>.
/// </summary>
public sealed unsafe class RadialGradient : Gradient
{
    internal RadialGradient(void* handle, bool owner) : base(handle, owner) { }

    /// <summary>
    /// Creates a new radial gradient cairo_pattern_t between the two circles defined by (cx0, cy0, radius0)
    /// and (cx1, cy1, radius1). Before using the gradient pattern, a number of color stops should be
    /// defined using <see cref="Gradient.AddColorStop(double, Color)"/> or one of the other overloads available.
    /// </summary>
    /// <param name="cx0">x coordinate for the center of the start circle</param>
    /// <param name="cy0">y coordinate for the center of the start circle</param>
    /// <param name="radius0">radius of the start circle</param>
    /// <param name="cx1">x coordinate for the center of the end circle</param>
    /// <param name="cy1">y coordinate for the center of the end circle</param>
    /// <param name="radius1">radius of the end circle</param>
    public RadialGradient(double cx0, double cy0, double radius0, double cx1, double cy1, double radius1)
        : base(cairo_pattern_create_radial(cx0, cy0, radius0, cx1, cy1, radius1), owner: true) { }

    /// <summary>
    /// Gets the gradient endpoint circles for a radial gradient, each specified as a center
    /// coordinate and a radius.
    /// </summary>
    public RadialCircles RadialCircles
    {
        get
        {
            this.CheckDisposed();

            double x0, y0, radius0, x1, y1, radius1;

            double* x0Native      = &x0;
            double* y0Native      = &y0;
            double* radius0Native = &radius0;
            double* x1Native      = &x1;
            double* y1Native      = &y1;
            double* radius1Native = &radius1;

            Status status = cairo_pattern_get_radial_circles(this.Handle, x0Native, y0Native, radius0Native, x1Native, y1Native, radius1Native);

            status.ThrowIfStatus(Status.PatternTypeMismatch);

            if (x0Native is null || y0Native is null || radius0Native is null || x1Native is null || y1Native is null || radius1Native is null)
            {
                throw new InvalidOperationException();
            }

            RadialCircles radialCircles = default;
            radialCircles[0]            = new PointDWithRadius(x0, y0, radius0);
            radialCircles[1]            = new PointDWithRadius(x1, y1, radius1);

            return radialCircles;
        }
    }
}

[InlineArray(2)]
public struct RadialCircles
{
    private PointDWithRadius _point;
}
