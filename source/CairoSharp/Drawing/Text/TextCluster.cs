// (c) gfoidl, all rights reserved

using static Cairo.Drawing.Text.TextNative;

namespace Cairo.Drawing.Text;

/// <summary>
/// The <see cref="TextCluster"/> structure holds information about a single text cluster. A text cluster is
/// a minimal mapping of some glyphs corresponding to some UTF-8 text.
/// </summary>
/// <param name="Bytes">the number of bytes of UTF-8 text covered by cluster</param>
/// <param name="Glyphs">the number of glyphs covered by cluster</param>
/// <remarks>
/// For a cluster to be valid, both <paramref name="Bytes"/> and <paramref name="Glyphs"/> should be non-negative,
/// and at least one should be non-zero. Note that clusters with zero glyphs are not as well supported as normal
/// clusters. For example, PDF rendering applications typically ignore those clusters when PDF text is
/// being selected.
/// </remarks>
public readonly record struct TextCluster(int Bytes, int Glyphs)
{
    /// <summary>
    /// Allocates an array of <see cref="TextCluster"/>'s. This method is only useful in implementations of
    /// cairo_user_scaled_font_text_to_glyphs_func_t where the user needs to allocate an array of text clusters
    /// that cairo will free. For all other uses, user can use their own allocation method for text clusters.
    /// </summary>
    /// <param name="numberOfClusters">number of text_clusters to allocate</param>
    /// <returns> the newly allocated array of text clusters that should be freed using <see cref="CairoObject.Dispose()"/>()</returns>
    /// <remarks>
    /// This method returns <c>null</c> if <paramref name="numberOfClusters"/> is not positive, or if out of memory.
    /// That means, the <c>null</c> return value signals out-of-memory only if <paramref name="numberOfClusters"/> was positive.
    /// </remarks>
    public static unsafe TextClusterArray Allocate(int numberOfClusters)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfClusters);

        TextCluster* clusters = cairo_text_cluster_allocate(numberOfClusters);

        if (clusters is null)
        {
            CairoException.ThrowOutOfMemory();
        }

        return new TextClusterArray(clusters, numberOfClusters);
    }
}

/// <summary>
/// A <see cref="TextCluster"/>-array.
/// </summary>
public sealed unsafe class TextClusterArray : CairoObject
{
    private readonly int _numberOfClusters;

    internal TextClusterArray(TextCluster* clusters, int numberOfClusters)
        : base(clusters)
        => _numberOfClusters = numberOfClusters;

    protected override void DisposeCore(void* handle) => cairo_text_cluster_free((TextCluster*)handle);

    /// <summary>
    /// The span representation of the <see cref="TextCluster"/>s.
    /// </summary>
    public ReadOnlySpan<TextCluster> Span => new ReadOnlySpan<TextCluster>(this.Handle, _numberOfClusters);
}
