// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Shapes;

/// <summary>
/// A hexagon shape.
/// </summary>
public sealed class Hexagon : Shape
{
    private static readonly double s_sqrt3Inv = 1d / Math.Sqrt(3d);

    /// <summary>
    /// The inradius of the hexagon.
    /// </summary>
    public double Inradius { get; }

    /// <summary>
    /// The circumradius of the hexagon.
    /// </summary>
    public double Circumradius { get; }

    /// <summary>
    /// Creates a hexagon with given inradius.
    /// </summary>
    /// <param name="cr">The <see cref="CairoContext"/></param>
    /// <param name="inradius">The inradius of the hexagon</param>
    /// <remarks>
    /// When <paramref name="inradius" /> is negative, the absolute value
    /// gets used, no exception is thrown.
    /// </remarks>
    public Hexagon(CairoContext cr, double inradius) : base(cr)
    {
        // Anpassen der Seitenlänge damit der Abstand zwischen 2 Sechsecken 2.ri beträgt.
        // ri...Inkreisradius, a = ru...Kantenlänge
        // Herleitung über: 2.ri = a.sqrt(3),

        inradius          = Math.Abs(inradius);
        this.Inradius     = inradius;
        this.Circumradius = 2 * inradius * s_sqrt3Inv;
    }

    protected internal override void CreatePath(CairoContext cr)
    {
        double ri   = this.Inradius;
        double ru   = this.Circumradius;
        double a    = ru;
        double aBy2 = a * 0.5;

        // Hexagon points. Above center first, then clockwise.
        cr.MoveTo   (0  , -ru);
        cr.LineTo   (ri , -aBy2);
        cr.RelLineTo(0  , a);
        cr.LineTo   (0  , ru);
        cr.LineTo   (-ri, aBy2);
        cr.RelLineTo(0  , -a);
        cr.ClosePath();
    }
}
