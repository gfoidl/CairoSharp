// (c) gfoidl, all rights reserved

using Cairo.Extensions.GObject;
using static Cairo.Extensions.Pango.PangoFontFaceNative;

namespace Cairo.Extensions.Pango;

/// <summary>
/// A <see cref="PangoFontFace"/> is used to represent a group of fonts with the same family,
/// slant, weight, and width, but varying sizes.
/// </summary>
public sealed unsafe class PangoFontFace : CairoObject<pango_font_face>
{
    private readonly PangoFontFamily _family;

    internal PangoFontFace(PangoFontFamily family, pango_font_face* face) : base(face, isOwnedByCairo: true, needsDestroy: false)
        => _family = family;

    protected override void DisposeCore(pango_font_face* handle)
        => throw new InvalidOperationException("PangoFontFace must not be freed");

    /// <summary>
    /// Gets a name representing the style of this face.
    /// </summary>
    /// <remarks>
    /// Note that a font family may contain multiple faces with the same name (e.g. a variable
    /// and a non-variable face for the same style).
    /// </remarks>
    public string FaceName
    {
        get
        {
            this.CheckDisposed();
            return pango_font_face_get_face_name(this.Handle);
        }
    }

    /// <summary>
    /// Returns whether a <see cref="PangoFontFace"/> is synthesized.
    /// </summary>
    /// <remarks>
    /// This will be the case if the underlying font rendering engine creates this face from
    /// another face, by shearing, emboldening, lightening or modifying it in some other way.
    /// </remarks>
    public bool IsSynthesized
    {
        get
        {
            this.CheckDisposed();
            return pango_font_face_is_synthesized(this.Handle);
        }
    }

    /// <summary>
    /// Gets the <see cref="Extensions.Pango.PangoFontFamily"/> that face belongs to.
    /// </summary>
    public PangoFontFamily FontFamily => _family;

    /// <summary>
    /// List the available sizes for a font.
    /// </summary>
    /// <returns>List of sizes for the font face, or an empty list for scalable fonts.</returns>
    /// <remarks>
    /// This is only applicable to bitmap fonts. The sizes returned are in cairo units and
    /// are sorted in ascending order.
    /// <para>
    /// Note: the native Pango API returns the values in Pango units, whilst this API
    /// returns cairo units.<br />
    /// <code>
    /// Pango unit = cairo unit * Pango.Scale
    /// </code>
    /// </para>
    /// </remarks>
    public FontFaceSizeIterator ListSizes()
    {
        this.CheckDisposed();
        return new FontFaceSizeIterator(this.Handle);
    }

    /// <summary>
    /// Enumerator for the sizes in the <see cref="PangoFontFace"/>.
    /// </summary>
    public struct FontFaceSizeIterator : IDisposable
    {
        private readonly pango_font_face* _face;

        private int* _sizes;
        private int  _count;
        private int  _i;

        internal FontFaceSizeIterator(pango_font_face* face) => _face = face;

        public readonly FontFaceSizeIterator GetEnumerator() => this;

        public readonly void Dispose()
        {
            if (_sizes is not null)
            {
                GObjectNative.g_free(_sizes);
            }
        }

        public readonly double Current
        {
            get
            {
                if (_i < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext() before accessing the first element");
                }

                return _sizes[_i] / (double)Pango.Scale;
            }
        }

        public bool MoveNext()
        {
            if (_i < 0)
            {
                fixed (int** sizes = &_sizes)
                fixed (int* count  = &_count)
                {
                    pango_font_face_list_sizes(_face, sizes, count);
                }

                // For scalable fonts, stores null at the location pointed to by sizes
                // and 0 at the location pointed to by n_sizes.
                if (_sizes is null || _count == 0)
                {
                    return false;
                }
            }
            else
            {
                _i++;
            }

            return _i < _count;
        }
    }
}
