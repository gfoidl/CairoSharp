// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Shapes;

/// <summary>
/// Extensions for <see cref="CairoContext"/> and <see cref="Shapes"/>.
/// </summary>
public static class ContextExtensions
{
    extension(CairoContext cr)
    {
        /// <summary>
        /// Adds a circle to the path.
        /// </summary>
        /// <param name="xCenter">the x-coordinate of the center</param>
        /// <param name="yCenter">the y-coordinate of the center</param>
        /// <param name="radius">the radius</param>
        public void Circle(double xCenter, double yCenter, double radius)
        {
            using Circle circle = new(cr, radius);
            using (cr.Save())
            {
                cr.Translate(xCenter, yCenter);
                cr.AppendPath(circle.Path);
            }
        }

        /// <summary>
        /// Adds a circle to the path.
        /// </summary>
        /// <param name="point">the center and radius for the circle</param>
        public void Circle(PointDWithRadius point) => cr.Circle(point.X, point.Y, point.Radius);

        /// <summary>
        /// Adds a square to the path.
        /// </summary>
        /// <param name="xCenter">the x-coordinate of the center</param>
        /// <param name="yCenter">the y-coordinate of the center</param>
        /// <param name="inradius">the inradius (= half the length)</param>
        public void Square(double xCenter, double yCenter, double inradius)
        {
            using Square square = new(cr, inradius);
            using (cr.Save())
            {
                cr.Translate(xCenter, yCenter);
                cr.AppendPath(square.Path);
            }
        }

        /// <summary>
        /// Adds a hexagon to the path.
        /// </summary>
        /// <param name="xCenter">the x-coordinate of the center</param>
        /// <param name="yCenter">the y-coordinate of the center</param>
        /// <param name="inradius">the inradius (= half the length)</param>
        public void Hexagon(double xCenter, double yCenter, double inradius)
        {
            using Hexagon hexagon = new(cr, inradius);
            using (cr.Save())
            {
                cr.Translate(xCenter, yCenter);
                cr.AppendPath(hexagon.Path);
            }
        }
    }
}
