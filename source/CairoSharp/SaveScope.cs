// (c) gfoidl, all rights reserved

namespace Cairo;

/// <summary>
/// A helper type for <see cref="CairoContext.Save"/> / <see cref="CairoContext.Restore"/>.
/// </summary>
public ref struct SaveScope
{
    private CairoContext? _context;

    internal SaveScope(CairoContext context) => _context = context;

    /// <summary>
    /// Calls <see cref="CairoContext.Restore"/>
    /// </summary>
    public void Dispose()
    {
        _context?.Restore();
        _context = null;
    }
}
