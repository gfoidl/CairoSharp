// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.Tee.TeeSurfaceNative;

namespace Cairo.Surfaces.Tee;

/// <summary>
/// Tee surface — Redirect input to multiple surfaces
/// </summary>
/// <remarks>
/// The "tee" surface supports redirecting all its input to multiple surfaces.
/// </remarks>
public sealed unsafe class TeeSurface : Surface
{
    /// <summary>
    /// Creates a new "tee" surface.
    /// </summary>
    /// <param name="primary">the primary <see cref="Surface"/></param>
    /// <remarks>
    /// The primary surface is used when querying surface options, like font options and extents.
    /// <para>
    /// Operations performed on the tee surface will be replayed on any surface added to it.
    /// </para>
    /// </remarks>
    public TeeSurface(Surface primary) : base(cairo_tee_surface_create(primary.Handle)) { }

    /// <summary>
    /// Adds a new target surface to the list of replicas of a tee surface.
    /// </summary>
    /// <param name="target">the surface to add</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <c>null</c></exception>
    public void Add(Surface target)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(target);

        cairo_tee_surface_add(this.Handle, target.Handle);
    }

    /// <summary>
    /// Retrieves the replica surface at the given index.
    /// </summary>
    /// <param name="index">the index of the replica to retrieve</param>
    /// <returns>the surface at the given index</returns>
    /// <remarks>
    /// The primary surface used to create the <see cref="TeeSurface"/> is always set at the zero index.
    /// </remarks>
    public Surface this[int index]
    {
        get
        {
            this.CheckDisposed();

            void* handle = cairo_tee_surface_index(this.Handle, (uint)index);
            return new Surface(handle, isOwnedByCairo: true, needsDestroy: false);
        }
    }

    /// <summary>
    /// Removes the given surface from the list of replicas of a tee surface.
    /// </summary>
    /// <param name="target">the surface to remove</param>
    public void Remove(Surface target)
    {
        this.CheckDisposed();
        cairo_tee_surface_remove(this.Handle, target.Handle);
    }
}
