// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces;

/// <summary>
/// <see cref="DeviceType"/> is used to describe the type of a given device. The
/// devices types are also known as "backends" within cairo.
/// </summary>
/// <remarks>
/// The device type can be queried with cairo_device_get_type()
/// <para>
/// The various cairo_device_t functions can be used with devices of any type, but some
/// backends also provide type-specific functions that must only be called with a device
/// of the appropriate type. These functions have names that begin with cairo_type_device such
/// as cairo_xcb_device_debug_cap_xrender_version().
/// </para>
/// <para>
/// The behavior of calling a type-specific function with a device of the wrong type is undefined.
/// </para>
/// <para>
/// New entries may be added in future versions.
/// </para>
/// </remarks>
public enum DeviceType
{
    /// <summary>
    /// The device is of type Direct Render Manager, since 1.10
    /// </summary>
    DirectRenderManager,

    /// <summary>
    /// The device is of type OpenGL, since 1.10
    /// </summary>
    OpenGL,

    /// <summary>
    /// The device is of type script, since 1.10
    /// </summary>
    Script,

    /// <summary>
    /// The device is of type xcb, since 1.10
    /// </summary>
    Xcb,

    /// <summary>
    /// The device is of type xlib, since 1.10
    /// </summary>
    Xlib,

    /// <summary>
    /// The device is of type XML, since 1.10
    /// </summary>
    Xml,

    /// <summary>
    /// The device is of type cogl, since 1.12
    /// </summary>
    Cogl,

    /// <summary>
    /// The device is of type win32, since 1.12
    /// </summary>
    Win32,

    /// <summary>
    /// The device is invalid, since 1.10
    /// </summary>
    Invalid
}
