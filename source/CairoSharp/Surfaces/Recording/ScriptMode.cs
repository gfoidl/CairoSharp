// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces.Recording;

/// <summary>
/// A set of script output variants.
/// </summary>
public enum ScriptMode
{
    /// <summary>
    /// the output will be in readable text (default). (Since 1.12)
    /// </summary>
    Ascii,

    /// <summary>
    /// the output will use byte codes. (Since 1.12)
    /// </summary>
    Binary
}
