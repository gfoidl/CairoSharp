// (c) gfoidl, all rights reserved

using static Cairo.Drawing.Patterns.PatternNative;

namespace Cairo.Drawing.Patterns;

/// <summary>
/// A <see cref="Pattern"/> with a single color.
/// </summary>
public sealed unsafe class SolidPattern : Pattern
{
    internal SolidPattern(void* handle, bool isOwnedByCairo, bool needsDestroy = true)
        : base(handle, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Creates a new <see cref="SolidPattern"/> corresponding to an opaque color. The color
    /// components are floating point numbers in the range 0 to 1. If the values passed in
    /// are outside that range, they will be clamped.
    /// </summary>
    /// <param name="red">red component of the color</param>
    /// <param name="green">green component of the color</param>
    /// <param name="blue">blue component of the color</param>
    public SolidPattern(double red, double green, double blue)
        : base(cairo_pattern_create_rgb(red, green, blue)) { }

    /// <summary>
    /// Creates a new <see cref="SolidPattern"/> corresponding to a translucent color. The color
    /// components are floating point numbers in the range 0 to 1. If the values passed in
    /// are outside that range, they will be clamped.
    /// </summary>
    /// <param name="red">red component of the color</param>
    /// <param name="green">green component of the color</param>
    /// <param name="blue">blue component of the color</param>
    /// <param name="alpha">alpha component of the color</param>
    public SolidPattern(double red, double green, double blue, double alpha)
        : base(cairo_pattern_create_rgba(red, green, blue, alpha)) { }

    /// <summary>
    /// Creates a new <see cref="SolidPattern"/> corresponding to a translucent color.
    /// </summary>
    /// <param name="color">the color</param>
    public SolidPattern(Color color) : this(color.Red, color.Green, color.Blue, color.Alpha) { }

    /// <summary>
    /// Creates a new <see cref="SolidPattern"/> corresponding to a color.
    /// </summary>
    /// <param name="color">the color</param>
    /// <param name="ignoreAlpha">
    /// When <c>true</c> only the opaque color is used (same as <see cref="SolidPattern(double, double, double)"/>),
    /// when <c>false</c> the translucent color is used (same as <see cref="SolidPattern(double, double, double, double)"/>.
    /// </param>
    public SolidPattern(Color color, bool ignoreAlpha)
        : base(ignoreAlpha
            ? cairo_pattern_create_rgb(color.Red, color.Green, color.Blue)
            : cairo_pattern_create_rgba(color.Red, color.Green, color.Blue, color.Alpha))
    { }

    /// <summary>
    /// Gets the solid color for a solid color pattern.
    /// </summary>
    /// <remarks>
    /// Note that the color and alpha values are not premultiplied.
    /// </remarks>
    public Color Color
    {
        get
        {
            this.CheckDisposed();

            double red, green, blue, alpha;

            double* redNative   = &red;
            double* greenNative = &green;
            double* blueNative  = &blue;
            double* alphaNative = &alpha;

            cairo_pattern_get_rgba(this.Handle, redNative, greenNative, blueNative, alphaNative);

            this.Status.ThrowIfStatus(Status.PatternTypeMismatch);

            if (redNative is null || greenNative is null || blueNative is null || alphaNative is null)
            {
                throw new InvalidOperationException();
            }

            return new Color(red, green, blue, alpha);
        }
    }
}
