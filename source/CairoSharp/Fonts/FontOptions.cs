// (c) gfoidl, all rights reserved

using System.ComponentModel;
using static Cairo.Fonts.FontOptionsNative;

namespace Cairo.Fonts;

[EditorBrowsable(EditorBrowsableState.Never)]
public struct cairo_font_options_t;

/// <summary>
/// cairo_font_options_t — How a font should be rendered
/// </summary>
/// <remarks>
/// The font options specify how fonts should be rendered. Most of the time the font options
/// implied by a surface are just right and do not need any changes, but for pixel-based targets
/// tweaking font options may result in superior output on a particular display.
/// </remarks>
public sealed unsafe class FontOptions : CairoObject<cairo_font_options_t>, IEquatable<FontOptions>
{
    internal FontOptions(cairo_font_options_t* fontOptions, bool isOwnedByCairo = false)
        : base(fontOptions, isOwnedByCairo)
        => _customColorPaletteEntry = new CustomColorPaletteEntry(this);

    /// <summary>
    /// Allocates a new font options object with all options initialized to default values.
    /// </summary>
    /// <remarks>
    /// Free with <see cref="CairoObject.Dispose()"/>. This constructor always returns a valid pointer;
    /// if memory cannot be allocated, then a special error object is returned where all operations
    /// on the object do nothing. You can check for this with <see cref="Status"/>.
    /// </remarks>
    public FontOptions() : this(cairo_font_options_create()) { }

    protected override void DisposeCore(cairo_font_options_t* handle) => cairo_font_options_destroy(handle);

    /// <summary>
    /// Allocates a new font options object copying the option values from original.
    /// </summary>
    /// <returns>
    /// a newly allocated <see cref="FontOptions"/>. Free with <see cref="CairoObject.Dispose()"/>.
    /// </returns>
    /// <exception cref="CairoException">memory cannot be allocated</exception>
    public FontOptions Copy()
    {
        this.CheckDisposed();

        FontOptions result = new(cairo_font_options_copy(this.Handle), isOwnedByCairo: false);
        result.Status.ThrowIfStatus(Status.NoMemory);

        return result;
    }

    /// <summary>
    /// Checks whether an error has previously occurred for this font options object
    /// </summary>
    /// <remarks>
    /// <see cref="Status.Success"/>, <see cref="Status.NoMemory"/>, <see cref="Status.NullPointer"/>
    /// </remarks>
    public Status Status
    {
        get
        {
            this.CheckDisposed();
            return cairo_font_options_status(this.Handle);
        }
    }

    /// <summary>
    /// Merges non-default options from other into options, replacing existing values. This
    /// operation can be thought of as somewhat similar to compositing other onto options with
    /// the operation of CAIRO_OPERATOR_OVER.
    /// </summary>
    /// <param name="other">another <see cref="FontOptions"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c></exception>
    public void Merge(FontOptions other)
    {
        ArgumentNullException.ThrowIfNull(other);

        this.CheckDisposed();
        cairo_font_options_merge(this.Handle, other.Handle);
    }

    public override int GetHashCode()
    {
        this.CheckDisposed();

        // According the cairo docs this case is safe.
        return (int)cairo_font_options_hash(this.Handle).Value;
    }

    public override bool Equals(object? obj) => this.Equals(obj as FontOptions);

    /// <summary>
    /// Compares two font options objects for equality.
    /// </summary>
    /// <param name="other">another <see cref="FontOptions"/></param>
    /// <returns>
    /// <c>true</c> if all fields of the two font options objects match. Note that this
    /// method will return <c>false</c> if either object is in error.
    /// </returns>
    public bool Equals(FontOptions? other)
    {
        if (other is null)
        {
            return false;
        }

        this.CheckDisposed();

        return cairo_font_options_equal(this.Handle, other.Handle);
    }

    public static bool operator ==(FontOptions a, FontOptions b)
    {
        if (a is null)
        {
            return b is null;
        }

        return a.Equals(b);
    }

    public static bool operator !=(FontOptions a, FontOptions b) => !(a == b);

    /// <summary>
    /// Gets or sets the antialiasing mode for the font options object. This specifies the
    /// type of antialiasing to do when rendering text.
    /// </summary>
    public Antialias Antialias
    {
        get
        {
            this.CheckDisposed();
            return cairo_font_options_get_antialias(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_font_options_set_antialias(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the subpixel order for the font options object. The subpixel order specifies
    /// the order of color elements within each pixel on the display device when rendering with
    /// an antialiasing mode of <see cref="Antialias.Subpixel"/>. See the documentation for
    /// <see cref="SubpixelOrder"/> for full details.
    /// </summary>
    public SubpixelOrder SubpixelOrder
    {
        get
        {
            this.CheckDisposed();
            return cairo_font_options_get_subpixel_order(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_font_options_set_subpixel_order(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the hint style for font outlines for the font options object. This controls
    /// whether to fit font outlines to the pixel grid, and if so, whether to optimize for
    /// fidelity or contrast. See the documentation for <see cref="HintStyle"/> for full details.
    /// </summary>
    public HintStyle HintStyle
    {
        get
        {
            this.CheckDisposed();
            return cairo_font_options_get_hint_style(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_font_options_set_hint_style(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the metrics hinting mode for the font options object. This controls whether
    /// metrics are quantized to integer values in device units. See the documentation
    /// for <see cref="HintMetrics"/> for full details.
    /// </summary>
    public HintMetrics HintMetrics
    {
        get
        {
            this.CheckDisposed();
            return cairo_font_options_get_hint_metrics(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_font_options_set_hint_metrics(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the OpenType font variations for the font options object.
    /// </summary>
    /// <remarks>
    /// Font variations are specified as a string with a format that is similar to the
    /// CSS font-variation-settings. The string contains a comma-separated list of axis
    /// assignments, which each assignment consists of a 4-character axis name and a value,
    /// separated by whitespace and optional equals sign.
    /// <para>
    /// Examples:<br />
    /// wght=200,wdth=140.5<br />
    /// wght 200 , wdth 140.5
    /// </para>
    /// <para>
    /// The returned string belongs to the options and must not be modified. It is valid until
    /// either the font options object is destroyed or the font variations in this
    /// object is modified with the setter.
    /// </para>
    /// </remarks>
    public string? Variations
    {
        get
        {
            this.CheckDisposed();

            return cairo_font_options_get_variations(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_font_options_set_variations(this.Handle, value);
        }
    }

    /// <summary>
    /// Gets or sets the color mode for the font options object. This controls whether color
    /// fonts are to be rendered in color or as outlines. See the documentation for
    /// <see cref="ColorMode"/> for full details.
    /// </summary>
    /// <remarks>
    /// Since: 1.18
    /// </remarks>
    public ColorMode ColorMode
    {
        get
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            return cairo_font_options_get_color_mode(this.Handle);
        }
        set
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            cairo_font_options_set_color_mode(this.Handle, value);
        }
    }

    public const int ColorPaletteDefault = 0;

    /// <summary>
    /// Gets or sets the OpenType font color palette for the font options object. OpenType
    /// color fonts with a CPAL table may contain multiple palettes. The default
    /// color palette index is <see cref="ColorPaletteDefault"/>.
    /// </summary>
    /// <remarks>
    /// If palette_index is invalid, the default palette is used.
    /// <para>
    /// Individual colors within the palette may be overriden with cairo_font_options_set_custom_palette_color().
    /// </para>
    /// </remarks>
    public int ColorPalette
    {
        get
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            return (int)cairo_font_options_get_color_palette(this.Handle);
        }
        set
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            cairo_font_options_set_color_palette(this.Handle, (uint)value);
        }
    }

    private readonly CustomColorPaletteEntry _customColorPaletteEntry;

    /// <summary>
    /// Gets or sets a custom palette color for the font options object. This overrides the
    /// palette color at the specified color index. This override is independent of the
    /// selected palette index and will remain in place even if <see cref="ColorPalette"/> is
    /// called to change the palette index.
    /// </summary>
    /// <remarks>
    /// It is only possible to override color indexes already in the font palette.
    /// <para>
    /// Available from Cairo 1.18 onwards.
    /// </para>
    /// </remarks>
    public CustomColorPaletteEntry CustomColorPalette
    {
        get
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);

            this.CheckDisposed();
            return _customColorPaletteEntry;
        }
    }

    public readonly struct CustomColorPaletteEntry
    {
        private readonly FontOptions _fontOptions;

        internal CustomColorPaletteEntry(FontOptions fontOptions) => _fontOptions = fontOptions;

        public Color? this[int index]
        {
            get
            {
                Status status = cairo_font_options_get_custom_palette_color(
                    _fontOptions.Handle,
                    (uint)index,
                    out double red,
                    out double green,
                    out double blue,
                    out double alpha);

                if (status == Status.Success)
                {
                    return new Color(red, green, blue, alpha);
                }

                return null;
            }
            set
            {
                if (!value.HasValue)
                {
                    return;
                }

                Color color = value.Value;

                cairo_font_options_set_custom_palette_color(
                    _fontOptions.Handle,
                    (uint)index,
                    color.Red,
                    color.Green,
                    color.Blue,
                    color.Alpha);
            }
        }
    }
}
