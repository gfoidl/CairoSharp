// (c) gfoidl, all rights reserved

namespace Cairo.Fonts.FreeType;

/// <summary>
/// A helper type for <see cref="FreeTypeFont.SetSynthesize(Synthesize)"/> /
/// <see cref="FreeTypeFont.UnsetSynthesize(Synthesize)"/>.
/// </summary>
public readonly struct SynthesizeScope : IDisposable
{
    private readonly FreeTypeFont? _freeTypeFont;
    private readonly Synthesize    _flags;

    internal SynthesizeScope(FreeTypeFont freeTypeFont, Synthesize flags)
        => (_freeTypeFont, _flags) = (freeTypeFont, flags);

    /// <summary>
    /// Calls <see cref="FreeTypeFont.UnsetSynthesize(Synthesize)"/>
    /// </summary>
    public void Dispose() => _freeTypeFont?.UnsetSynthesize(_flags);
}
