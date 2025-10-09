// (c) gfoidl, all rights reserved

namespace Cairo.Fonts;

/// <summary>
/// Specifies whether to hint font metrics; hinting font metrics means quantizing them so
/// that they are integer values in device space. Doing this improves the consistency of
/// letter and line spacing, however it also means that text will be laid out differently
/// at different zoom factors.
/// </summary>
public enum HintMetrics
{
    /// <summary>
    /// Hint metrics in the default manner for the font backend and target device, since 1.0
    /// </summary>
    Default,

    /// <summary>
    /// Do not hint font metrics, since 1.0
    /// </summary>
    Off,

    /// <summary>
    /// Hint font metrics, since 1.0
    /// </summary>
    On
}
