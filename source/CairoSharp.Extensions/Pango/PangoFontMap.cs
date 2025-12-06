// (c) gfoidl, all rights reserved

using Cairo.Extensions.GObject;
using static Cairo.Extensions.Pango.PangoFontMapNative;

namespace Cairo.Extensions.Pango;

/// <summary>
/// A <see cref="PangoFontMap"/> represents the set of fonts available for a particular rendering system.
/// </summary>
/// <remarks>
/// This is a virtual object with implementations being specific to particular rendering systems.
/// </remarks>
public sealed unsafe class PangoFontMap : CairoObject<pango_font_map>
{
    private PangoFontMap(pango_font_map* fontMap) : base(fontMap, isOwnedByCairo: true, needsDestroy: false) { }

    /// <summary>
    /// Gets a default PangoCairoFontMap to use with cairo.
    /// </summary>
    /// <returns>
    /// The default PangoCairo fontmap for the current thread. This object is owned by Pango and must
    /// not be freed (note: you still should Dispose the object, but here disposal won't free the native
    /// resource).
    /// </returns>
    public static PangoFontMap CairoFontMapGetDefault()
    {
        pango_font_map* fontMap = pango_cairo_font_map_get_default();
        return new PangoFontMap(fontMap);
    }

    protected override void DisposeCore(pango_font_map* handle)
        => throw new InvalidOperationException("PangoFontMap must not be freed");

    /// <summary>
    /// Loads a font file with one or more fonts into the <see cref="PangoFontMap"/>.
    /// </summary>
    /// <param name="fileName">Absolute path to the font file.</param>
    /// <remarks>
    /// The added fonts will take precedence over preexisting fonts with the same name.
    /// </remarks>
    /// <exception cref="PangoException">An error occured while loading the font from the file.</exception>
    public void AddFontFile(string fileName)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(fileName);

        GError* error = null;

        if (!pango_font_map_add_font_file(this.Handle, fileName, &error))
        {
            throw new PangoException(error);
        }
    }

    /// <summary>
    /// List all families for a fontmap.
    /// </summary>
    /// <returns>All families for a fontmap.</returns>
    /// <remarks>
    /// Note that the returned families are not in any particular order.
    /// </remarks>
    public FontFamilyIterator ListFamilies()
    {
        this.CheckDisposed();
        return new FontFamilyIterator(this.Handle);
    }

    /// <summary>
    /// Enumerator for <see cref="PangoFontFamily"/>.
    /// </summary>
    public struct FontFamilyIterator : IDisposable
    {
        private readonly pango_font_map* _fontMap;

        private pango_font_family** _families;
        private int                 _count;
        private int                 _i = -1;

        internal FontFamilyIterator(pango_font_map* fontMap) => _fontMap = fontMap;

        public readonly FontFamilyIterator GetEnumerator() => this;

        public readonly void Dispose()
        {
            if (_families is not null)
            {
                GObjectNative.g_free(_families);
            }
        }

        public readonly PangoFontFamily Current
        {
            get
            {
                if (_i < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext() before accessing the first element");
                }

                pango_font_family* family = _families[_i];
                return new PangoFontFamily(family);
            }
        }

        public bool MoveNext()
        {
            if (_i < 0)
            {
                fixed (pango_font_family*** families = &_families)
                fixed (int* count                    = &_count)
                {
                    pango_font_map_list_families(_fontMap, families, count);
                }

                _i = 0;
            }
            else
            {
                _i++;
            }

            return _i < _count;
        }
    }
}
