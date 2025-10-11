// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Shapes;

/// <summary>
/// A square shape.
/// </summary>
public sealed class Square : Shape
{
    /// <summary>
    /// Inradius of the square.
    /// </summary>
    public double Inradius { get; }

    /// <summary>Creates a square with given inradius</summary>
    /// <param name="cr">The <see cref="CairoContext"/></param>
    /// <param name="inradius">The inradius of the square</param>
    /// <remarks>
    /// When <paramref name="inradius" /> is negative, the absolute value
    /// gets used, no exception is thrown.
    /// </remarks>
    public Square(CairoContext cr, double inradius) : base(cr)
        => this.Inradius = Math.Abs(inradius);

    protected internal override void CreatePath(CairoContext cr)
    {
        cr.Rectangle(-this.Inradius, -this.Inradius, 2 * this.Inradius, 2 * this.Inradius);
    }
}
