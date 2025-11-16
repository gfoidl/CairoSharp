// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Text;
using static Cairo.Surfaces.DeviceNative;

namespace Cairo.Surfaces;

/// <summary>
/// <see cref="Device"/> — interface to underlying rendering system
/// </summary>
/// <remarks>
/// Devices are the abstraction Cairo employs for the rendering system used by a <see cref="Surface"/>.
/// You can get the device of a surface using <see cref="Surface.Device"/>.
/// <para>
/// Devices are created using custom functions specific to the rendering system you want to use.
/// See the documentation for the surface types for those functions.
/// </para>
/// <para>
/// An important function that devices fulfill is sharing access to the rendering system between
/// Cairo and your application. If you want to access a device directly that you used to draw to with
/// Cairo, you must first call <see cref="Flush"/> to ensure that Cairo finishes all operations on
/// the device and resets it to a clean state.
/// </para>
/// <para>
/// Cairo also provides the functions <see cref="Acquire"/> and <see cref="Release"/> to synchronize access
/// to the rendering system in a multithreaded environment. This is done internally,
/// but can also be used by applications.
/// </para>
/// </remarks>
public unsafe class Device : CairoObject<cairo_device_t>
{
    internal Device(cairo_device_t* device, bool needsReference = true) : base(CreateCore(device, needsReference)) { }

    [StackTraceHidden]
    private static cairo_device_t* CreateCore(cairo_device_t* device, bool needsReference = true)
    {
        if (needsReference)
        {
            cairo_device_reference(device);
        }

        return device;
    }

    protected override void DisposeCore(cairo_device_t* device)
    {
        cairo_device_destroy(device);

        PrintDebugInfo(device);
        [Conditional("DEBUG")]
        static void PrintDebugInfo(cairo_device_t* device)
        {
            uint rc = cairo_device_get_reference_count(device);
            Debug.WriteLine($"Device 0x{(nint)device}: reference count = {rc}");
        }
    }

    /// <summary>
    /// Checks whether an error has previously occurred for this device.
    /// </summary>
    /// <remarks>
    /// <see cref="Status.Success"/> on success or an error code if the device is in an error state.
    /// </remarks>
    public Status Status
    {
        get
        {
            this.CheckDisposed();
            return cairo_device_status(this.Handle);
        }
    }

    /// <summary>
    /// This method finishes the device and drops all references to external resources. All surfaces,
    /// fonts and other objects created for this device will be finished, too. Further operations
    /// on the device will not affect the device but will instead trigger a <see cref="Status.DeviceFinished"/> error.
    /// </summary>
    /// <remarks>
    /// When the last call to cairo_device_destroy() decreases the reference count to zero, cairo will
    /// call cairo_device_finish() if it hasn't been called already, before freeing the resources associated
    /// with the device.
    /// <para>
    /// This method may acquire devices.
    /// </para>
    /// </remarks>
    public void Finish()
    {
        this.CheckDisposed();
        cairo_device_finish(this.Handle);
    }

    /// <summary>
    /// Finish any pending operations for the device and also restore any temporary modifications
    /// cairo has made to the device's state. This method must be called before switching from using
    /// the device with Cairo to operating on it directly with native APIs. If the device doesn't support
    /// direct access, then this method does nothing.
    /// </summary>
    public void Flush()
    {
        this.CheckDisposed();
        cairo_device_flush(this.Handle);
    }

    /// <summary>
    /// This property returns the type of the device. See <see cref="DeviceType"/> for available types.
    /// </summary>
    public DeviceType Type
    {
        get
        {
            this.CheckDisposed();
            return cairo_device_get_type(this.Handle);
        }
    }

    /// <summary>
    /// Returns the current reference count of <see cref="Device"/>.
    /// </summary>
    /// <remarks>
    /// If the object is a nil object, 0 will be returned.
    /// </remarks>
    internal int ReferenceCount
    {
        get
        {
            this.CheckDisposed();
            return (int)cairo_device_get_reference_count(this.Handle);
        }
    }

    /// <summary>
    /// Acquires the <see cref="Device"/> for the current thread. This method will block
    /// until no other thread has acquired the device.
    /// </summary>
    /// <returns>
    /// <see cref="Status.Success"/> on success or an error code if the device is in an error state and could
    /// not be acquired. After a successful call to <see cref="Acquire"/>, a matching call to <see cref="Release"/>
    /// is required.
    /// </returns>
    /// <remarks>
    /// If the return value is <see cref="Status.Success"/>, you successfully acquired the device.
    /// From now on your thread owns the device and no other thread will be able to acquire it until a matching
    /// call to <see cref="Release"/>. It is allowed to recursively acquire the device multiple times from the same thread.
    /// <para>
    /// You must never acquire two different devices at the same time unless this is explicitly allowed.
    /// Otherwise the possibility of deadlocks exist. As various Cairo functions can acquire devices when
    /// called, these functions may also cause deadlocks when you call them with an acquired device.
    /// So you must not have a device acquired when calling them. These functions are marked in the documentation.
    /// </para>
    /// </remarks>
    public Status Acquire()
    {
        this.CheckDisposed();
        return cairo_device_acquire(this.Handle);
    }

    /// <summary>
    /// Releases a device previously acquired using <see cref="Acquire"/>. See that method for details.
    /// </summary>
    public void Release()
    {
        this.CheckDisposed();
        cairo_device_release(this.Handle);
    }

    /// <summary>
    /// Returns the total elapsed time of the observation in nanoseconds
    /// </summary>
    public double ObserverElapsed
    {
        get
        {
            this.CheckDisposed();
            return cairo_device_observer_elapsed(this.Handle);
        }
    }

    /// <summary>
    /// Returns the elapsed time of the fill operations in nanoseconds
    /// </summary>
    public double ObserverFillElapsed
    {
        get
        {
            this.CheckDisposed();
            return cairo_device_observer_fill_elapsed(this.Handle);
        }
    }

    /// <summary>
    /// Returns the elapsed time of the glyph operations in nanoseconds
    /// </summary>
    public double ObserverGlyphsElapsed
    {
        get
        {
            this.CheckDisposed();
            return cairo_device_observer_glyphs_elapsed(this.Handle);
        }
    }

    /// <summary>
    /// Returns the elapsed time of the mask operations in nanoseconds
    /// </summary>
    public double ObserverMaskElapsed
    {
        get
        {
            this.CheckDisposed();
            return cairo_device_observer_mask_elapsed(this.Handle);
        }
    }

    /// <summary>
    /// Returns the elapsed time of the paint operations in nanoseconds
    /// </summary>
    public double ObserverPaintElapsed
    {
        get
        {
            this.CheckDisposed();
            return cairo_device_observer_paint_elapsed(this.Handle);
        }
    }

    /// <summary>
    /// Creates a string representation of the device log.
    /// </summary>
    public string GetObserverLog()
    {
        this.CheckDisposed();

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        StringBuilder sb             = new();
        cairo_write_func_t writeFunc = &WriteFunc;

        Status status = cairo_device_observer_print(this.Handle, writeFunc, &sb);

        status.ThrowIfNotSuccess();

        return sb.ToString();

        static Status WriteFunc(void* state, byte* data, uint length)
        {
            string log = new((sbyte*)data, 0, (int)length);

            StringBuilder sb = *(StringBuilder*)state;
            sb.Append(log);

            return Status.Success;
        }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    }

    /// <summary>
    /// Returns the elapsed time of the stroke operations in nanoseconds
    /// </summary>
    public double ObserverStrokeElapsed
    {
        get
        {
            this.CheckDisposed();
            return cairo_device_observer_stroke_elapsed(this.Handle);
        }
    }
}
