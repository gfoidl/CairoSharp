// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Loading.SVG;

[Serializable]
public class LibRsvgException : LoadingException
{
    public LibRsvgException() { }
    public LibRsvgException(string message) : base(message) { }
    public LibRsvgException(string message, Exception inner) : base(message, inner) { }

    internal unsafe LibRsvgException(GError* error) : base(error) { }
}
