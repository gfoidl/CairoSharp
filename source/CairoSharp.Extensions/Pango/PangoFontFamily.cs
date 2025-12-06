// (c) gfoidl, all rights reserved

using Cairo.Extensions.GObject;
using static Cairo.Extensions.Pango.PangoFontFamilyNative;

namespace Cairo.Extensions.Pango;

/// <summary>
/// A <see cref="PangoFontFamily"/> is used to represent a family of related font faces.
/// </summary>
/// <remarks>
/// The font faces in a family share a common design, but differ in slant, weight, width
/// or other aspects.
/// </remarks>
public sealed unsafe class PangoFontFamily : CairoObject<pango_font_family>
{
    internal PangoFontFamily(pango_font_family* family) : base(family, isOwnedByCairo: true, needsDestroy: false) { }

    protected override void DisposeCore(pango_font_family* handle)
        => throw new InvalidOperationException("PangoFontFamily must not be freed");

    /// <summary>
    /// Gets the name of the family.
    /// </summary>
    /// <remarks>
    /// The name is unique among all fonts for the font backend and can be used in a
    /// PangoFontDescription to specify that a face from this family is desired.
    /// </remarks>
    public string Name
    {
        get
        {
            this.CheckDisposed();
            return pango_font_family_get_name(this.Handle);
        }
    }

    /// <summary>
    /// A monospace font is a font designed for text display where the characters form a regular grid.
    /// </summary>
    /// <remarks>
    /// For Western languages this would mean that the advance width of all characters are the same, but
    /// this categorization also includes Asian fonts which include double-width characters: characters
    /// that occupy two grid cells. g_unichar_iswide() returns a result that indicates whether a character
    /// is typically double-width in a monospace font.
    /// </remarks>
    public bool IsMonospace
    {
        get
        {
            this.CheckDisposed();
            return pango_font_family_is_monospace(this.Handle);
        }
    }

    /// <summary>
    /// A variable font is a font which has axes that can be modified to produce different faces.
    /// </summary>
    /// <remarks>
    /// Such axes are also known as variations.
    /// </remarks>
    public bool IsVariable
    {
        get
        {
            this.CheckDisposed();
            return pango_font_family_is_variable(this.Handle);
        }
    }

    public override string ToString() => $"""
        Name:        {this.Name}
        IsMonospace: {this.IsMonospace}
        IsVariable:  {this.IsVariable}
        """;

    /// <summary>
    /// Gets the <see cref="PangoFontFace"/> of family with the given name.
    /// </summary>
    /// <param name="name">
    /// The name of a face. If the name is <c>null</c>, the family’s default face
    /// (fontconfig calls it "Regular") will be returned.
    /// </param>
    /// <returns>
    /// The <see cref="PangoFontFace"/>, or <c>null</c> if no face with the given name exists.
    /// </returns>
    public PangoFontFace? GetFace(string? name)
    {
        this.CheckDisposed();

        pango_font_face* face = pango_font_family_get_face(this.Handle, name);

        return face is not null
            ? new PangoFontFace(this, face)
            : null;
    }

    /// <summary>
    /// Lists the different font faces that make up <see cref="PangoFontFamily"/>.
    /// </summary>
    /// <returns>List of font faces for the family.</returns>
    /// <remarks>
    /// The faces in a family share a common design, but differ in slant, weight, width
    /// and other aspects.
    /// <para>
    /// Note that the returned faces are not in any particular order, and multiple faces may
    /// have the same name or characteristics.
    /// </para>
    /// </remarks>
    public FontFaceIterator ListFaces()
    {
        this.CheckDisposed();
        return new FontFaceIterator(this);
    }

    /// <summary>
    /// Enumerator for <see cref="PangoFontFace"/> for the <see cref="PangoFontFamily"/>.
    /// </summary>
    public struct FontFaceIterator : IDisposable
    {
        private readonly PangoFontFamily _family;

        private pango_font_face** _faces;
        private int               _count;
        private int               _i = -1;

        internal FontFaceIterator(PangoFontFamily family) => _family = family;

        public readonly FontFaceIterator GetEnumerator() => this;

        public readonly void Dispose()
        {
            if (_faces is not null)
            {
                GObjectNative.g_free(_faces);
            }
        }

        public readonly PangoFontFace Current
        {
            get
            {
                if (_i < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext() before accessing the first element");
                }

                pango_font_face* face = _faces[_i];
                return new PangoFontFace(_family, face);
            }
        }

        public bool MoveNext()
        {
            if (_i < 0)
            {
                fixed (pango_font_face*** faces = &_faces)
                fixed (int* count               = &_count)
                {
                    pango_font_family_list_faces(_family.Handle, faces, count);
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
