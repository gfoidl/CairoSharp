// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cairo.Drawing.Patterns;
using Cairo.Drawing.Text;
using Cairo.Fonts.Scaled;
using static Cairo.Drawing.Text.TextNative;
using static Cairo.Fonts.FontFaceNative;
using static Cairo.Fonts.Scaled.ScaledFontNative;
using static Cairo.Fonts.User.UserFontNative;

namespace Cairo.Fonts.User;

// https://www.cairographics.org/manual/cairo-User-Fonts.html

/// <summary>
/// User Fonts — Font support with font data provided by the user
/// </summary>
/// <remarks>
/// The user-font feature allows the cairo user to provide drawings for glyphs in a font.
/// This is most useful in implementing fonts in non-standard formats, like SVG fonts and
/// Flash fonts, but can also be used by games and other application to draw "funky" fonts.
/// </remarks>
public sealed unsafe class UserFont : FontFace
{
    internal UserFont(cairo_font_face_t* fontFace, bool isOwnedByCairo = false, bool needsDestroy = true)
        : base(fontFace, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// <see cref="Init"/> is the type of delegate which is called when a scaled-font needs to be created
    /// for a user font-face.
    /// </summary>
    /// <param name="scaledFont">the scaled-font being created</param>
    /// <param name="cairoContext">a cairo context, in font space</param>
    /// <param name="fontExtents">font extents to fill in, in font space</param>
    /// <returns>
    /// <see cref="Status.Success"/> on success, or one of the <see cref="Status"/> error codes for failure.
    /// </returns>
    /// <remarks>
    /// The cairo context cr is not used by the caller, but is prepared in font space, similar to what the
    /// cairo contexts passed to the <see cref="RenderGlyph"/> delegate will look like. The callback can use
    /// this context for extents computation for example. After the callback is called, cr is checked for any
    /// error status.
    /// <para>
    /// The extents argument is where the user font sets the font extents for scaled_font. It is in font space,
    /// which means that for most cases its ascent and descent members should add to 1.0. extents is preset to hold
    /// a value of 1.0 for ascent, height, and max_x_advance, and 0.0 for descent and max_y_advance members.
    /// </para>
    /// <para>
    /// The callback is optional. If not set, default font extents as described in the previous paragraph will be used.
    /// </para>
    /// <para>
    /// Note that <paramref name="scaledFont"/> is not fully initialized at this point and trying to use it for text
    /// operations in the callback will result in deadlock.
    /// </para>
    /// </remarks>
    public delegate Status Init(ScaledFont scaledFont, CairoContext cairoContext, ref FontExtents fontExtents);

    /// <summary>
    /// <see cref="RenderGlyph"/> is the type of delegate which is called when a user scaled-font needs to render a glyph.
    /// </summary>
    /// <param name="scaledFont">user scaled-font</param>
    /// <param name="glyph">glyph code to render</param>
    /// <param name="cairoContext">cairo context to draw to, in font space</param>
    /// <param name="textExtents">glyph extents to fill in, in font space</param>
    /// <returns>
    /// <see cref="Status.Success"/> upon success, <see cref="Status.UserFontNotImplemented"/> if fallback options should
    /// be tried, or <see cref="Status.UserFontError"/> or any other error status on error.
    /// </returns>
    /// <remarks>
    /// The callback is mandatory, and expected to draw the glyph with code glyph to the cairo context cr. cr is
    /// prepared such that the glyph drawing is done in font space. That is, the matrix set on cr is the scale
    /// matrix of scaled_font. The extents argument is where the user font sets the font extents for scaled_font.
    /// However, if user prefers to draw in user space, they can achieve that by changing the matrix on cr.
    /// <para>
    /// All cairo rendering operations to cr are permitted. However, when this callback is set with
    /// cairo_user_font_face_set_render_glyph_func(), the result is undefined if any source other than the default
    /// source on cr is used. That means, glyph bitmaps should be rendered using
    /// <see cref="CairoContext.Mask(Pattern)"/> instead of <see cref="CairoContext.Paint"/>.
    /// </para>
    /// <para>
    /// When this callback is set with cairo_user_font_face_set_render_color_glyph_func(), the default source
    /// is black. Setting the source is a valid operation. cairo_user_scaled_font_get_foreground_marker() or
    /// cairo_user_scaled_font_get_foreground_source() may be called to obtain the current source at the time the
    /// glyph is rendered.
    /// </para>
    /// <para>
    /// Other non-default settings on cr include a font size of 1.0 (given that it is set up to be in font space),
    /// and font options corresponding to <paramref name="scaledFont"/>.
    /// </para>
    /// <para>
    /// The extents argument is preset to have x_bearing, width, and y_advance of zero, y_bearing set to -font_extents.ascent,
    /// height to font_extents.ascent+font_extents.descent, and x_advance to font_extents.max_x_advance. The only field
    /// user needs to set in majority of cases is x_advance. If the width field is zero upon the callback returning
    /// (which is its preset value), the glyph extents are automatically computed based on the drawings done to cr.
    /// This is in most cases exactly what the desired behavior is. However, if for any reason the callback sets the
    /// extents, it must be ink extents, and include the extents of all drawing done to cr in the callback.
    /// </para>
    /// <para>
    /// Where both color and non-color callbacks has been set using cairo_user_font_face_set_render_color_glyph_func(),
    /// and cairo_user_font_face_set_render_glyph_func(), the color glyph callback will be called first. If the color
    /// glyph callback returns <see cref="Status.UserFontNotImplemented"/>, any drawing operations are discarded and the
    /// non-color callback will be called. This is the only case in which the <see cref="Status.UserFontNotImplemented"/>
    /// may be returned from a render callback. This fallback sequence allows a user font face to contain a combination
    /// of both color and non-color glyphs.
    /// </para>
    /// </remarks>
    public delegate Status RenderGlyph(ScaledFont scaledFont, int glyph, CairoContext cairoContext, ref TextExtents textExtents);

    /// <summary>
    /// <see cref="TextToGlyphs"/> is the type of delegate which is called to convert input text to an array of glyphs.
    /// This is used by the cairo_show_text() operation.
    /// </summary>
    /// <param name="scaledFont">the scaled-font being created</param>
    /// <param name="text">a string of text</param>
    /// <param name="glyphs">pointer to array of glyphs to fill, in font space</param>
    /// <param name="clusters">pointer to array of cluster mapping information to fill</param>
    /// <param name="clusterFlags">
    /// pointer to location to store cluster flags corresponding to the <paramref name="clusters"/>
    /// </param>
    /// <param name="useClusterMapping">whether cluster mapping should be used or not</param>
    /// <returns>
    /// <see cref="Status.Success"/> upon success, <see cref="Status.UserFontNotImplemented"/> if fallback options should
    /// be tried, or <see cref="Status.UserFontError"/> or any other error status on error.
    /// </returns>
    /// <remarks>
    /// Using this callback the user-font has full control on glyphs and their positions. That means, it allows
    /// for features like ligatures and kerning, as well as complex shaping required for scripts like Arabic and Indic.
    /// <para>
    /// The callback should populate the glyph indices and positions (in font space) assuming that the text is
    /// to be shown at the origin.
    /// </para>
    /// <para>
    /// The callback is optional. If num_glyphs is negative upon the callback returning or if the return value is
    /// <see cref="Status.UserFontNotImplemented"/>, the <see cref="UnicodeToGlyph"/> callback is tried.
    /// See <see cref="UnicodeToGlyph"/>.
    /// </para>
    /// <para>
    /// Note: While cairo does not impose any limitation on glyph indices, some applications may assume that a glyph
    /// index fits in a 16-bit unsigned integer. As such, it is advised that user-fonts keep their glyphs in the
    /// 0 to 65535 range. Furthermore, some applications may assume that glyph 0 is a special glyph-not-found glyph.
    /// User-fonts are advised to use glyph 0 for such purposes and do not use that glyph value for other purposes.
    /// </para>
    /// </remarks>
    public delegate Status TextToGlyphs(ScaledFont scaledFont, string text, out GlyphArray glyphs, out TextClusterArray? clusters, out ClusterFlags clusterFlags, bool useClusterMapping = true);

    /// <summary>
    /// <see cref="UnicodeToGlyph"/> is the type of delegate which is called to convert an input Unicode character
    /// to a single glyph. This is used by the cairo_show_text() operation.
    /// </summary>
    /// <param name="scaledFont">the scaled-font being created</param>
    /// <param name="unicode">input unicode character code-point</param>
    /// <param name="glyphIndex">output glyph index</param>
    /// <returns>
    /// <see cref="Status.Success"/> upon success, <see cref="Status.UserFontNotImplemented"/> if fallback options should
    /// be tried, or <see cref="Status.UserFontError"/> or any other error status on error.
    /// </returns>
    /// <remarks>
    /// This callback is used to provide the same functionality as the <see cref="TextToGlyphs"/> callback does,
    /// but has much less control on the output, in exchange for increased ease of use. The inherent assumption to
    /// using this callback is that each character maps to one glyph, and that the mapping is context independent.
    /// It also assumes that glyphs are positioned according to their advance width. These mean no ligatures,
    /// kerning, or complex scripts can be implemented using this callback.
    /// <para>
    /// The callback is optional, and only used if <see cref="TextToGlyphs"/> callback is not set or fails to return glyphs.
    /// If this callback is not set or if it returns <see cref="Status.UserFontNotImplemented"/>, an identity mapping from
    /// Unicode code-points to glyph indices is assumed.
    /// </para>
    /// <para>
    /// Note: While cairo does not impose any limitation on glyph indices, some applications may assume that a glyph
    /// index fits in a 16-bit unsigned integer. As such, it is advised that user-fonts keep their glyphs in the
    /// 0 to 65535 range. Furthermore, some applications may assume that glyph 0 is a special glyph-not-found glyph.
    /// User-fonts are advised to use glyph 0 for such purposes and do not use that glyph value for other purposes.
    /// </para>
    /// </remarks>
    public delegate Status UnicodeToGlyph(ScaledFont scaledFont, int unicode, out int glyphIndex);

    private GCHandle _stateHandle;          // mutable struct
    private static UserDataKey s_stateKey;  // mutable struct, don't make readonly

    /// <summary>
    /// Creates a new user font-face.
    /// </summary>
    /// <param name="init">init callback</param>
    /// <param name="renderGlyph">callback for rendering a glyph</param>
    /// <param name="textToGlyphs">callback for text to glyphs</param>
    /// <param name="renderColorGlyph">callback for text to colored glyph</param>
    /// <param name="unicodeToGlyph">callback for unicode to glyph</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="renderGlyph"/> is <c>null</c>
    /// </exception>
    public UserFont(
        Init?           init,
        RenderGlyph     renderGlyph,
        RenderGlyph?    renderColorGlyph = null,
        TextToGlyphs?   textToGlyphs     = null,
        UnicodeToGlyph? unicodeToGlyph   = null)
        : base(cairo_user_font_face_create())
    {
        ArgumentNullException.ThrowIfNull(renderGlyph);

        State state  = new() { RenderGlyph = renderGlyph };
        _stateHandle = GCHandle.Alloc(state, GCHandleType.Normal);

        cairo_user_font_face_set_render_glyph_func(this.Handle, &RenderGlyphCore);

        if (init is not null)
        {
            state.Init = init;
            cairo_user_font_face_set_init_func(this.Handle, &InitCore);
        }

        if (renderColorGlyph is not null)
        {
            state.RenderColorGlyph = renderColorGlyph;
            cairo_user_font_face_set_render_color_glyph_func(this.Handle, &RenderColorGlyphCore);
        }

        if (textToGlyphs is not null)
        {
            state.TextToGlyphs = textToGlyphs;
            cairo_user_font_face_set_text_to_glyphs_func(this.Handle, &TextToGlyphsCore);
        }

        if (unicodeToGlyph is not null)
        {
            state.UnicodeToGlyph = unicodeToGlyph;
            cairo_user_font_face_set_unicode_to_glyph_func(this.Handle, &UnicodeToGlyphCore);
        }

        cairo_font_face_set_user_data(this.Handle, ref s_stateKey, GCHandle.ToIntPtr(_stateHandle).ToPointer(), &Destroy);

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void Destroy(void* data)
        {
            // nop
        }
    }

    protected override void DisposeCore(cairo_font_face_t* fontFace)
    {
        base.DisposeCore(fontFace);

        if (_stateHandle.IsAllocated)
        {
            _stateHandle.Free();
        }
    }

    /// <summary>
    /// Gets the foreground pattern of the glyph currently being rendered. A <see cref="RenderGlyph"/> delegate that
    /// has been set may call this property to retrieve the current foreground pattern for the glyph being rendered.
    /// The property should not be called outside of a callback.
    /// </summary>
    /// <remarks>
    /// The foreground marker pattern contains an internal marker to indicate that it is to be substituted with the
    /// current source when rendered to a surface. Querying the foreground marker will reveal a solid black color,
    /// however this is not representative of the color that will actually be used. Similarly, setting a solid black
    /// color will render black, not the foreground pattern when the glyph is painted to a surface. Using the foreground
    /// marker as the source instead of <see cref="ForegroundSource"/> in a color render callback has the following benefits:
    /// <list type="number">
    /// <item>
    /// Cairo only needs to call the render callback once as it can cache the recording. Cairo will substitute the
    /// actual foreground color when rendering the recording.
    /// </item>
    /// <item>
    /// On backends that have the concept of a foreground color in fonts such as PDF, PostScript, and SVG, cairo can
    /// generate more optimal output. The glyph can be included in an embedded font.
    /// </item>
    /// </list>
    /// The one drawback of the using foreground marker is the render callback can not access the color components
    /// of the pattern as the actual foreground pattern is not available at the time the render callback is invoked.
    /// If the render callback needs to query the foreground pattern, use <see cref="ForegroundSource"/>.
    /// </remarks>
    public Pattern? ForegroundMarker
    {
        get
        {
            this.CheckDisposed();

            cairo_pattern_t* handle = cairo_user_scaled_font_get_foreground_marker((cairo_scaled_font_t*)this.Handle);
            return Pattern.Lookup(handle, isOwnedByCairo: true);
        }
    }

    /// <summary>
    /// Gets the foreground pattern of the glyph currently being rendered. A <see cref="RenderGlyph"/> delegate that has been
    /// set may call this property to retrieve the current foreground pattern for the glyph being rendered. The property
    /// should not be called outside of a callback.
    /// </summary>
    /// <remarks>
    /// This property returns the current source at the time the glyph is rendered. Compared with
    /// <see cref="ForegroundMarker"/>, this property returns the actual source pattern that will be used to render
    /// the glyph. The render callback is free to query the pattern and extract color components or other pattern data.
    /// For example if the render callback wants to create a gradient stop based on colors in the foreground source pattern,
    /// it will need to use this property in order to be able to query the colors in the foreground pattern.
    /// <para>
    /// While this property does not have the restrictions on using the pattern that <see cref="ForegroundMarker"/> has, it
    /// does incur a performance penalty. If a render callback calls this property:
    /// <list type="number">
    /// <item>
    /// Cairo will call the render callback whenever the current pattern of the context in which the glyph is rendered changes.
    /// </item>
    /// <item>
    /// On backends that support font embedding (PDF, PostScript, and SVG), cairo can not embed this glyph in a font.
    /// Instead the glyph will be emitted as an image or sequence of drawing operations each time it is used.
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    public Pattern? ForegroundSource
    {
        get
        {
            this.CheckDisposed();

            cairo_pattern_t* handle = cairo_user_scaled_font_get_foreground_source((cairo_scaled_font_t*)this.Handle);
            return Pattern.Lookup(handle, isOwnedByCairo: true);
        }
    }

    private static State GetState(cairo_scaled_font_t* scaledFont)
    {
        cairo_font_face_t* fontFace    = cairo_scaled_font_get_font_face(scaledFont);
        void* userData                 = cairo_font_face_get_user_data(fontFace, ref s_stateKey);
        GCHandle gcHandle              = GCHandle.FromIntPtr(new IntPtr(userData));

        Debug.Assert(gcHandle.IsAllocated);

        return (State)gcHandle.Target!;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static Status InitCore(cairo_scaled_font_t* scaledFont, cairo_t* cr, FontExtents* fontExtents)
    {
        State state = GetState(scaledFont);

        Debug.Assert(state.Init is not null);

        // Here just wrapper objects, w/o memory management, thus no Dispose needed.
        ScaledFont sf        = new(scaledFont, isOwnedByCairo: true, needsDestroy: false);
        CairoContext context = new(cr        , isOwnedByCairo: true, needsDestroy: false);

        return state.Init(sf, context, ref Unsafe.AsRef<FontExtents>(fontExtents));
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static Status RenderGlyphCore(cairo_scaled_font_t* scaledFont, CULong glyph, cairo_t* cr, TextExtents* textExtents)
    {
        State state = GetState(scaledFont);

        // Here just wrapper objects, w/o memory management, thus no Dispose needed.
        ScaledFont sf        = new(scaledFont, isOwnedByCairo: true, needsDestroy: false);
        CairoContext context = new(cr        , isOwnedByCairo: true, needsDestroy: false);

        return state.RenderGlyph(sf, (int)glyph.Value, context, ref Unsafe.AsRef<TextExtents>(textExtents));
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static Status RenderColorGlyphCore(cairo_scaled_font_t* scaledFont, CULong glyph, cairo_t* cr, TextExtents* textExtents)
    {
        State state = GetState(scaledFont);

        if (state.RenderColorGlyph is null)
        {
            return Status.UserFontNotImplemented;
        }

        // Here just wrapper objects, w/o memory management, thus no Dispose needed.
        ScaledFont sf        = new(scaledFont, isOwnedByCairo: true, needsDestroy: false);
        CairoContext context = new(cr        , isOwnedByCairo: true, needsDestroy: false);

        return state.RenderColorGlyph(sf, (int)glyph.Value, context, ref Unsafe.AsRef<TextExtents>(textExtents));
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static Status TextToGlyphsCore(
        cairo_scaled_font_t* scaledFont,
        byte*                utf8,
        int                  utf8Len,
        Glyph**              glyphs,
        int*                 numGlyphs,
        TextCluster**        clusters,
        int*                 numClusters,
        ClusterFlags*        clusterFlags)
    {
        State state = GetState(scaledFont);

        if (state.TextToGlyphs is null)
        {
            clusterFlags = default;
            return Status.UserFontNotImplemented;
        }

        // Here just wrapper objects, w/o memory management, thus no Dispose needed.
        ScaledFont sf = new(scaledFont, isOwnedByCairo: true, needsDestroy: false);

        string text            = new((sbyte*)utf8, 0, utf8Len);
        bool useClusterMapping = clusters is not null;

        Status status = state.TextToGlyphs(sf, text, out GlyphArray glyphArray, out TextClusterArray? clusterArray, out ClusterFlags clusterFlagsTmp, useClusterMapping);
        *clusterFlags = clusterFlagsTmp;

        try
        {
            ReadOnlySpan<Glyph> glyphSpan = glyphArray.Span;
            if (*numGlyphs < glyphSpan.Length)
            {
                *glyphs = cairo_glyph_allocate(glyphSpan.Length);
            }
            *numGlyphs = glyphSpan.Length;
            glyphSpan.CopyTo(new Span<Glyph>(*glyphs, *numGlyphs));

            if (useClusterMapping && clusterArray is not null)
            {
                ReadOnlySpan<TextCluster> clusterSpan = clusterArray.Span;
                if (*numClusters < clusterSpan.Length)
                {
                    *clusters = cairo_text_cluster_allocate(clusterSpan.Length);
                }
                *numClusters = clusterSpan.Length;
                clusterSpan.CopyTo(new Span<TextCluster>(*clusters, *numClusters));
            }

            return status;
        }
        finally
        {
            glyphArray   .Dispose();
            clusterArray?.Dispose();
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static Status UnicodeToGlyphCore(cairo_scaled_font_t* scaledFont, CULong unicode, CULong* glyphIndex)
    {
        State state = GetState(scaledFont);

        if (state.UnicodeToGlyph is null)
        {
            glyphIndex = default;
            return Status.UserFontNotImplemented;
        }

        // Here just wrapper objects, w/o memory management, thus no Dispose needed.
        ScaledFont sf = new(scaledFont, isOwnedByCairo: true, needsDestroy: false);

        Status status = state.UnicodeToGlyph(sf, (int)unicode.Value, out int gi);

        *glyphIndex = new CULong((uint)gi);
        return status;
    }

    private class State
    {
        public Init? Init                       { get; set; }
        public required RenderGlyph RenderGlyph { get; set; }
        public RenderGlyph? RenderColorGlyph    { get; set; }
        public TextToGlyphs? TextToGlyphs       { get; set; }
        public UnicodeToGlyph? UnicodeToGlyph   { get; set; }
    }
}
