// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;

namespace Cairo.Extensions.Arrows;

/// <summary>
/// An arrow or vector.
/// </summary>
public class Arrow
{
    [InlineArray(3)]
    protected struct ArrowPoints
    {
        private PointD _item;
    }

    private readonly CairoContext _cr;

    /// <summary>
    /// Length of the arrow head.
    /// </summary>
    protected readonly double _length;

    /// <summary>
    /// Half arrow head opening angle in radians.
    /// </summary>
    protected readonly double _angle;

    /// <summary>
    /// The <see cref="PointD"/> constructing the arrow head.
    /// </summary>
    /// <remarks>
    /// The head is at (0,0) and the ends at (-l, l.tan(a)) respectively (-l, -l.tan(a)).
    /// Therefore the points have to be brought by transformation to the correct place.
    /// </remarks>
    protected ArrowPoints _arrowPoints;     // mutable struct

    /// <summary>
    /// Creates a new <see cref="Arrow"/>.
    /// </summary>
    /// <param name="cr">the <see cref="CairoContext"/></param>
    /// <param name="length">length of the arrow head</param>
    /// <param name="angleInDegrees">half arrow head opening angle in degrees</param>
    /// <remarks>
    /// The absolut values are used, no exception will be thrown if either <paramref name="length"/>
    /// or <paramref name="angleInDegrees"/> is negative.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="cr"/> is <c>null</c></exception>
    public Arrow(CairoContext cr, double length = 0.05, double angleInDegrees = 10)
    {
        ArgumentNullException.ThrowIfNull(cr);
        _cr = cr;

        _length = Math.Abs(length);
        _angle  = Math.Abs(angleInDegrees).DegreesToRadians();

        double x = -length;
        double y = length * Math.Tan(_angle);

        _arrowPoints[0] = new PointD(0,  0);
        _arrowPoints[1] = new PointD(x,  y);
        _arrowPoints[2] = new PointD(x, -y);
    }

    /// <summary>
    ///  Draws a vector, i.e. an arrow with arrow head at end (<paramref name="x1"/>, <paramref name="y1"/>) only.
    /// </summary>
    /// <param name="x0">x-coordinate of the start point</param>
    /// <param name="y0">y-coordinate of the start point</param>
    /// <param name="x1">x-coordinate of the end point</param>
    /// <param name="y1">y-coordinate of the end point</param>
    public void DrawVector(double x0, double y0, double x1, double y1)
        => this.DrawVector(new PointD(x0, y0), new PointD(x1, y1));

    /// <summary>
    /// Draws a vector, i.e. an arrow with arrow head at <paramref name="end"/> only.
    /// </summary>
    /// <param name="start">start point</param>
    /// <param name="end">end point</param>
    public void DrawVector(PointD start, PointD end) => this.Draw(start, end, drawHeadOnStart: false);

    /// <summary>
    /// Draws an arrow.
    /// </summary>
    /// <param name="x0">x-coordinate of the start point</param>
    /// <param name="y0">y-coordinate of the start point</param>
    /// <param name="x1">x-coordinate of the end point</param>
    /// <param name="y1">y-coordinate of the end point</param>
    public void DrawArrow(double x0, double y0, double x1, double y1)
        => this.DrawArrow(new PointD(x0, y0), new PointD(x1, y1));

    /// <summary>
    /// Draws an arrow.
    /// </summary>
    /// <param name="start">start point</param>
    /// <param name="end">end point</param>
    public void DrawArrow(PointD start, PointD end) => this.Draw(start, end, drawHeadOnStart: true);

    private void Draw(PointD start, PointD end, bool drawHeadOnStart)
    {
        double dx     = end.X - start.X;
        double dy     = end.Y - start.Y;
        double angle  = Math.Atan2(dy, dx);
        double length = Math.Sqrt(dx * dx + dy * dy);

        CairoContext cr = _cr;

        // Translate to the start point and rotate in a way that the x-axis coincide
        // with the vector. In doing so utilize that Math.Atan2 is mathematically positive,
        // whereas cairo's rotate is mathematically negative.
        using (cr.Save())
        {
            cr.Translate(start.X, start.Y);
            cr.Rotate(angle);

            // Draw the line. Keep in mind that the line is only drawn to the begin of the arrow head.
            if (drawHeadOnStart)
            {
                cr.MoveTo(_length, 0);
            }
            else
            {
                cr.MoveTo(0, 0);
            }

            cr.LineTo(length - _length, 0);
            cr.Stroke();

            // Draw arrow heads
            // Start
            if (drawHeadOnStart)
            {
                // rotate by 180°
                using (cr.Save())
                {
                    cr.Rotate(Math.PI);
                    this.DrawArrowHead(cr);
                }
            }

            // End -- translate to the endpoint
            using (cr.Save())
            {
                cr.Translate(length, 0);
                this.DrawArrowHead(cr);
            }
        }
    }

    /// <summary>
    /// Draws the arrow head.
    /// </summary>
    /// <param name="cr">the see <see cref="CairoContext"/></param>
    protected virtual void DrawArrowHead(CairoContext cr)
    {
        cr.MoveTo(_arrowPoints[0]);
        cr.LineTo(_arrowPoints[1]);
        cr.LineTo(_arrowPoints[2]);
        cr.ClosePath();

        cr.Fill();
    }
}
