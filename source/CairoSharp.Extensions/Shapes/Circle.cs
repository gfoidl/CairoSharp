// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Shapes;

/// <summary>
/// A circle shape.
/// </summary>
public sealed class Circle : Shape
{
    /// <summary>
    /// Radius of the cirlce.
    /// </summary>
    public double Radius { get; }

    /// <summary>Creates a circle with given radius</summary>
    /// <param name="cr">The <see cref="CairoContext"/></param>
    /// <param name="radius">The radius</param>
    /// <remarks>
    /// When <paramref name="radius" /> is negative, the absolute value
    /// gets used, no exception is thrown.
    /// </remarks>
    public Circle(CairoContext cr, double radius) : base(cr)
        => this.Radius = Math.Abs(radius);

    protected internal override void CreatePath(CairoContext cr)
    {
        cr.Arc(0, 0, this.Radius, 0, Math.Tau);
    }
}
