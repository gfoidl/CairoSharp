// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Loading.SVG;

[Serializable]
public class LibRsvgException : Exception
{
    public LibRsvgException() { }
    public LibRsvgException(string message) : base(message) { }
    public LibRsvgException(string message, Exception inner) : base(message, inner) { }

    internal unsafe LibRsvgException(GError* error) : this(GetErrorMessage(error)) { }

    private static unsafe string GetErrorMessage(GError* error)
    {
        string msg = new(error->Message);

        // From the docs, e.g. https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/method.Handle.render_document.html
        // In case of error, the argument will be set to a newly allocated GError; the caller will take
        // ownership of the data, and be responsible for freeing it.
        LibRSvgNative.g_object_unref(error);

        return msg;
    }
}
