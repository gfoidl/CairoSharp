// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.GObject;

[Serializable]
public abstract class GObjectException : Exception
{
    public int Domain { get; }
    public int Code   { get; }

    protected GObjectException() { }
    protected GObjectException(string message) : base(message) { }
    protected GObjectException(string message, Exception inner) : base(message, inner) { }

    private protected unsafe GObjectException(GError* error) : this(GetErrorMessage(error))
    {
        this.Domain = error->Domain;
        this.Code   = error->Code;
    }

    private static unsafe string GetErrorMessage(GError* error)
    {
        string msg = new(error->Message);

        // From the docs, e.g. https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/method.Handle.render_document.html
        // In case of error, the argument will be set to a newly allocated GError; the caller will take
        // ownership of the data, and be responsible for freeing it.
        GObjectNative.g_error_free(error);

        return msg;
    }
}
