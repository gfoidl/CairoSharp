// (c) gfoidl, all rights reserved

using Cairo.Extensions.GObject;

namespace Cairo.Extensions.Loading;

[Serializable]
public abstract class LoadingException : GObjectException
{
    protected LoadingException() { }
    protected LoadingException(string message) : base(message) { }
    protected LoadingException(string message, Exception inner) : base(message, inner) { }

    private protected unsafe LoadingException(GError* error) : base(error) { }
}
