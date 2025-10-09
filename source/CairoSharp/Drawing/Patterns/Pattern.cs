// (c) gfoidl, all rights reserved

using System.Diagnostics.CodeAnalysis;
using Cairo.Utilities;
using static Cairo.Drawing.Patterns.PatternNative;

namespace Cairo.Drawing.Patterns;

/// <summary>
/// cairo_pattern_t — Sources for drawing
/// </summary>
/// <remarks>
/// <see cref="Pattern"/> is the paint with which cairo draws. The primary use of patterns is as
/// the source for all cairo drawing operations, although they can also be used as masks, that is,
/// as the brush too.
/// <para>
/// A cairo pattern is created by using one of the many constructors, of the form cairo_pattern_create_type()
/// or implicitly through cairo_set_source_type() functions.
/// </para>
/// </remarks>
public unsafe class Pattern : CairoObject
{
    protected internal Pattern(void* handle, bool owner) : base(handle)
    {
        this.Status.ThrowIfNotSuccess();

        if (!owner)
        {
            cairo_pattern_reference(handle);
        }
    }

    protected override void DisposeCore(void* handle) => cairo_pattern_destroy(handle);

    /// <summary>
    /// Checks whether an error has previously occurred for this pattern.
    /// </summary>
    public Status Status
    {
        get
        {
            this.CheckDisposed();
            return cairo_pattern_status(this.Handle);
        }
    }

    /// <summary>
    /// Gets or sets the mode to be used for drawing outside the area of a pattern. See <see cref="Extend"/>
    /// for details on the semantics of each extend strategy.
    /// </summary>
    /// <remarks>
    /// The default extend mode is <see cref="Extend.None"/> for surface patterns and <see cref="Extend.Pad"/>
    /// for gradient patterns.
    /// </remarks>
    public Extend Extend
    {
        get
        {
            this.CheckDisposed();
            return cairo_pattern_get_extend(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_pattern_set_extend(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the filter to be used for resizing when using this pattern. See <see cref="Filter"/>
    /// for details on each filter.
    /// </summary>
    /// <remarks>
    /// Note that you might want to control filtering even when you do not have an explicit <see cref="Pattern"/>
    /// object, (for example when using <see cref="CairoContext.SetSourceSurface(Surfaces.Surface, double, double)"/>).
    /// In these cases, it is convenient to use <see cref="CairoContext.GetSource"/> to get access to the pattern
    /// that cairo creates implicitly. For example:
    /// <code>
    /// context.SetSourceSurface(image, x, y);
    /// context.GetSource().Filter(Filter.Nearest);
    /// </code>
    /// </remarks>
    public Filter Filter
    {
        get
        {
            this.CheckDisposed();
            return cairo_pattern_get_filter(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_pattern_set_filter(this.Handle, value);
        }
    }

    /// <summary>
    /// Sets the pattern's transformation matrix to matrix. This matrix is a transformation from
    /// user space to pattern space.
    /// </summary>
    /// <param name="matrix">a cairo matrix</param>
    /// <remarks>
    /// When a pattern is first created it always has the identity matrix for its transformation matrix,
    /// which means that pattern space is initially identical to user space.
    /// <para>
    /// Important: Please note that the direction of this transformation matrix is from user space to
    /// pattern space. This means that if you imagine the flow from a pattern to user space (and on
    /// to device space), then coordinates in that flow will be transformed by the inverse of the pattern matrix.
    /// </para>
    /// <para>
    /// For example, if you want to make a pattern appear twice as large as it does by default the correct code to use is:
    /// <code>
    /// Matrix matrix = default;
    /// matrix.InitScale(0.5, 0.5);
    /// pattern.SetMatrix(ref matrix);
    /// </code>
    /// Meanwhile, using values of 2.0 rather than 0.5 in the code above would cause the pattern to appear
    /// at half of its default size.
    /// </para>
    /// <para>
    /// Also, please note the discussion of the user-space locking semantics of <see cref="CairoContext.SetSource(Pattern)"/>.
    /// </para>
    /// </remarks>
    public void SetMatrix(ref Matrix matrix)
    {
        this.CheckDisposed();
        cairo_pattern_set_matrix(this.Handle, ref matrix);
    }

    /// <summary>
    /// Stores the pattern's transformation matrix into <paramref name="matrix"/>.
    /// </summary>
    /// <param name="matrix">return value for the matrix</param>
    public void GetMatrix(out Matrix matrix)
    {
        this.CheckDisposed();
        cairo_pattern_get_matrix(this.Handle, out matrix);
    }

    /// <summary>
    /// Get the pattern's type. See <see cref="PatternType"/> for available types.
    /// </summary>
    public PatternType Type
    {
        get
        {
            this.CheckDisposed();
            return cairo_pattern_get_type(this.Handle);
        }
    }

    /// <summary>
    /// Returns the current reference count of pattern.
    /// </summary>
    /// <remarks>
    /// If the object is a nil object, 0 will be returned.
    /// </remarks>
    public int ReferenceCount
    {
        get
        {
            this.CheckDisposed();
            return (int)cairo_pattern_get_reference_count(this.Handle);
        }
    }

    /// <summary>
    /// Attach user data to pattern.
    /// </summary>
    /// <param name="key">the address of a <see cref="UserDataKey"/> to attach the user data to</param>
    /// <param name="userData">the user data to attach to the context</param>
    /// <param name="destroyFunction">
    /// a cairo_destroy_func_t which will be called when the context is destroyed or when new
    /// user data is attached using the same key.
    /// </param>
    /// <remarks>
    /// To remove user data from a pattern, call this method with the key that was used to set it
    /// and <c>null</c> for data.
    /// </remarks>
    internal void SetUserData(ref UserDataKey key, void* userData, cairo_destroy_func_t destroyFunction)
    {
        this.CheckDisposed();

        Status status = cairo_pattern_set_user_data(this.Handle, ref key, userData, destroyFunction);
        status.ThrowIfNotSuccess();
    }

    /// <summary>
    /// Return user data previously attached to pattern using the specified key. If no user data
    /// has been attached with the given key this method returns <c>null</c>.
    /// </summary>
    /// <param name="key">the address of the <see cref="UserDataKey"/> the user data was attached to</param>
    /// <returns> the user data previously attached or <c>null</c>.</returns>
    internal void* GetUserData(ref UserDataKey key)
    {
        this.CheckDisposed();
        return cairo_pattern_get_user_data(this.Handle, ref key);
    }

    /// <summary>
    /// Gets or set the dithering mode of the rasterizer used for drawing shapes. This value is a hint,
    /// and a particular backend may or may not support a particular value. At the current time,
    /// only pixman is supported.
    /// </summary>
    public Dither Dither
    {
        get
        {
            this.CheckDisposed();
            return cairo_pattern_get_dither(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_pattern_set_dither(this.Handle, value);
        }
    }

    internal static Pattern? Lookup(void* pattern, bool owner = false)
    {
        if (pattern is null)
        {
            return null;
        }

        PatternType patternType = cairo_pattern_get_type(pattern);

        return patternType switch
        {
            PatternType.Solid        => new SolidPattern  (pattern, owner),
            PatternType.Surface      => new SurfacePattern(pattern, owner),
            PatternType.Linear       => new LinearGradient(pattern, owner),
            PatternType.Radial       => new RadialGradient(pattern, owner),
            PatternType.Mesh         => new Mesh          (pattern, owner),
            PatternType.RasterSource => new RasterSource  (pattern, owner),
            _                        => new Pattern       (pattern, owner)
        };
    }
}
