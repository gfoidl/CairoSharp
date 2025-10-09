// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.Recording.RecordingSurfaceNative;

namespace Cairo.Surfaces.Recording;

/// <summary>
/// Recording Surfaces — Records all drawing operations
/// </summary>
/// <remarks>
/// A recording surface is a surface that records all drawing operations at the highest level
/// of the surface backend interface, (that is, the level of paint, mask, stroke, fill, and
/// show_text_glyphs). The recording surface can then be "replayed" against any target
/// surface by using it as a source surface.
/// <para>
/// If you want to replay a surface so that the results in target will be identical to the results
/// that would have been obtained if the original operations applied to the recording surface had
/// instead been applied to the target surface, you can see the example code in
/// <a href="https://www.cairographics.org/manual/cairo-Recording-Surfaces.html">cairo docs</a>.
/// </para>
/// <para>
/// A recording surface is logically unbounded, i.e. it has no implicit constraint on the size of the
/// drawing surface. However, in practice this is rarely useful as you wish to replay against a
/// particular target surface with known bounds. For this case, it is more efficient to specify the
/// target extents to the recording surface upon creation.
/// </para>
/// <para>
/// The recording phase of the recording surface is careful to snapshot all necessary objects
/// (paths, patterns, etc.), in order to achieve accurate replay. The efficiency of the recording
/// surface could be improved by improving the implementation of snapshot for the various objects.
/// For example, it would be nice to have a copy-on-write implementation for _cairo_surface_snapshot.
/// </para>
/// </remarks>
public sealed unsafe class RecordingSurface : Surface
{
    /// <summary>
    /// Creates a recording-surface which can be used to record all drawing operations at the
    /// highest level (that is, the level of paint, mask, stroke, fill and show_text_glyphs).
    /// The recording surface can then be "replayed" against any target surface by using it
    /// as a source to drawing operations.
    /// </summary>
    /// <param name="content">the content of the recording surface</param>
    /// <param name="extents">the extents to record in pixels</param>
    /// <remarks>
    /// The recording phase of the recording surface is careful to snapshot all necessary
    /// objects (paths, patterns, etc.), in order to achieve accurate replay.
    /// </remarks>
    public RecordingSurface(Content content, Rectangle extents)
        : base(cairo_recording_surface_create(content, &extents), owner: true) { }

    /// <summary>
    /// Creates a recording-surface similar to <see cref="RecordingSurface(Content, Rectangle)"/>,
    /// but this one records unbounded operations.
    /// </summary>
    /// <param name="content">the content of the recording surface</param>
    public RecordingSurface(Content content)
        : base(cairo_recording_surface_create(content, null), owner: true) { }

    /// <summary>
    /// Measures the extents of the operations stored within the recording-surface. This
    /// is useful to compute the required size of an image surface (or equivalent) into which
    /// to replay the full sequence of drawing operations.
    /// </summary>
    /// <returns>A rectangle for the ink bounding box</returns>
    public Rectangle GetInkExtents()
    {
        cairo_recording_surface_ink_extents(this.Handle, out double x0, out double y0, out double width, out double height);

        return new Rectangle(x0, y0, width, height);
    }

    /// <summary>
    /// Get the extents of the recording-surface.
    /// </summary>
    /// <param name="extents">
    /// the <see cref="Rectangle"/> to be assigned the extents
    /// </param>
    /// <returns>
    /// <c>true</c> if the surface is bounded, of recording type, and not in
    /// an error state, otherwise <c>false</c>
    /// </returns>
    public bool TryGetExtents(out Rectangle extents) => cairo_recording_surface_get_extents(this.Handle, out extents);
}
