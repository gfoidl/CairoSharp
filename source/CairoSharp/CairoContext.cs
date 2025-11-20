// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Drawing;
using Cairo.Drawing.Patterns;
using Cairo.Surfaces;
using static Cairo.CairoContextNative;

namespace Cairo;

/// <summary>
/// The native (cairo) context type.
/// </summary>
public struct cairo_t;

/// <summary>
/// cairo_t — The cairo drawing context
/// </summary>
/// <remarks>
/// <see cref="CairoContext"/> is the main object used when drawing with cairo. To draw with cairo,
/// you create a <see cref="CairoContext"/>, set the target surface, and drawing options for the
/// <see cref="CairoContext"/>, create shapes with functions like <see cref="PathExtensions.MoveTo(CairoContext, PointD)"/>
/// and <see cref="PathExtensions.LineTo(CairoContext, PointD)"/>, and then draw shapes with <see cref="Stroke"/> or <see cref="Fill"/>.
/// <para>
/// <see cref="CairoContext"/>'s can be pushed to a stack via <see cref="Save"/>.
/// They may then safely be changed, without losing the current state. Use <see cref="Restore"/> to
/// restore to the saved state.
/// </para>
/// </remarks>
public sealed unsafe class CairoContext : CairoObject<cairo_t>
{
    /// <summary>
    /// Creates a new <see cref="CairoContext"/> with all graphics state parameters set to default
    /// values and with target as a target surface. The target surface should be constructed with
    /// a backend-specific function such as <see cref="Surfaces.Images.ImageSurface.ImageSurface(Format, int, int)"/>
    /// (or any other cairo_backend_surface_create() variant).
    /// </summary>
    /// <param name="target">target surface for the context</param>
    /// <remarks>
    /// This constructor references <paramref name="target"/>, so you can immediately call <see cref="CairoObject.Dispose()"/>
    /// on that surface if you don't need to maintain a separate reference to it.
    /// </remarks>
    /// <exception cref="CairoException">
    /// If you attempt to target a surface which does not support writing (such as cairo_mime_surface_t)
    /// then an exception with <see cref="Status.WriteError"/> will be raised.
    /// </exception>
    public CairoContext(Surface target) : this(CreateCore(target), isOwnedByCairo: false) { }

    [StackTraceHidden]
    private static cairo_t* CreateCore(Surface target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return cairo_create(target.Handle);
    }

    /// <summary>
    /// Creates a new <see cref="CairoContext"/> from the given native handle.
    /// </summary>
    /// <param name="cr">the native handle to a cairo context</param>
    /// <remarks>
    /// Ownership of the handle is not transferred. <see cref="CairoObject.Dispose()"/> can be called,
    /// but it will not free the native cairo context (it is actually a no-op here).
    /// </remarks>
    public CairoContext(cairo_t* cr) : base(cr, needsDestroy: false) { }

    /// <summary>
    /// Creates a new <see cref="CairoContext"/> from the given native handle.
    /// </summary>
    /// <param name="cr">the native handle to a cairo context</param>
    /// <remarks>
    /// Ownership of the handle is not transferred. <see cref="CairoObject.Dispose()"/> can be called,
    /// but it will not free the native cairo context (it is actually a no-op here).
    /// </remarks>
    public CairoContext(IntPtr cr) : base((cairo_t*)cr.ToPointer(), needsDestroy: false) { }

    internal CairoContext(cairo_t* cr, bool isOwnedByCairo, bool needsDestroy = true)
        : base(cr, isOwnedByCairo, needsDestroy)
    {
        this.Status.ThrowIfStatus(Status.NoMemory);
        this.Status.ThrowIfStatus(Status.WriteError);

        if (isOwnedByCairo && needsDestroy)
        {
            cairo_reference(cr);
        }
    }

    protected override void DisposeCore(cairo_t* cr)
    {
        cairo_destroy(this.Handle);

        PrintDebugInfo(cr);
        [Conditional("DEBUG")]
        static void PrintDebugInfo(cairo_t* cr)
        {
            uint rc = cairo_get_reference_count(cr);
            Debug.WriteLine($"CairoContext 0x{(nint)cr}: reference count = {rc}");
        }
    }

    /// <summary>
    /// The native handle to <c>cairo_t</c>
    /// </summary>
    /// <remarks>
    /// This handle can be used to pass cairo to other libraries like Pango, poppler, etc.
    /// <para>
    /// Note: this handle is owned by CairoSharp, so don't free it in anyway, otherwise
    /// undefined behavior can occur.
    /// </para>
    /// </remarks>
    public cairo_t* NativeContext => this.Handle;

    /// <summary>
    /// Checks whether an error has previously occurred for this context.
    /// </summary>
    public Status Status
    {
        get
        {
            this.CheckDisposed();
            return cairo_status(this.Handle);
        }
    }

    /// <summary>
    /// Makes a copy of the current state of cr and saves it on an internal stack
    /// of saved states for cr.
    /// </summary>
    /// <returns>
    /// A <see cref="SaveScope"/>, which when <see cref="SaveScope.Dispose"/>d calls
    /// <see cref="Restore"/>.
    /// <para>
    /// So instead of writing code like
    /// <code>
    /// cr.Save();
    /// // ...
    /// cr.Restore();
    /// </code>
    /// one can write
    /// <code>
    /// using (cr.Save())
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </para>
    /// </returns>
    /// <remarks>
    /// When <see cref="Restore"/> is called, cr will be restored to the saved state.
    /// Multiple calls to <see cref="Save"/> and <see cref="Restore"/> can be nested; each call to
    /// <see cref="Restore"/> restores the state from the matching paired <see cref="Save"/>.
    /// <para>
    /// It isn't necessary to clear all saved states before a <see cref="CairoContext"/> is freed.
    /// If the reference count of a <see cref="CairoContext"/> drops to zero in response to a call
    /// to <see cref="CairoObject.Dispose()"/>, any saved states will be freed along with the
    /// <see cref="CairoContext"/>.
    /// </para>
    /// </remarks>
    public SaveScope Save()
    {
        this.CheckDisposed();
        cairo_save(this.Handle);

        return new SaveScope(this);
    }

    /// <summary>
    /// Restores cr to the state saved by a preceding call to <see cref="Save"/> and removes that
    /// state from the stack of saved states.
    /// </summary>
    public void Restore()
    {
        this.CheckDisposed();
        cairo_restore(this.Handle);
    }

    /// <summary>
    /// Gets the target surface for the cairo context as passed to <see cref="CairoContext(Surface)"/>.
    /// </summary>
    /// <remarks>
    /// This property will always return a valid pointer, but the result can be a "nil" surface if cr
    /// is already in an error state, (ie. <see cref="Status"/> != <see cref="Status.Success"/>). A nil
    /// surface is indicated by <see cref="Surface.Status"/> != <see cref="Status.Success"/>
    /// (or by the convenience <see cref="Surface.IsNilSurface"/> property).
    /// <para>
    /// This object is owned by cairo.
    /// </para>
    /// </remarks>
    public Surface Target
    {
        get
        {
            this.CheckDisposed();
            return Surface.Lookup(cairo_get_target(this.Handle), isOwnedByCairo: true, needsDestroy: false);
        }
    }

    /// <summary>
    /// Temporarily redirects drawing to an intermediate surface known as a group.
    /// </summary>
    /// <remarks>
    /// The redirection lasts until the group is completed by a call to <see cref="PopGroup"/> or
    /// <see cref="PopGroupToSource"/>. These calls provide the result of any drawing to the group as
    /// a pattern, (either as an explicit object, or set as the source pattern).
    /// <para>
    /// This group functionality can be convenient for performing intermediate compositing. One
    /// common use of a group is to render objects as opaque within the group, (so that they
    /// occlude each other), and then blend the result with translucence onto the destination.
    /// </para>
    /// <para>
    /// Groups can be nested arbitrarily deep by making balanced calls to <see cref="PushGroup()"/> / <see cref="PopGroup"/>.
    /// Each call pushes / pops the new target group onto / from a stack.
    /// </para>
    /// <para>
    /// The <see cref="PushGroup()"/> method calls <see cref="Save"/> so that any changes to the
    /// graphics state will not be visible outside the group, (the <see cref="PopGroup"/> method call
    /// <see cref="Restore"/>).
    /// </para>
    /// <para>
    /// By default the intermediate group will have a content type of <see cref="Content.ColorAlpha"/>.
    /// Other content types can be chosen for the group by using <see cref="PushGroupWithContent"/> instead.
    /// </para>
    /// <para>
    /// As an example, here is how one might fill and stroke a path with translucence, but without
    /// any portion of the fill being visible under the stroke:
    /// <code>
    /// context.PushGroup();
    /// context.SetSource(fillPattern);
    /// context.FillPreserve();
    /// context.SetSource(strokePattern);
    /// context.Stroke();
    /// context.PopGroupToSource();
    /// context.PaintWithAlpha(alpha);
    /// </code>
    /// </para>
    /// </remarks>
    public void PushGroup()
    {
        this.CheckDisposed();
        cairo_push_group(this.Handle);
    }

    /// <summary>
    /// Temporarily redirects drawing to an intermediate surface known as a group.
    /// The redirection lasts until the group is completed by a call to <see cref="PopGroup"/> or
    /// <see cref="PopGroupToSource"/>. These calls provide the result of any drawing to the
    /// group as a pattern, (either as an explicit object, or set as the source pattern).
    /// </summary>
    /// <param name="content">a <see cref="Content"/> indicating the type of group that will be created</param>
    /// <remarks>
    /// The group will have a content type of content. The ability to control this content
    /// type is the only distinction between this method and <see cref="PushGroup()"/> which you
    /// should see for a more detailed description of group rendering.
    /// </remarks>
    public void PushGroupWithContent(Content content)
    {
        this.CheckDisposed();
        cairo_push_group_with_content(this.Handle, content);
    }

    /// <summary>
    /// Temporarily redirects drawing to an intermediate surface known as a group.
    /// The redirection lasts until the group is completed by a call to <see cref="PopGroup"/> or
    /// <see cref="PopGroupToSource"/>. These calls provide the result of any drawing to the
    /// group as a pattern, (either as an explicit object, or set as the source pattern).
    /// </summary>
    /// <param name="content">a <see cref="Content"/> indicating the type of group that will be created</param>
    /// <remarks>
    /// This is a convenience overload, and the same as a call to <see cref="PushGroupWithContent(Content)"/>.
    /// </remarks>
    public void PushGroup(Content content) => this.PushGroupWithContent(content);

    /// <summary>
    /// Terminates the redirection begun by a call to <see cref="PushGroup()"/> or
    /// <see cref="PushGroupWithContent(Content)"/> and returns a new pattern containing the
    /// results of all drawing operations performed to the group.
    /// </summary>
    /// <returns>
    /// a newly created (surface) pattern containing the results of all drawing operations
    /// performed to the group. The caller owns the returned object and should call <see cref="CairoObject.Dispose()"/>
    /// when finished with it.
    /// </returns>
    /// <remarks>
    /// The <see cref="PopGroup"/> method calls <see cref="Restore"/>, (balancing a call to <see cref="Save"/>
    /// by the <see cref="PushGroup()"/> method), so that any changes to the graphics state will
    /// not be visible outside the group.
    /// </remarks>
    public SurfacePattern PopGroup()
    {
        this.CheckDisposed();

        cairo_pattern_t* pattern = cairo_pop_group(this.Handle);

        Debug.Assert(pattern is not null);
        SurfacePattern? surface = Pattern.Lookup(pattern, isOwnedByCairo: false) as SurfacePattern;

        return surface ?? throw new CairoException("Unexpected result, should be a surface pattern");
    }

    /// <summary>
    /// Terminates the redirection begun by a call to <see cref="PushGroup()"/>or
    /// <see cref="PushGroupWithContent(Content)"/> and installs the resulting pattern as
    /// the source pattern in the given cairo context.
    /// </summary>
    /// <remarks>
    /// The behavior of this method is equivalent to the sequence of operations:
    /// <code>
    /// using Pattern pattern = context.PopGroup();
    /// context.SetSource(pattern);
    /// </code>
    /// but is more convenient as there is no need for a variable to store the short-lived
    /// pointer to the pattern.
    /// <para>
    /// The <see cref="PopGroup"/> method calls <see cref="Restore"/>, (balancing a call to <see cref="Save"/>
    /// by the <see cref="PushGroup()"/> method), so that any changes to the graphics state will
    /// not be visible outside the group.
    /// </para>
    /// </remarks>
    public void PopGroupToSource()
    {
        this.CheckDisposed();
        cairo_pop_group_to_source(this.Handle);
    }

    /// <summary>
    /// Gets the current destination surface for the context. This is either the original target
    /// surface as passed to <see cref="CairoContext(Surface)"/> or the target surface for
    /// the current group as started by the most recent call to <see cref="PushGroup()"/> or
    /// <see cref="PushGroupWithContent(Content)"/>.
    /// </summary>
    /// <remarks>
    /// This object is owned by cairo.
    /// <para>
    /// This property will always return a valid pointer, but the result can be a "nil" surface if cr is
    /// already in an error state, (ie. <see cref="Status"/> != <see cref="Status.Success"/>. A nil
    /// surface is indicated by <see cref="Surface.Status"/> != <see cref="Status.Success"/>
    /// (or by the convenience <see cref="Surface.IsNilSurface"/> property).
    /// </para>
    /// </remarks>
    public Surface GroupTarget
    {
        get
        {
            this.CheckDisposed();

            cairo_surface_t* surface = cairo_get_group_target(this.Handle);
            return Surface.Lookup(surface, isOwnedByCairo: true, needsDestroy: false);
        }
    }

    /// <summary>
    /// Sets the source pattern within cr to an opaque color. This opaque color will then
    /// be used for any subsequent drawing operation until a new source pattern is set.
    /// </summary>
    /// <remarks>
    /// The color components are floating point numbers in the range 0 to 1. If the values
    /// passed in are outside that range, they will be clamped.
    /// <para>
    /// The default source pattern is opaque black, (that is, it is equivalent to
    /// <c>SetSourceRgb(0, 0, 0)</c>).
    /// </para>
    /// </remarks>
    public void SetSourceRgb(double red, double green, double blue)
    {
        this.CheckDisposed();
        cairo_set_source_rgb(this.Handle, red, green, blue);
    }

    /// <summary>
    /// Sets the source pattern within cr to a translucent color. This color will then be
    /// used for any subsequent drawing operation until a new source pattern is set.
    /// </summary>
    /// <remarks>
    /// The color and alpha components are floating point numbers in the range 0 to 1.
    /// If the values passed in are outside that range, they will be clamped.
    /// <para>
    /// Note that the color and alpha values are not premultiplied.
    /// </para>
    /// <para>
    /// The default source pattern is opaque black, (that is, it is equivalent to
    /// <c>SetSourceRgba(0, 0, 0, 1)</c>).
    /// </para>
    /// </remarks>
    public void SetSourceRgba(double red, double green, double blue, double alpha)
    {
        this.CheckDisposed();
        cairo_set_source_rgba(this.Handle, red, green, blue, alpha);
    }

    /// <summary>
    /// Sets the source pattern within cr to a translucent color. This color will then be
    /// used for any subsequent drawing operation until a new source pattern is set.
    /// </summary>
    /// <param name="color">color to set</param>
    /// <remarks>
    /// Note that the color and alpha values are not premultiplied.
    /// <para>
    /// The default source pattern is opaque black, (that is, it is equivalent to
    /// <c>SetSourceRgb(Color.Default)</c>).
    /// </para>
    /// </remarks>
    public void SetSourceColor(Color color)
    {
        this.CheckDisposed();
        cairo_set_source_rgba(this.Handle, color.Red, color.Green, color.Blue, color.Alpha);
    }

    /// <summary>
    /// Sets the source pattern within cr to a translucent color. This color will then be
    /// used for any subsequent drawing operation until a new source pattern is set.
    /// </summary>
    /// <remarks>
    /// See <see cref="SetSourceColor(Color)"/> for further information. This is just a
    /// convenience property setter.
    /// </remarks>
    public Color Color
    {
        set => this.SetSourceColor(value);
    }

    /// <summary>
    /// Sets the source pattern within cr to source. This pattern will then be used for
    /// any subsequent drawing operation until a new source pattern is set.
    /// </summary>
    /// <param name="pattern">
    /// a <see cref="Pattern"/> to be used as the source for subsequent drawing operations.
    /// </param>
    /// <remarks>
    /// Note: The pattern's transformation matrix will be locked to the user space
    /// in effect at the time of <see cref="SetSource(Pattern)"/>. This means that further
    /// modifications of the current transformation matrix will not affect the
    /// source pattern. See <see cref="Matrix"/>.
    /// <para>
    /// The default source pattern is a solid pattern that is opaque black, (that is, it is equivalent
    /// to <c>SetSourceColor(Color.Default)</c>).
    /// </para>
    /// </remarks>
    public void SetSource(Pattern pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        this.CheckDisposed();

        cairo_set_source(this.Handle, pattern.Handle);
    }

    /// <summary>
    /// This is a convenience method for <see cref="SetSourceSurface(Surface, double, double)"/>.
    /// </summary>
    /// <param name="surface">a surface to be used to set the source pattern</param>
    /// <param name="x">User-space X coordinate for surface origin</param>
    /// <param name="y">User-space Y coordinate for surface origin</param>
    public void SetSource(Surface surface, double x, double y) => this.SetSourceSurface(surface, x, y);

    /// <summary>
    /// This is a convenience method for creating a pattern from surface and setting
    /// it as the source in cr with <see cref="SetSource(Pattern)"/>.
    /// </summary>
    /// <param name="surface">a surface to be used to set the source pattern</param>
    /// <param name="x">User-space X coordinate for surface origin</param>
    /// <param name="y">User-space Y coordinate for surface origin</param>
    /// <remarks>
    /// The x and y parameters give the user-space coordinate at which the surface origin
    /// should appear. (The surface origin is its upper-left corner before any transformation
    /// has been applied.) The x and y parameters are negated and then set as translation
    /// values in the pattern matrix.
    /// <para>
    /// Other than the initial translation pattern matrix, as described above, all other pattern
    /// attributes, (such as its extend mode), are set to the default values as in
    /// <see cref="SurfacePattern(Surface)"/>. The resulting pattern can be queried with
    /// <see cref="GetSource"/> so that these attributes can be modified if desired, (eg. to
    /// create a repeating pattern with <see cref="Pattern.Extend"/>).
    /// </para>
    /// </remarks>
    public void SetSourceSurface(Surface surface, double x, double y)
    {
        ArgumentNullException.ThrowIfNull(surface);
        this.CheckDisposed();

        cairo_set_source_surface(this.Handle, surface.Handle, x, y);
    }

    /// <summary>
    /// Gets the current source pattern for cr.
    /// </summary>
    /// <returns>
    /// the current source pattern. This object is owned by cairo.
    /// </returns>
    public Pattern? GetSource()
    {
        this.CheckDisposed();

        cairo_pattern_t* pattern = cairo_get_source(this.Handle);
        return Pattern.Lookup(pattern, isOwnedByCairo: true, needsDestroy: false);
    }

    /// <summary>
    /// Set the antialiasing mode of the rasterizer used for drawing shapes. This value
    /// is a hint, and a particular backend may or may not support a particular value.
    /// At the current time, no backend supports <see cref="Antialias.Subpixel"/> when drawing shapes.
    /// </summary>
    /// <remarks>
    /// Note that this option does not affect text rendering, instead see <see cref="Fonts.FontOptions.Antialias"/>.
    /// </remarks>
    public Antialias Antialias
    {
        get
        {
            this.CheckDisposed();
            return cairo_get_antialias(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_set_antialias(this.Handle, value);
        }
    }

    /// <summary>
    /// Sets the dash pattern to be used by <see cref="Stroke"/>. A dash pattern is specified
    /// by dashes, an array of positive values. Each value provides the length of alternate
    /// "on" and "off" portions of the stroke. The offset specifies an offset into the pattern
    /// at which the stroke begins.
    /// </summary>
    /// <param name="dashes">
    /// an array specifying alternate lengths of on and off stroke portions
    /// <para>
    /// If <see cref="ReadOnlySpan{T}.Empty"/> dashing is disabled.
    /// </para>
    /// <para>
    /// When <see cref="ReadOnlySpan{T}.Length"/> is 1 a symmetric pattern is assumed with alternating
    /// on and off portions of the size specified by the single value in dashes.
    /// </para>
    /// <para>
    /// If any value in dashes is negative, or if all values are 0, then cr will be put into an
    /// error state with a status of <see cref="Status.InvalidDash"/>.
    /// </para>
    /// </param>
    /// <param name="offset">an offset into the dash pattern at which the stroke should start</param>
    /// <remarks>
    /// Each "on" segment will have caps applied as if the segment were a separate sub-path.
    /// In particular, it is valid to use an "on" length of 0.0 with <see cref="LineCap.Round"/>
    /// or <see cref="LineCap.Square"/> in order to distributed dots or squares along a path.
    /// <para>
    /// Note: The length values are in user-space units as evaluated at the time of stroking.
    /// This is not necessarily the same as the user space at the time of
    /// <see cref="SetDash(ReadOnlySpan{double}, double)"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">
    /// will be thrown when <see cref="Status"/> is <see cref="Status.InvalidDash"/>
    /// </exception>
    public void SetDash(ReadOnlySpan<double> dashes, double offset)
    {
        this.CheckDisposed();

        fixed (double* ptr = dashes)
        {
            cairo_set_dash(this.Handle, ptr, dashes.Length, offset);
        }

        this.Status.ThrowIfStatus(Status.InvalidDash);
    }

    /// <summary>
    /// Sets the dash pattern to be used by <see cref="Stroke"/>. A dash pattern is specified
    /// by dashes, an array of positive values. Each value provides the length of alternate
    /// "on" and "off" portions of the stroke.
    /// </summary>
    /// <param name="dashes">
    /// an array specifying alternate lengths of on and off stroke portions
    /// <para>
    /// If <see cref="ReadOnlySpan{T}.Empty"/> dashing is disabled.
    /// </para>
    /// <para>
    /// When <see cref="ReadOnlySpan{T}.Length"/> is 1 a symmetric pattern is assumed with alternating
    /// on and off portions of the size specified by the single value in dashes.
    /// </para>
    /// <para>
    /// If any value in dashes is negative, or if all values are 0, then cr will be put into an
    /// error state with a status of <see cref="Status.InvalidDash"/>.
    /// </para>
    /// </param>
    /// <remarks>
    /// This is a convenience overload for <see cref="SetDash(ReadOnlySpan{double}, double)"/> with
    /// offset = 0.
    /// </remarks>
    public void SetDash(params ReadOnlySpan<double> dashes) => this.SetDash(dashes, offset: 0);

    /// <summary>
    /// This property returns the length of the dash array in cr (0 if dashing is not currently in effect).
    /// </summary>
    public int DashCount
    {
        get
        {
            this.CheckDisposed();
            return cairo_get_dash_count(this.Handle);
        }
    }

    /// <summary>
    /// Gets whether dashing is currently in effect.
    /// </summary>
    /// <remarks>
    /// Convenience property for <c>DashCount > 0</c>.
    /// </remarks>
    public bool DashingActive => this.DashCount > 0;

    /// <summary>
    /// Gets the current dash array.
    /// </summary>
    /// <param name="dashes">return value for the dash array, or <see cref="Span{T}.Empty"/></param>
    /// <param name="offset">return value for the current dash offset, or NULL</param>
    /// <returns>
    /// <c>true</c> when there are dashes set, otherwise <c>false</c>
    /// </returns>
    /// <remarks>
    /// If not <see cref="Span{T}.Empty"/>, <paramref name="dashes"/> should be big enough to hold
    /// at least the number of values returned by <see cref="DashCount"/>.
    /// </remarks>
    public bool TryGetDash(Span<double> dashes, out double offset)
    {
        this.CheckDisposed();

        fixed (double* dashesNativ  = dashes)
        fixed (double* offsetNative = &offset)
        {
            cairo_get_dash(this.Handle, dashesNativ, offsetNative);

            if (dashesNativ is null || offsetNative is null)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets or sets the current fill rule within the cairo context. The fill rule is used to
    /// determine which regions are inside or outside a complex (potentially self-intersecting)
    /// path. The current fill rule affects both <see cref="Fill"/> and <see cref="Clip"/>.
    /// </summary>
    /// <remarks>
    /// See <see cref="Drawing.FillRule"/> for details on the semantics of each available fill rule.
    /// <para>
    /// The default fill rule is <see cref="FillRule.Winding"/>.
    /// </para>
    /// </remarks>
    public FillRule FillRule
    {
        get
        {
            this.CheckDisposed();
            return cairo_get_fill_rule(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_set_fill_rule(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the current line cap style within the cairo context
    /// </summary>
    /// <remarks>
    /// See <see cref="LineCap"/> for details about how the available line cap styles are drawn.
    /// <para>
    /// As with the other stroke parameters, the current line cap style is examined by
    /// <see cref="Stroke"/>, and <see cref="StrokeExtents()"/>, but does not have any
    /// effect during path construction.
    /// </para>
    /// <para>
    /// The default line cap style is <see cref="LineCap.Butt"/>.
    /// </para>
    /// </remarks>
    public LineCap LineCap
    {
        get
        {
            this.CheckDisposed();
            return cairo_get_line_cap(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_set_line_cap(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the current line join style within the cairo context.
    /// </summary>
    /// <remarks>
    /// See <see cref="LineJoin"/> for details about how the available line join styles are drawn.
    /// <para>
    /// As with the other stroke parameters, the current line cap style is examined by
    /// <see cref="Stroke"/>, and <see cref="StrokeExtents()"/>, but does not have any
    /// effect during path construction.
    /// </para>
    /// <para>
    /// The default line join style is <see cref="LineJoin.Miter"/>.
    /// </para>
    /// </remarks>
    public LineJoin LineJoin
    {
        get
        {
            this.CheckDisposed();
            return cairo_get_line_join(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_set_line_join(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the current line width within the cairo context.
    /// </summary>
    /// <remarks>
    /// The line width value specifies the diameter of a pen that is circular in user space,
    /// (though device-space pen may be an ellipse in general due to scaling/shear/rotation of the CTM).
    /// <para>
    /// Note: When the description above refers to user space and CTM it refers to the user space
    /// and CTM in effect at the time of the stroking operation, not the user space and CTM in effect
    /// at the time of the call to <see cref="LineWidth"/>. The simplest usage makes both of these spaces
    /// identical. That is, if there is no change to the CTM between a call to <see cref="LineWidth"/> and the
    /// stroking operation, then one can just pass user-space values to <see cref="LineWidth"/> and
    /// ignore this note.
    /// </para>
    /// <para>
    /// As with the other stroke parameters, the current line cap style is examined by
    /// <see cref="Stroke"/>, and <see cref="StrokeExtents()"/>, but does not have any
    /// effect during path construction.
    /// </para>
    /// <para>
    /// This property returns the current line width value exactly as set by <see cref="LineWidth"/>.
    /// Note that the value is unchanged even if the CTM has changed between the calls to cairo_set_line_width()
    /// and cairo_get_line_width().
    /// </para>
    /// <para>
    /// The default line width value is 2.0.
    /// </para>
    /// </remarks>
    public double LineWidth
    {
        get
        {
            this.CheckDisposed();
            return cairo_get_line_width(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_set_line_width(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the current miter limit within the cairo context.
    /// </summary>
    /// <remarks>
    /// If the current line join style is set to <see cref="LineJoin.Miter"/> (see <see cref="LineJoin"/>),
    /// the miter limit is used to determine whether the lines should be joined with a bevel instead
    /// of a miter. Cairo divides the length of the miter by the line width. If the result is greater
    /// than the miter limit, the style is converted to a bevel.
    /// <para>
    /// As with the other stroke parameters, the current line cap style is examined by
    /// <see cref="Stroke"/>, and <see cref="StrokeExtents()"/>, but does not have any
    /// effect during path construction.
    /// </para>
    /// <para>
    /// The default miter limit value is 10.0, which will convert joins with interior angles
    /// less than 11 degrees to bevels instead of miters. For reference, a miter limit of 2.0 makes
    /// the miter cutoff at 60 degrees, and a miter limit of 1.414 makes the cutoff at 90 degrees.
    /// </para>
    /// <para>
    /// A miter limit for a desired angle can be computed as: miter limit = 1/sin(angle/2)
    /// </para>
    /// </remarks>
    public double MiterLimit
    {
        get
        {
            this.CheckDisposed();
            return cairo_get_miter_limit(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_set_miter_limit(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the compositing operator to be used for all drawing operations.
    /// </summary>
    /// <remarks>
    /// Sets the compositing operator to be used for all drawing operations. See <see cref="Operator"/> for
    /// details on the semantics of each available compositing operator.
    /// <para>
    /// The default operator is <see cref="Operator.Over"/>.
    /// </para>
    /// </remarks>
    public Operator Operator
    {
        get
        {
            this.CheckDisposed();
            return cairo_get_operator(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_set_operator(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the tolerance used when converting paths into trapezoids
    /// in device units (typically pixels).
    /// </summary>
    /// <remarks>
    /// Curved segments of the path will be subdivided until the maximum deviation between the
    /// original path and the polygonal approximation is less than tolerance. The default value
    /// is 0.1. A larger value will give better performance, a smaller value, better appearance.
    /// (Reducing the value from the default value of 0.1 is unlikely to improve appearance significantly.)
    /// The accuracy of paths within Cairo is limited by the precision of its internal arithmetic,
    /// and the prescribed tolerance is restricted to the smallest representable internal value.
    /// </remarks>
    public double Tolerance
    {
        get
        {
            this.CheckDisposed();
            return cairo_get_tolerance(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_set_tolerance(this.Handle, value);
        }
    }

    /// <summary>
    /// Establishes a new clip region by intersecting the current clip region with the current path as it
    /// would be filled by <see cref="Fill"/> and according to the current fill rule (see <see cref="FillRule"/>).
    /// </summary>
    /// <remarks>
    /// After <see cref="Clip"/>, the current path will be cleared from the cairo context.
    /// <para>
    /// The current clip region affects all drawing operations by effectively masking out any
    /// changes to the surface that are outside the current clip region.
    /// </para>
    /// <para>
    /// Calling <see cref="Clip"/> can only make the clip region smaller, never larger.
    /// But the current clip is part of the graphics state, so a temporary restriction of the
    /// clip region can be achieved by calling <see cref="Clip"/> within a <see cref="Save"/>/<see cref="Restore"/>
    /// pair. The only other means of increasing the size of the clip region is <see cref="ResetClip"/>.
    /// </para>
    /// </remarks>
    public void Clip()
    {
        this.CheckDisposed();
        cairo_clip(this.Handle);
    }

    /// <summary>
    /// Establishes a new clip region by intersecting the current clip region with the current path as it
    /// would be filled by <see cref="Fill"/> and according to the current fill rule (see <see cref="FillRule"/>).
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="Clip"/>, <see cref="ClipPreserve"/> preserves the path within the cairo context.
    /// <para>
    /// The current clip region affects all drawing operations by effectively masking out any changes
    /// to the surface that are outside the current clip region.
    /// </para>
    /// <para>
    /// Calling <see cref="ClipPreserve"/> can only make the clip region smaller, never larger.
    /// But the current clip is part of the graphics state, so a temporary restriction of the clip
    /// region can be achieved by calling <see cref="ClipPreserve"/> within a <see cref="Save"/>/<see cref="Restore"/>
    /// pair. The only other means of increasing the size of the clip region is <see cref="ResetClip"/>.
    /// </para>
    /// </remarks>
    public void ClipPreserve()
    {
        this.CheckDisposed();
        cairo_clip_preserve(this.Handle);
    }

    /// <summary>
    /// Computes a bounding box in user coordinates covering the area inside the current clip.
    /// </summary>
    /// <param name="x1">left of the resulting extents</param>
    /// <param name="y1">top of the resulting extents</param>
    /// <param name="x2">right of the resulting extents</param>
    /// <param name="y2">bottom of the resulting extents</param>
    public void GetClipExtents(out double x1, out double y1, out double x2, out double y2)
    {
        this.CheckDisposed();
        cairo_clip_extents(this.Handle, out x1, out y1, out x2, out y2);
    }

    /// <summary>
    /// Tests whether the given point is inside the area that would be visible through the current
    /// clip, i.e. the area that would be filled by a <see cref="Paint"/> operation.
    /// </summary>
    /// <param name="x">X coordinate of the point to test</param>
    /// <param name="y">Y coordinate of the point to test</param>
    /// <returns>
    /// <c>true</c> if the point is inside, or <c>false</c> if outside.
    /// </returns>
    public bool InClip(double x, double y)
    {
        this.CheckDisposed();
        return cairo_in_clip(this.Handle, x, y);
    }

    /// <summary>
    /// Tests whether the given point is inside the area that would be visible through the current
    /// clip, i.e. the area that would be filled by a <see cref="Paint"/> operation.
    /// </summary>
    /// <param name="point">the point to test</param>
    /// <returns>
    /// <c>true</c> if the point is inside, or <c>false</c> if outside.
    /// </returns>
    public bool InClip(PointD point) => this.InClip(point.X, point.Y);

    /// <summary>
    /// Reset the current clip region to its original, unrestricted state. That is, set the clip
    /// region to an infinitely large shape containing the target surface. Equivalently, if
    /// infinity is too hard to grasp, one can imagine the clip region being reset to the exact
    /// bounds of the target surface.
    /// </summary>
    /// <remarks>
    /// Note that code meant to be reusable should not call <see cref="ResetClip"/> as it will cause
    /// results unexpected by higher-level code which calls <see cref="Clip"/>. Consider using <see cref="Save"/>
    /// and <see cref="Restore"/> around <see cref="Clip"/> as a more robust means of
    /// temporarily restricting the clip region.
    /// </remarks>
    public void ResetClip()
    {
        this.CheckDisposed();
        cairo_reset_clip(this.Handle);
    }

    /// <summary>
    /// Gets the current clip region as a list of rectangles in user coordinates. Never returns <c>null</c>.
    /// </summary>
    /// <remarks>
    /// The status in the list may be <see cref="Status.ClipNotRepresentable"/> to indicate that the clip
    /// region cannot be represented as a list of user-space rectangles. The status may have other values
    /// to indicate other errors.
    /// </remarks>
    /// <returns>
    /// the current clip region as a list of rectangles in user coordinates, which should be
    /// destroyed using <see cref="CairoObject.Dispose()"/>.
    /// </returns>
    public RectangleList CopyClipRectangleList()
    {
        this.CheckDisposed();

        RectangleListRaw* handle = cairo_copy_clip_rectangle_list(this.Handle);
        return new RectangleList(handle);
    }

    /// <summary>
    /// A drawing operator that fills the current path according to the current fill rule, (each
    /// sub-path is implicitly closed before being filled). After <see cref="Fill"/>, the current
    /// path will be cleared from the cairo context.
    /// </summary>
    /// <remarks>
    /// See <see cref="FillRule"/> and <see cref="FillPreserve"/>.
    /// </remarks>
    public void Fill()
    {
        this.CheckDisposed();
        cairo_fill(this.Handle);
    }

    /// <summary>
    /// A drawing operator that fills the current path according to the current fill rule, (each
    /// sub-path is implicitly closed before being filled). Unlike <see cref="Fill"/>, <see cref="FillPreserve"/>
    /// preserves the path within the cairo context.
    /// </summary>
    /// <remarks>
    /// See <see cref="FillRule"/> and <see cref="FillPreserve"/>.
    /// </remarks>
    public void FillPreserve()
    {
        this.CheckDisposed();
        cairo_fill_preserve(this.Handle);
    }

    /// <summary>
    /// Computes a bounding box in user coordinates covering the area that would be affected,
    /// (the "inked" area), by a <see cref="Fill"/> operation given the current path and fill
    /// parameters. If the current path is empty, returns an empty rectangle ((0,0), (0,0)).
    /// Surface dimensions and clipping are not taken into account.
    /// </summary>
    /// <param name="x1">left of the resulting extents</param>
    /// <param name="y1">top of the resulting extents</param>
    /// <param name="x2">right of the resulting extents</param>
    /// <param name="y2">bottom of the resulting extents</param>
    /// <remarks>
    /// Contrast with <see cref="PathExtensions.PathExtents(CairoContext, out double, out double, out double, out double)"/>,
    /// which is similar, but returns non-zero extents for some paths with no inked area, (such as a simple line segment).
    /// <para>
    /// Note that <see cref="FillExtents(out double, out double, out double, out double)"/> must necessarily
    /// do more work to compute the precise inked areas in light of the fill rule, so
    /// <see cref="PathExtensions.PathExtents(CairoContext, out double, out double, out double, out double)"/>
    /// may be more desirable for sake of performance if the non-inked path extents are desired.
    /// </para>
    /// <para>
    /// See <see cref="Fill"/>, <see cref="FillRule"/> and <see cref="FillPreserve"/>.
    /// </para>
    /// </remarks>
    public void FillExtents(out double x1, out double y1, out double x2, out double y2)
    {
        this.CheckDisposed();
        cairo_fill_extents(this.Handle, out x1, out y1, out x2, out y2);
    }

    /// <summary>
    /// Computes a bounding box in user coordinates covering the area that would be affected,
    /// (the "inked" area), by a <see cref="Fill"/> operation given the current path and fill
    /// parameters. If the current path is empty, returns an empty rectangle ((0,0), (0,0)).
    /// Surface dimensions and clipping are not taken into account.
    /// </summary>
    /// <returns>The bounding box</returns>
    /// <remarks>
    /// See <see cref="FillExtents(out double, out double, out double, out double)"/> for
    /// more information.
    /// </remarks>
    public Rectangle FillExtents()
    {
        this.FillExtents(out double x1, out double y1, out double x2, out double y2);
        return new Rectangle(x1, y1, x2 - y1, y2 - y1);
    }

    /// <summary>
    /// Tests whether the given point is inside the area that would be affected by a <see cref="Fill"/>
    /// operation given the current path and filling parameters. Surface dimensions and
    /// clipping are not taken into account.
    /// </summary>
    /// <param name="x">X coordinate of the point to test</param>
    /// <param name="y">Y coordinate of the point to test</param>
    /// <returns>
    /// <c>true</c> if the point is inside, or <c>false</c> if outside.
    /// </returns>
    /// <remarks>
    /// See <see cref="Fill"/>, <see cref="FillRule"/> and <see cref="FillPreserve"/>.
    /// </remarks>
    public bool InFill(double x, double y)
    {
        this.CheckDisposed();
        return cairo_in_fill(this.Handle, x, y);
    }

    /// <summary>
    /// Tests whether the given point is inside the area that would be affected by a <see cref="Fill"/>
    /// operation given the current path and filling parameters. Surface dimensions and
    /// clipping are not taken into account.
    /// </summary>
    /// <param name="point">the point to test</param>
    /// <returns>
    /// <c>true</c> if the point is inside, or <c>false</c> if outside.
    /// </returns>
    /// <remarks>
    /// See <see cref="Fill"/>, <see cref="FillRule"/> and <see cref="FillPreserve"/>.
    /// </remarks>
    public bool InFill(PointD point) => this.InFill(point.X, point.Y);

    /// <summary>
    /// A drawing operator that paints the current source using the alpha channel of pattern
    /// as a mask. (Opaque areas of pattern are painted with the source, transparent areas
    /// are not painted.)
    /// </summary>
    /// <param name="pattern">a <see cref="Pattern"/></param>
    /// <exception cref="ArgumentNullException">when <paramref name="pattern"/> is <c>null</c></exception>
    public void Mask(Pattern pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        this.CheckDisposed();

        cairo_mask(this.Handle, pattern.Handle);
    }

    /// <summary>
    /// A drawing operator that paints the current source using the alpha channel of surface
    /// as a mask. (Opaque areas of surface are painted with the source, transparent areas
    /// are not painted.)
    /// </summary>
    /// <param name="surface">a <see cref="Surface"/></param>
    /// <param name="surfaceX">X coordinate at which to place the origin of <paramref name="surface"/></param>
    /// <param name="surfaceY">Y coordinate at which to place the origin of <paramref name="surface"/></param>
    /// <exception cref="ArgumentNullException">when <paramref name="surface"/> is <c>null</c></exception>
    public void MaskSurface(Surface surface, double surfaceX, double surfaceY)
    {
        ArgumentNullException.ThrowIfNull(surface);
        this.CheckDisposed();

        cairo_mask_surface(this.Handle, surface.Handle, surfaceX, surfaceY);
    }

    /// <summary>
    /// A drawing operator that paints the current source everywhere within the current clip region.
    /// </summary>
    public void Paint()
    {
        this.CheckDisposed();
        cairo_paint(this.Handle);
    }

    /// <summary>
    /// A drawing operator that paints the current source everywhere within the current clip
    /// region using a mask of constant alpha value <paramref name="alpha"/>. The effect is similar
    /// to <see cref="Paint"/>, but the drawing is faded out using the alpha value.
    /// </summary>
    /// <param name="alpha">alpha value, between 0 (transparent) and 1 (opaque)</param>
    public void PaintWithAlpha(double alpha)
    {
        this.CheckDisposed();
        cairo_paint_with_alpha(this.Handle, alpha);
    }

    /// <summary>
    /// A drawing operator that strokes the current path according to the current line width,
    /// line join, line cap, and dash settings. After <see cref="Stroke"/>, the current path
    /// will be cleared from the cairo context. See <see cref="LineWidth"/>, <see cref="LineJoin"/>,
    /// <see cref="LineCap"/>, <see cref="SetDash(ReadOnlySpan{double}, double)"/>, and
    /// <see cref="StrokePreserve"/>.
    /// </summary>
    /// <remarks>
    /// Note: Degenerate segments and sub-paths are treated specially and provide a useful result.
    /// These can result in two different situations:
    /// <list type="number">
    /// <item>
    /// Zero-length "on" segments set in <see cref="SetDash(ReadOnlySpan{double}, double)"/>.
    /// If the cap style is <see cref="LineCap.Round"/> or <see cref="LineCap.Square"/> then
    /// these segments will be drawn as circular dots or squares respectively. In the case of
    /// <see cref="LineCap.Square"/>, the orientation of the squares is determined by the
    /// direction of the underlying path.
    /// </item>
    /// <item>
    /// A sub-path created by <see cref="PathExtensions.MoveTo(CairoContext, PointD)"/> followed by either a
    /// <see cref="PathExtensions.ClosePath(CairoContext)"/> or one or more calls to
    /// <see cref="PathExtensions.LineTo(CairoContext, PointD)"/> to the same coordinate as the
    /// <see cref="PathExtensions.MoveTo(CairoContext, PointD)"/>.
    /// If the cap style is <see cref="LineCap.Round"/> then these sub-paths will be drawn as circular
    /// dots. Note that in the case of <see cref="LineCap.Square"/> a degenerate sub-path will not
    /// be drawn at all, (since the correct orientation is indeterminate).
    /// </item>
    /// </list>
    /// In no case will a cap style of <see cref="LineCap.Butt"/> cause anything to be drawn in the case
    /// of either degenerate segments or sub-paths.
    /// </remarks>
    public void Stroke()
    {
        this.CheckDisposed();
        cairo_stroke(this.Handle);
    }

    /// <summary>
    /// A drawing operator that strokes the current path according to the current line width, line join,
    /// line cap, and dash settings. Unlike <see cref="Stroke"/>, <see cref="StrokePreserve"/> preserves
    /// the path within the cairo context.
    /// </summary>
    /// <remarks>
    /// See <see cref="LineWidth"/>, <see cref="LineJoin"/>, <see cref="LineCap"/>,
    /// <see cref="SetDash(ReadOnlySpan{double}, double)"/>, and <see cref="StrokePreserve"/>.
    /// </remarks>
    public void StrokePreserve()
    {
        this.CheckDisposed();
        cairo_stroke_preserve(this.Handle);
    }

    /// <summary>
    /// Computes a bounding box in user coordinates covering the area that would be affected, (the
    /// "inked" area), by a <see cref="Stroke"/> operation given the current path and stroke parameters.
    /// If the current path is empty, returns an empty rectangle ((0,0), (0,0)). Surface dimensions
    /// and clipping are not taken into account.
    /// </summary>
    /// <param name="x1">left of the resulting extents</param>
    /// <param name="y1">top of the resulting extents</param>
    /// <param name="x2">right of the resulting extents</param>
    /// <param name="y2">bottom of the resulting extents</param>
    /// <remarks>
    /// Note that if the line width is set to exactly zero, then <see cref="StrokeExtents()"/> will return
    /// an empty rectangle. Contrast with <see cref="PathExtensions.PathExtents(CairoContext, out double, out double, out double, out double)"/>
    /// which can be used to compute the non-empty bounds as the line width approaches zero.
    /// <para>
    /// Note that <see cref="StrokeExtents()"/> must necessarily do more work to compute the precise
    /// inked areas in light of the stroke parameters, so
    /// <see cref="PathExtensions.PathExtents(CairoContext, out double, out double, out double, out double)"/>
    /// may be more desirable for sake of performance if non-inked path extents are desired.
    /// </para>
    /// <para>
    /// See <see cref="Stroke"/>, <see cref="LineWidth"/>, <see cref="LineJoin"/>, <see cref="LineCap"/>,
    /// <see cref="SetDash(ReadOnlySpan{double}, double)"/>, and <see cref="StrokePreserve"/>.
    /// </para>
    /// </remarks>
    public void StrokeExtents(out double x1, out double y1, out double x2, out double y2)
    {
        this.CheckDisposed();
        cairo_stroke_extents(this.Handle, out x1, out y1, out x2, out y2);
    }

    /// <summary>
    /// Computes a bounding box in user coordinates covering the area that would be affected, (the
    /// "inked" area), by a <see cref="Stroke"/> operation given the current path and stroke parameters.
    /// If the current path is empty, returns an empty rectangle ((0,0), (0,0)). Surface dimensions
    /// and clipping are not taken into account.
    /// </summary>
    /// <returns>the bounding box</returns>
    /// <remarks>
    /// See <see cref="StrokeExtents(out double, out double, out double, out double)"/> for more information.
    /// </remarks>
    public Rectangle StrokeExtents()
    {
        this.CheckDisposed();

        this.StrokeExtents(out double x1, out double y1, out double x2, out double y2);
        return new Rectangle(x1, y1, x2 - x1, y2 - y1);
    }

    /// <summary>
    /// Tests whether the given point is inside the area that would be affected by a <see cref="Stroke"/>
    /// operation given the current path and stroking parameters. Surface dimensions and clipping
    /// are not taken into account.
    /// </summary>
    /// <param name="x">X coordinate of the point to test</param>
    /// <param name="y">Y coordinate of the point to test</param>
    /// <returns>
    /// <c>true</c> if the point is inside, or <c>false</c> outside.
    /// </returns>
    /// <remarks>
    /// See <see cref="Stroke"/>, <see cref="LineWidth"/>, <see cref="LineJoin"/>, <see cref="LineCap"/>,
    /// <see cref="SetDash(ReadOnlySpan{double}, double)"/>, and <see cref="StrokePreserve"/>.
    /// </remarks>
    public bool InStroke(double x, double y)
    {
        this.CheckDisposed();
        return cairo_in_stroke(this.Handle, x, y);
    }

    /// <summary>
    /// Tests whether the given point is inside the area that would be affected by a <see cref="Stroke"/>
    /// operation given the current path and stroking parameters. Surface dimensions and clipping
    /// are not taken into account.
    /// </summary>
    /// <param name="point">the point to test</param>
    /// <returns>
    /// <c>true</c> if the point is inside, or <c>false</c> outside.
    /// </returns>
    /// <remarks>
    /// See <see cref="Stroke"/>, <see cref="LineWidth"/>, <see cref="LineJoin"/>, <see cref="LineCap"/>,
    /// <see cref="SetDash(ReadOnlySpan{double}, double)"/>, and <see cref="StrokePreserve"/>.
    /// </remarks>
    public bool InStroke(PointD point) => this.InStroke(point.X, point.Y);

    /// <summary>
    /// Emits the current page for backends that support multiple pages, but doesn't clear it,
    /// so, the contents of the current page will be retained for the next page too. Use
    /// <see cref="ShowPage"/> if you want to get an empty page after the emission.
    /// </summary>
    /// <remarks>
    /// This is a convenience method that simply calls <see cref="Surface.CopyPage"/> on cr 's target.
    /// </remarks>
    public void CopyPage()
    {
        this.CheckDisposed();
        cairo_copy_page(this.Handle);
    }

    /// <summary>
    /// Emits and clears the current page for backends that support multiple pages. Use <see cref="CopyPage"/>
    /// if you don't want to clear the page.
    /// </summary>
    /// <remarks>
    /// This is a convenience method that simply calls <see cref="Surface.ShowPage"/> on cr 's target.
    /// </remarks>
    public void ShowPage()
    {
        this.CheckDisposed();
        cairo_show_page(this.Handle);
    }

    /// <summary>
    /// Returns the current reference count of cr.
    /// </summary>
    /// <remarks>
    /// If the object is a nil object, 0 will be returned.
    /// </remarks>
    internal int ReferenceCount
    {
        get
        {
            this.CheckDisposed();
            return (int)cairo_get_reference_count(this.Handle);
        }
    }

    /// <summary>
    /// Attach user data to cr.
    /// </summary>
    /// <param name="key">the address of a <see cref="UserDataKey"/> to attach the user data to</param>
    /// <param name="userData">the user data to attach to the context</param>
    /// <param name="destroyFunction">
    /// a cairo_destroy_func_t which will be called when the context is destroyed or when new
    /// user data is attached using the same key.
    /// </param>
    /// <remarks>
    /// To remove user data from a surface, call this method with the key that was used to set it
    /// and <c>null</c> for data.
    /// </remarks>
    internal void SetUserData(ref UserDataKey key, void* userData, cairo_destroy_func_t destroyFunction)
    {
        this.CheckDisposed();

        Status status = cairo_set_user_data(this.Handle, ref key, userData, destroyFunction);
        status.ThrowIfNotSuccess();
    }

    /// <summary>
    /// Return user data previously attached to cr using the specified key. If no user data
    /// has been attached with the given key this method returns <c>null</c>.
    /// </summary>
    /// <param name="key">the address of the <see cref="UserDataKey"/> the user data was attached to</param>
    /// <returns> the user data previously attached or <c>null</c>.</returns>
    internal void* GetUserData(ref UserDataKey key)
    {
        this.CheckDisposed();
        return cairo_get_user_data(this.Handle, ref key);
    }

    /// <summary>
    /// Gets or sets lines within the cairo context to be hairlines. Hairlines are logically zero-width
    /// lines that are drawn at the thinnest renderable width possible in the current context.
    /// </summary>
    /// <remarks>
    /// On surfaces with native hairline support, the native hairline functionality will be used. Surfaces
    /// that support hairlines include:
    /// <list type="bullet">
    /// <item>pdf/ps: Encoded as 0-width line.</item>
    /// <item>win32_printing: Rendered with PS_COSMETIC pen.</item>
    /// <item>svg: Encoded as 1px non-scaling-stroke.</item>
    /// <item>script: Encoded with set-hairline function.</item>
    /// </list>
    /// Cairo will always render hairlines at 1 device unit wide, even if an anisotropic scaling was applied
    /// to the stroke width. In the wild, handling of this situation is not well-defined. Some PDF, PS, and
    /// SVG renderers match Cairo's output, but some very popular implementations (Acrobat, Chrome, rsvg)
    /// will scale the hairline unevenly. As such, best practice is to reset any anisotropic scaling before
    /// calling <see cref="Stroke"/>.
    /// See <a href="https://cairographics.org/cookbook/ellipses/">cairo cookbok - ellipses</a> for an example.
    /// </remarks>
    public bool Hairline
    {
        get
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            return cairo_get_hairline(this.Handle);
        }
        set
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            cairo_set_hairline(this.Handle, value);
        }
    }
}
