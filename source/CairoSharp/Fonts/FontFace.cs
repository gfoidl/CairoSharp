// (c) gfoidl, all rights reserved

using System.ComponentModel;
using System.Diagnostics;
using static Cairo.Fonts.FontFaceNative;
using unsafe ReferenceFunc = delegate*<Cairo.Fonts.cairo_font_face_t*, Cairo.Fonts.cairo_font_face_t*>;

namespace Cairo.Fonts;

[EditorBrowsable(EditorBrowsableState.Never)]
public struct cairo_font_face_t;

/// <summary>
/// cairo_font_face_t — Base class for font faces
/// </summary>
/// <remarks>
/// cairo_font_face_t represents a particular font at a particular weight, slant, and other
/// characteristic but no size, transformation, or size.
/// <para>
/// Font faces are created using font-backend-specific constructors, typically of the
/// form cairo_backend_font_face_create(), or implicitly using the toy text API by way
/// of cairo_select_font_face(). The resulting face can be accessed using cairo_get_font_face().
/// </para>
/// </remarks>
public unsafe class FontFace : CairoObject<cairo_font_face_t>
{
    protected internal FontFace(cairo_font_face_t* fontFace, bool isOwnedByCairo = false, bool needsDestroy = true, ReferenceFunc referenceFunc = null)
        : base(fontFace, isOwnedByCairo, needsDestroy)
    {
        this.Status.ThrowIfNotSuccess();

        if (isOwnedByCairo && needsDestroy)
        {
            if (referenceFunc is null)
            {
                cairo_font_face_reference(fontFace);
            }
            else
            {
                referenceFunc(fontFace);
            }
        }
    }

    protected override void DisposeCore(cairo_font_face_t* fontFace)
    {
        cairo_font_face_destroy(fontFace);

        PrintDebugInfo(fontFace);
        [Conditional("DEBUG")]
        static void PrintDebugInfo(cairo_font_face_t* fontFace)
        {
            uint rc = cairo_font_face_get_reference_count(fontFace);
            Debug.WriteLine($"FontFace 0x{(nint)fontFace}: reference count = {rc}");
        }
    }

    /// <summary>
    /// Checks whether an error has previously occurred for this font face
    /// </summary>
    /// <remarks>
    /// <see cref="Status.Success"/> or another error such as <see cref="Status.NoMemory"/>.
    /// </remarks>
    public Status Status
    {
        get
        {
            this.CheckDisposed();
            return cairo_font_face_status(this.Handle);
        }
    }

    /// <summary>
    /// This property returns the type of the backend used to create a font face. See <see cref="FontType"/> for available types.
    /// </summary>
    public FontType FontType
    {
        get
        {
            this.CheckDisposed();
            return cairo_font_face_get_type(this.Handle);
        }
    }

    /// <summary>
    /// Returns the current reference count of <see cref="FontFace"/>.
    /// </summary>
    /// <remarks>
    /// If the object is a nil object, 0 will be returned.
    /// </remarks>
    internal int ReferenceCount
    {
        get
        {
            this.CheckDisposed();
            return (int)cairo_font_face_get_reference_count(this.Handle);
        }
    }

    /// <summary>
    /// Attach user data to <see cref="FontFace"/>.
    /// </summary>
    /// <param name="key">the address of a <see cref="UserDataKey"/> to attach the user data to</param>
    /// <param name="userData">the user data to attach to the font face</param>
    /// <param name="destroyFunction">
    /// a cairo_destroy_func_t which will be called when the font face is destroyed or when new
    /// user data is attached using the same key.
    /// </param>
    /// <remarks>
    /// To remove user data from a font face, call this method with the key that was used to set
    /// it and <c>null</c> for <paramref name="userData"/>.
    /// </remarks>
    internal void SetUserData(UserDataKey* key, void* userData, cairo_destroy_func_t destroyFunction)
    {
        this.CheckDisposed();

        Status status = cairo_font_face_set_user_data(this.Handle, key, userData, destroyFunction);
        status.ThrowIfNotSuccess();
    }

    /// <summary>
    /// Return user data previously attached to <see cref="FontFace"/> using the specified key.
    /// </summary>
    /// <param name="key">the address of the <see cref="UserDataKey"/> the user data was attached to</param>
    /// <returns> the user data previously attached or <c>null</c></returns>
    /// <remarks>
    /// If no user data has been attached with the given key this method returns <c>null</c>.
    /// </remarks>
    internal void* GetUserData(UserDataKey* key)
    {
        this.CheckDisposed();
        return cairo_font_face_get_user_data(this.Handle, key);
    }
}
