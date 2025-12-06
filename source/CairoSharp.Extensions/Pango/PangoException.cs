// (c) gfoidl, all rights reserved

using Cairo.Extensions.GObject;

namespace Cairo.Extensions.Pango;

[Serializable]
public class PangoException : GObjectException
{
    public PangoException() { }
    public PangoException(string message) : base(message) { }
    public PangoException(string message, Exception inner) : base(message, inner) { }

    internal unsafe PangoException(GError* error) : base(error) { }
}
