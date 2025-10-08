// (c) gfoidl, all rights reserved

using static Cairo.Fonts.DirectWrite.DirectWriteFontNative;

namespace Cairo.Fonts.DirectWrite;

/// <summary>
/// DWrite Fonts — Font support for Microsoft DirectWrite
/// </summary>
/// <remarks>
/// The Microsoft DirectWrite font backend is primarily used to render text on Microsoft Windows systems.
/// </remarks>
public sealed unsafe class DirectWriteFont : FontFace
{
    /// <summary>
    /// Creates a new font for the DWrite font backend based on a DWrite font face.
    /// </summary>
    /// <param name="dwriteFontFace">A pointer to an IDWriteFontFace specifying the DWrite font to use.</param>
    /// <remarks>
    /// This font can then be used with <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/> or
    /// <see cref="Scaled.ScaledFont.ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// <para>
    /// See <a href="https://www.cairographics.org/manual/cairo-DWrite-Fonts.html#cairo-dwrite-font-face-create-for-dwrite-fontface">cairo docs</a>
    /// for an example about usage.
    /// </para>
    /// <para>
    /// Note: When printing a DWrite font to a <see cref="Surfaces.SurfaceType.Win32Printing"/> surface, the
    /// printing surface will substitute each DWrite font with a Win32 font created from the same
    /// underlying font file. If the matching font file can not be found, the <see cref="Surfaces.SurfaceType.Win32Printing"/>
    /// surface will convert each glyph to a filled path. If a DWrite font was not created from a system font,
    /// it is recommended that the font used to create the DWrite font be made available to GDI to avoid
    /// the undesirable fallback to emitting paths. This can be achieved using the GDI font loading functions
    /// such as AddFontMemResourceEx().
    /// </para>
    /// </remarks>
    public DirectWriteFont(IntPtr dwriteFontFace)
        : base(CreateCore(dwriteFontFace), owner: true) { }

    private static void* CreateCore(IntPtr dwriteFontFace)
    {
        CairoAPI.CheckSupportedVersion(1, 18, 0);

        return cairo_dwrite_font_face_create_for_dwrite_fontface(dwriteFontFace.ToPointer());
    }

    /// <summary>
    /// Gets or sets the IDWriteRenderingParams object to <see cref="FontFace"/>.
    /// </summary>
    /// <remarks>
    /// This IDWriteRenderingParams is used to render glyphs if default values of font options
    /// are used. If non-defalut values of font options are specified when creating a
    /// <see cref="Scaled.ScaledFont"/>, cairo creates a new IDWriteRenderingParams object for the
    /// <see cref="Scaled.ScaledFont"/> object by overwriting the corresponding parameters.
    /// </remarks>
    public IntPtr RenderingParams
    {
        get
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            return new IntPtr(cairo_dwrite_font_face_get_rendering_params(this.Handle));
        }
        set
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            cairo_dwrite_font_face_set_rendering_params(this.Handle, value.ToPointer());
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="MeasuringMode"/> enum to font_face.
    /// </summary>
    public MeasuringMode MeasuringMode
    {
        get
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            return cairo_dwrite_font_face_get_measuring_mode(this.Handle);
        }
        set
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            cairo_dwrite_font_face_set_measuring_mode(this.Handle, value);
        }
    }
}
