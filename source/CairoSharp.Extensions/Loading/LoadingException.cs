// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Loading;

[Serializable]
public abstract unsafe class LoadingException : Exception
{
    protected LoadingException() { }
    protected LoadingException(string message) : base(message) { }
    protected LoadingException(string message, Exception inner) : base(message, inner) { }

    private protected unsafe LoadingException(GError* error) : this(GetErrorMessage(error)) { }

    private static unsafe string GetErrorMessage(GError* error)
    {
        string msg = new(error->Message);

        // From the docs, e.g. https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/method.Handle.render_document.html
        // In case of error, the argument will be set to a newly allocated GError; the caller will take
        // ownership of the data, and be responsible for freeing it.
        LoadingNative.g_object_unref(error);

        return msg;
    }
}
