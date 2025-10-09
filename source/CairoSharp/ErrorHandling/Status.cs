// (c) gfoidl, all rights reserved

namespace Cairo;

/// <summary>
/// <see cref="Status"/> is used to indicate errors that can occur when using Cairo.
/// In some cases it is returned directly by functions. but when using <see cref="CairoContext"/>,
/// the last error, if any, is stored in the context and can be retrieved with
/// <see cref="CairoContext.Status"/>.
/// </summary>
/// <remarks>
/// New entries may be added in future versions. Use <see cref="Error.GetString(Cairo.Status)"/>
/// to get a human-readable representation of an error message.
/// </remarks>
public enum Status
{
    /// <summary>
    /// no error has occurred (Since 1.0)
    /// </summary>
    Success = 0,

    /// <summary>
    /// out of memory (Since 1.0)
    /// </summary>
    NoMemory,

    /// <summary>
    /// <see cref="CairoContext.Restore"/> called without matching <see cref="CairoContext.Save"/> (Since 1.0)
    /// </summary>
    InvalidRestore,

    /// <summary>
    /// no saved group to pop, i.e. <see cref="CairoContext.PopGroup"/> without matching
    /// <see cref="CairoContext.PushGroup"/> (Since 1.0)
    /// </summary>
    InvalidPopGroup,

    /// <summary>
    /// no current point defined (Since 1.0)
    /// </summary>
    NoCurrentPoint,

    /// <summary>
    /// invalid matrix (not invertible) (Since 1.0)
    /// </summary>
    InvalidMatrix,

    /// <summary>
    /// invalid value for an input <see cref="Status"/> (Since 1.0)
    /// </summary>
    InvalidStatus,

    /// <summary>
    /// NULL pointer (Since 1.0)
    /// </summary>
    NullPointer,

    /// <summary>
    /// input string not valid UTF-8 (Since 1.0)
    /// </summary>
    InvalidString,

    /// <summary>
    /// input path data not valid (Since 1.0)
    /// </summary>
    InvalidPathData,

    /// <summary>
    /// error while reading from input stream (Since 1.0)
    /// </summary>
    ReadError,

    /// <summary>
    /// error while writing to output stream (Since 1.0)
    /// </summary>
    WriteError,

    /// <summary>
    /// target surface has been finished (Since 1.0)
    /// </summary>
    SurfaceFinished,

    /// <summary>
    /// the surface type is not appropriate for the operation (Since 1.0)
    /// </summary>
    SurfaceTypeMismatch,

    /// <summary>
    /// the pattern type is not appropriate for the operation (Since 1.0)
    /// </summary>
    PatternTypeMismatch,

    /// <summary>
    /// invalid value for an input <see cref="Surfaces.Content"/> (Since 1.0)
    /// </summary>
    InvalidContent,

    /// <summary>
    /// invalid value for an input <see cref="Surfaces.Format"/> (Since 1.0)
    /// </summary>
    InvalidFormat,

    /// <summary>
    /// invalid value for an input Visual* (Since 1.0)
    /// </summary>
    InvalidVisual,

    /// <summary>
    /// file not found (Since 1.0)
    /// </summary>
    FileNotFound,

    /// <summary>
    /// invalid value for a dash setting (Since 1.0)
    /// </summary>
    InvalidDash,

    /// <summary>
    /// invalid value for a DSC comment (Since 1.2)
    /// </summary>
    InvalidDscComment,

    /// <summary>
    /// invalid index passed to getter (Since 1.4)
    /// </summary>
    InvalidIndex,

    /// <summary>
    /// clip region not representable in desired format (Since 1.4)
    /// </summary>
    ClipNotRepresentable,

    /// <summary>
    /// error creating or writing to a temporary file (Since 1.6)
    /// </summary>
    TempFileError,

    /// <summary>
    /// invalid value for stride (Since 1.6)
    /// </summary>
    InvalidStride,

    /// <summary>
    /// the font type is not appropriate for the operation (Since 1.8)
    /// </summary>
    FontTypeMismatch,

    /// <summary>
    /// the user-font is immutable (Since 1.8)
    /// </summary>
    UserFontImmutable,

    /// <summary>
    /// error occurred in a user-font callback function (Since 1.8)
    /// </summary>
    UserFontError,

    /// <summary>
    /// negative number used where it is not allowed (Since 1.8)
    /// </summary>
    NegativeCount,

    /// <summary>
    /// input clusters do not represent the accompanying text and glyph array (Since 1.8)
    /// </summary>
    InvalidClusters,

    /// <summary>
    /// invalid value for an input <see cref="Drawing.Text.FontSlant"/> (Since 1.8)
    /// </summary>
    InvalidSlant,

    /// <summary>
    /// invalid value for an input <see cref="Drawing.Text.FontWeight"/> (Since 1.8)
    /// </summary>
    InvalidWeight,

    /// <summary>
    /// invalid value (typically too big) for the size of the input (surface, pattern, etc.) (Since 1.10)
    /// </summary>
    InvalidSize,

    /// <summary>
    /// user-font method not implemented (Since 1.10)
    /// </summary>
    UserFontNotImplemented,

    /// <summary>
    /// the device type is not appropriate for the operation (Since 1.10)
    /// </summary>
    DeviceTypeMismatch,

    /// <summary>
    /// an operation to the device caused an unspecified error (Since 1.10)
    /// </summary>
    DeviceError,

    /// <summary>
    /// a mesh pattern construction operation was used outside of a
    /// cairo_mesh_pattern_begin_patch() / cairo_mesh_pattern_end_patch()
    /// pair (Since 1.12)
    /// </summary>
    InvalidMeshConstruction,

    /// <summary>
    /// target device has been finished (Since 1.12)
    /// </summary>
    DeviceFinished,

    /// <summary>
    /// <see cref="Surfaces.MimeTypes.Jbig2GlobalId"/> has been used on at least one image but no image provided
    /// <see cref="Surfaces.MimeTypes.Jbig2Global"/> (Since 1.14)
    /// </summary>
    Jbig2GlobalMissing,

    /// <summary>
    /// error occurred in libpng while reading from or writing to a PNG file (Since 1.16)
    /// </summary>
    PngError,

    /// <summary>
    /// error occurred in libfreetype (Since 1.16)
    /// </summary>
    FreetypeError,

    /// <summary>
    /// error occurred in the Windows Graphics Device Interface (Since 1.16)
    /// </summary>
    Win32GdiError,

    /// <summary>
    /// invalid tag name, attributes, or nesting (Since 1.16)
    /// </summary>
    TagError,

    /// <summary>
    /// error occurred in the Windows Direct Write API (Since 1.18)
    /// </summary>
    DwriteError,

    /// <summary>
    /// error occurred in OpenType-SVG font rendering (Since 1.18)
    /// </summary>
    SvgFontError,

    /// <summary>
    /// this is a special value indicating the number of status values defined in this enumeration.
    /// When using this value, note that the version of cairo at run-time may have additional status
    /// values defined than the value of this symbol at compile-time. (Since 1.10)
    /// </summary>
    LastStatus
}
