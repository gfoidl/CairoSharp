// (c) gfoidl, all rights reserved

namespace Cairo.Drawing.Text;

/// <summary>
/// Specifies properties of a text cluster mapping.
/// </summary>
public enum ClusterFlags
{
    None,

    /// <summary>
    /// The clusters in the cluster array map to glyphs in the glyph array from end to start. (Since 1.8)
    /// </summary>
    Backward = 1
}
