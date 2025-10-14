// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Loading.PDF;

[Serializable]
public class PopplerException : LoadingException
{
    public PopplerException() { }
    public PopplerException(string message) : base(message) { }
    public PopplerException(string message, Exception inner) : base(message, inner) { }

    internal unsafe PopplerException(GError* error) : base(error) { }
}
