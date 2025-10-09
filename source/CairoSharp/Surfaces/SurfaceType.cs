// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces;

/// <summary>
/// <see cref="SurfaceType"/> is used to describe the type of a given surface. The surface
/// types are also known as "backends" or "surface backends" within cairo.
/// </summary>
/// <remarks>
/// The type of a surface is determined by the function used to create it, which will
/// generally be of the form cairo_type_surface_create(),
/// (though see cairo_surface_create_similar() as well).
/// <para>
/// The surface type can be queried with cairo_surface_get_type()
/// </para>
/// <para>
/// The various cairo_surface_t functions can be used with surfaces of any type,
/// but some backends also provide type-specific functions that must only be called
/// with a surface of the appropriate type. These functions have names that begin with
/// cairo_type_surface such as cairo_image_surface_get_width().
/// </para>
/// <para>
/// The behavior of calling a type-specific function with a surface of the wrong type is undefined.
/// </para>
/// <para>
/// New entries may be added in future versions.
/// </para>
/// </remarks>
public enum SurfaceType
{
    /// <summary>
    /// The surface is of type image, since 1.2
    /// </summary>
    Image,

    /// <summary>
    /// The surface is of type pdf, since 1.2
    /// </summary>
    Pdf,

    /// <summary>
    /// The surface is of type ps, since 1.2
    /// </summary>
    PS,

    /// <summary>
    /// The surface is of type xlib, since 1.2
    /// </summary>
    Xlib,

    /// <summary>
    /// The surface is of type xcb, since 1.2
    /// </summary>
    Xcb,

    /// <summary>
    /// The surface is of type glitz, since 1.2, deprecated 1.18 (glitz support
    /// have been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("Glitz support have been removed")]
    Glitz,

    /// <summary>
    /// The surface is of type quartz, since 1.2
    /// </summary>
    Quartz,

    /// <summary>
    /// The surface is of type win32, since 1.2
    /// </summary>
    Win32,

    /// <summary>
    /// The surface is of type beos, since 1.2, deprecated 1.18 (beos support have
    /// been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("BeOS support have been removed")]
    BeOS,

    /// <summary>
    /// The surface is of type directfb, since 1.2, deprecated 1.18 (directfb
    /// support have been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("DirectFB support have been removed")]
    DirectFB,

    /// <summary>
    /// The surface is of type svg, since 1.2
    /// </summary>
    Svg,

    /// <summary>
    /// The surface is of type os2, since 1.4, deprecated 1.18 (os2 support
    /// have been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("OS2 support have been removed")]
    OS2,

    /// <summary>
    /// The surface is a win32 printing surface, since 1.6
    /// </summary>
    Win32Printing,

    /// <summary>
    /// The surface is of type quartz_image, since 1.6
    /// </summary>
    QuartzImage,

    /// <summary>
    /// The surface is of type script, since 1.10
    /// </summary>
    Script,

    /// <summary>
    /// The surface is of type Qt, since 1.10, deprecated 1.18 (Ot support have
    /// been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("Qt support have been removed")]
    Qt,

    /// <summary>
    /// The surface is of type recording, since 1.10
    /// </summary>
    Recording,

    /// <summary>
    /// The surface is a OpenVG surface, since 1.10, deprecated 1.18 (OpenVG
    /// support have been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("OpenVG support have been removed")]
    OpenVG,

    /// <summary>
    /// The surface is of type OpenGL, since 1.10, deprecated 1.18 (OpenGL
    /// support have been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("OpenGL support have been removed")]
    OpenGL,

    /// <summary>
    /// The surface is of type Direct Render Manager, since 1.10, deprecated 1.18 (DRM support
    /// have been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("DRM support have been removed")]
    Drm,

    /// <summary>
    /// The surface is of type 'tee' (a multiplexing surface), since 1.10
    /// </summary>
    Tee,

    /// <summary>
    /// The surface is of type XML (for debugging), since 1.10
    /// </summary>
    Xml,

    /// <summary>
    /// The surface is of type Skia, since 1.10, deprecated 1.18 (Skia support
    /// have been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("Skia support have been removed")]
    Skia,

    /// <summary>
    /// The surface is a subsurface created with cairo_surface_create_for_rectangle(), since 1.10
    /// </summary>
    SubSurface,

    /// <summary>
    /// This surface is of type Cogl, since 1.12, deprecated 1.18 (Cogl support have
    /// been removed, this surface type will never be set by cairo)
    /// </summary>
    [Obsolete("Cogl support have been removed")]
    Cogl
}
