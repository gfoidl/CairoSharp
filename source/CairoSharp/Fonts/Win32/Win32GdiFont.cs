// (c) gfoidl, all rights reserved

using Cairo.Fonts.Scaled;
using static Cairo.Fonts.Win32.Win32GdiFontNative;

namespace Cairo.Fonts.Win32;

/// <summary>
/// Win32 GDI Fonts — Font support for Microsoft Windows
/// </summary>
/// <remarks>
/// The Microsoft Windows font backend is primarily used to render text on Microsoft Windows systems.
/// <para>
/// Note: Win32 GDI fonts do not support color fonts. Use DWrite fonts if color font support is required.
/// </para>
/// </remarks>
public sealed unsafe class Win32GdiFont : FontFace
{
    private Win32GdiFont(cairo_font_face_t* handle) : base(handle) { }

    /// <summary>
    /// Creates a new font for the Win32 font backend based on a LOGFONT.
    /// </summary>
    /// <param name="logfont">
    /// A LOGFONTW structure specifying the font to use. The lfHeight, lfWidth, lfOrientation
    /// and lfEscapement fields of this structure are ignored.
    /// </param>
    /// <remarks>
    /// This font can then be used with <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/> or
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// The <see cref="ScaledFont"/> returned from <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>
    /// is also for the Win32 backend and can be used with functions such as <see cref="SelectFont"/>.
    /// </remarks>
    public static Win32GdiFont CreateForLogFont(IntPtr logfont)
    {
        cairo_font_face_t* fontFace = cairo_win32_font_face_create_for_logfontw(logfont.ToPointer());
        return new Win32GdiFont(fontFace);
    }

    /// <summary>
    /// Creates a new font for the Win32 font backend based on a HFONT.
    /// </summary>
    /// <param name="hFont">An HFONT structure specifying the font to use.</param>
    /// <remarks>
    /// This font can then be used with <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/> or
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// The <see cref="ScaledFont"/> returned from <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>
    /// is also for the Win32 backend and can be used with functions such as <see cref="SelectFont"/>.
    /// </remarks>
    public static Win32GdiFont CreateForHFont(IntPtr hFont)
    {
        cairo_font_face_t* fontFace = cairo_win32_font_face_create_for_hfont(hFont.ToPointer());
        return new Win32GdiFont(fontFace);
    }

    /// <summary>
    /// Creates a new font for the Win32 font backend based on a LOGFONT.
    /// </summary>
    /// <param name="logfont">
    /// A LOGFONTW structure specifying the font to use. If font is NULL then the lfHeight,
    /// lfWidth, lfOrientation and lfEscapement fields of this structure are ignored. Otherwise
    /// lfWidth, lfOrientation and lfEscapement must be zero.
    /// </param>
    /// <param name="hFont">
    /// An HFONT that can be used when the font matrix is a scale by -lfHeight and the CTM is identity.
    /// </param>
    /// <remarks>
    /// This font can then be used with <see cref="TextExtensions.set_FontFace(CairoContext, FontFace)"/> or
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// The <see cref="ScaledFont"/> returned from <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>
    /// is also for the Win32 backend and can be used with functions such as <see cref="SelectFont"/>.
    /// </remarks>
    public static Win32GdiFont CreateForLogFontHFont(IntPtr logfont, IntPtr hFont)
    {
        cairo_font_face_t* fontFace = cairo_win32_font_face_create_for_logfontw_hfont(logfont.ToPointer(), hFont.ToPointer());
        return new Win32GdiFont(fontFace);
    }

    /// <summary>
    /// Selects the font into the given device context and changes the map mode and world
    /// transformation of the device context to match that of the font.
    /// </summary>
    /// <param name="hdc">a device context</param>
    /// <remarks>
    /// This method is intended for use when using layout APIs such as Uniscribe to do text layout
    /// with the cairo font. After finishing using the device context, you must call
    /// <see cref="DoneFont"/> to release any resources allocated by this method.
    /// <para>
    /// See <see cref="GetMetricsFactor"/> for converting logical coordinates from the device
    /// context to font space.
    /// </para>
    /// <para>
    /// Normally, calls to SaveDC() and RestoreDC() would be made around the use of this
    /// method to preserve the original graphics state.
    /// </para>
    /// </remarks>
    public void SelectFont(IntPtr hdc)
    {
        this.CheckDisposed();

        Status status = cairo_win32_scaled_font_select_font((cairo_scaled_font_t*)this.Handle, hdc.ToPointer());
        status.ThrowIfNotSuccess();
    }

    /// <summary>
    /// Releases any resources allocated by <see cref="SelectFont(nint)"/>
    /// </summary>
    public void DoneFont()
    {
        this.CheckDisposed();
        cairo_win32_scaled_font_done_font((cairo_scaled_font_t*)this.Handle);
    }

    /// <summary>
    /// Gets a scale factor between logical coordinates in the coordinate space used by <see cref="SelectFont(nint)"/>
    /// (that is, the coordinate system used by the Windows functions to return metrics) and font space coordinates.
    /// </summary>
    /// <returns>factor to multiply logical units by to get font space coordinates.</returns>
    public double GetMetricsFactor()
    {
        this.CheckDisposed();
        return cairo_win32_scaled_font_get_metrics_factor((cairo_scaled_font_t*)this.Handle);
    }

    /// <summary>
    /// Gets the transformation mapping the logical space used by <see cref="ScaledFont"/> to device space.
    /// </summary>
    /// <param name="logicalToDevice">matrix to return</param>
    public void GetLogicalToDevice(out Matrix logicalToDevice)
    {
        this.CheckDisposed();
        cairo_win32_scaled_font_get_logical_to_device((cairo_scaled_font_t*)this.Handle, out logicalToDevice);
    }

    /// <summary>
    /// Gets the transformation mapping device space to the logical space used by <see cref="ScaledFont"/>.
    /// </summary>
    /// <param name="deviceToLogical">matrix to return</param>
    public void GetDeviceToLogical(out Matrix deviceToLogical)
    {
        this.CheckDisposed();
        cairo_win32_scaled_font_get_device_to_logical((cairo_scaled_font_t*)this.Handle, out deviceToLogical);
    }
}
