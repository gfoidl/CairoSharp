// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Shapes;

/// <summary>
/// A base implementation of a <see cref="Shape"/> that is centered at (0, 0).
/// </summary>
/// <remarks>
/// The <see cref="Drawing.Path.Path"/> of the <see cref="Shape"/> is stored internally,
/// and can be re-used by drawing operations. So when done using the <see cref="Shape"/>
/// finish it by calling <see cref="Dispose"/>.
/// <para>
/// One-off usage of <see cref="Shape"/> may be too heavy, but for tesselation tasks it may
/// be ideal, as all the computation and layout of the <see cref="Shape"/> needs only be done
/// once, then can be re-used.
/// </para>
/// </remarks>
public abstract class Shape(CairoContext cr) : IDisposable
{
    private readonly CairoContext _cr = cr ?? throw new ArgumentNullException(nameof(cr));
    private Path?                 _path;

    public void Dispose()
    {
        _path?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The <see cref="Drawing.Path.Path"/> for this <see cref="Shape"/>.
    /// </summary>
    protected internal Path Path => _path ??= this.GetPath();

    /// <summary>
    /// Draws the oultine of the shape.
    /// </summary>
    /// <param name="centerX">The x-coordinate of the center</param>
    /// <param name="centerY">The y-coordinate of the center</param>
    /// <param name="stroke">
    /// When <c>true</c> calls <see cref="CairoContext.Stroke"/> after
    /// appending the path for the shape. When <c>false</c> doesn't call stroke.
    /// </param>
    public void Draw(double centerX, double centerY, bool stroke = true)
    {
        if (_cr.HasCurrentPoint)
        {
            _cr.NewSubPath();
        }

        using (_cr.Save())
        {
            _cr.Translate(centerX, centerY);
            _cr.AppendPath(this.Path);

            if (stroke)
            {
                _cr.Stroke();
            }
        }
    }

    /// <summary>
    /// Draws the oultine of the shape with the given <see cref="Color" />.
    /// </summary>
    /// <param name="centerX">The x-coordinate of the center</param>
    /// <param name="centerY">The y-coordinate of the center</param>
    /// <param name="color">
    /// The colour of the outline. Components must be given in the range [0,1]
    /// </param>
    /// <param name="stroke">
    /// When <c>true</c> calls <see cref="CairoContext.Stroke"/> after
    /// appending the path for the shape. When <c>false</c> doesn't call stroke.
    /// </param>
    public void Draw(double centerX, double centerY, Color color, bool stroke = true)
    {
        cr.Color = color;
        this.Draw(centerX, centerY, stroke);
    }

    /// <summary>
    /// Fills the shape.
    /// </summary>
    /// <param name="centerX">The x-coordinate of the center</param>
    /// <param name="centerY">The y-coordinate of the center</param>
    /// <param name="fill">
    /// When <c>true</c> calls <see cref="CairoContext.Fill"/> after
    /// appending the path for the shape. When <c>false</c> doesn't call fill.
    /// </param>
    public void Fill(double centerX, double centerY, bool fill = true)
    {
        if (_cr.HasCurrentPoint)
        {
            _cr.NewSubPath();
        }

        using (_cr.Save())
        {
            _cr.Translate(centerX, centerY);
            _cr.AppendPath(this.Path);

            if (fill)
            {
                _cr.Fill();
            }
        }
    }

    /// <summary>
    /// Fills the shape with the given <see cref="Color" />.
    /// </summary>
    /// <param name="centerX">The x-coordinate of the center</param>
    /// <param name="centerY">The y-coordinate of the center</param>
    /// <param name="color">
    /// The colour of the fill. Components must be given in the range [0,1]
    /// </param>
    /// <param name="fill">
    /// When <c>true</c> calls <see cref="CairoContext.Fill"/> after
    /// appending the path for the shape. When <c>false</c> doesn't call fill.
    /// </param>
    public void Fill(double centerX, double centerY, Color color, bool fill = true)
    {
        _cr.Color = color;
        this.Fill(centerX, centerY, fill);
    }

    private Path GetPath()
    {
        if (_cr.HasCurrentPoint)
        {
            _cr.NewPath();
        }

        this.CreatePath(_cr);
        Path path = _cr.CopyPath();

        _cr.NewPath();

        return path;
    }

    /// <summary>
    /// Creates the <see cref="Drawing.Path.Path"/> for the shape.
    /// </summary>
    /// <param name="cr">The <see cref="CairoContext"/></param>
    /// <remarks>
    /// When this method is called, <see cref="CairoContext"/> has no current point.
    /// After this method the current point will be cleared too.
    /// </remarks>
    protected internal abstract void CreatePath(CairoContext cr);
}
