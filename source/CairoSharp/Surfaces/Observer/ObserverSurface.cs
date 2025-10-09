// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static Cairo.Surfaces.Observer.ObserverSurfaceNative;

namespace Cairo.Surfaces.Observer;

/// <summary>
/// Event arguments ofr the <see cref="ObserverSurface"/>.
/// </summary>
/// <param name="target">The <see cref="Surface"/></param>
public class ObserverEventArgs(Surface target) : EventArgs
{
    public Surface Target { get; } = target;
}

/// <summary>
/// Surface Observer — Observing other surfaces
/// </summary>
/// <remarks>
/// A surface that exists solely to watch what another surface is doing.
/// </remarks>
public sealed unsafe class ObserverSurface : Surface
{
    private readonly Surface _target;
    private readonly void* _thisHandle;

    protected override void DisposeCore(void* handle)
    {
        GCHandle thisHandle = GCHandle.FromIntPtr(new IntPtr(_thisHandle));

        if (thisHandle.IsAllocated)
        {
            thisHandle.Free();
        }

        base.DisposeCore(handle);
    }

    /// <summary>
    /// Create a new surface that exists solely to watch another is doing. In the process it will
    /// log operations and times, which are fast, which are slow, which are frequent, etc.
    /// </summary>
    /// <param name="target">an existing surface for which the observer will watch</param>
    /// <param name="mode">sets the mode of operation (normal vs. record)</param>
    /// <param name="throwOnConstructionError">
    /// when <c>true</c> (the default) an exception is thrown when the surface could not be created.
    /// </param>
    /// <remarks>
    /// The mode parameter can be set to either <see cref="ObserverMode.Normal"/> or
    /// <see cref="ObserverMode.RecordOperations"/>, to control whether or not the internal observer
    /// should record operations.
    /// <para>
    /// The caller owns the surface and should call <see cref="CairoObject.Dispose()"/> when done with it.
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">
    /// when construction fails and <paramref name="throwOnConstructionError"/> is set to <c>true</c>
    /// </exception>
    public ObserverSurface(Surface target, ObserverMode mode, bool throwOnConstructionError = true)
        : base(cairo_surface_create_observer(target.Handle, mode), owner: true, throwOnConstructionError)
    {
        _target     = target ?? throw new ArgumentNullException(nameof(target));
        _thisHandle = GCHandle.ToIntPtr(GCHandle.Alloc(this, GCHandleType.Normal)).ToPointer();

        this.AddFillCallback();
        this.AddFinishCallback();
        this.AddFlushCallback();
        this.AddGlyphsCallback();
        this.AddMaskCallback();
        this.AddPaintCallback();
        this.AddStrokeCallback();
    }

    /// <summary>
    /// An event for fill operations on the observed surface.
    /// </summary>
    public event EventHandler<ObserverEventArgs>? FillOperation;

    private void AddFillCallback()
    {
        cairo_surface_observer_callback_t callback = &Callback;
        Status status                              = cairo_surface_observer_add_fill_callback(this.Handle, callback, _thisHandle);

        status.ThrowIfNotSuccess();

        static void Callback(void* observer, void* target, void* state)
        {
            Debug.Assert(observer == state);

            GCHandle thisHandle   = GCHandle.FromIntPtr(new IntPtr(state));
            ObserverSurface @this = (ObserverSurface)thisHandle.Target!;

            @this.OnEvent(@this.FillOperation);
        }
    }

    /// <summary>
    /// An event for finish operations on the observed surface.
    /// </summary>
    public event EventHandler<ObserverEventArgs>? FinishOperation;

    private void AddFinishCallback()
    {
        cairo_surface_observer_callback_t callback = &Callback;
        Status status                              = cairo_surface_observer_add_finish_callback(this.Handle, callback, _thisHandle);

        status.ThrowIfNotSuccess();

        static void Callback(void* observer, void* target, void* state)
        {
            Debug.Assert(observer == state);

            GCHandle thisHandle   = GCHandle.FromIntPtr(new IntPtr(state));
            ObserverSurface @this = (ObserverSurface)thisHandle.Target!;

            @this.OnEvent(@this.FinishOperation);
        }
    }

    /// <summary>
    /// An event for flush operations on the observed surface.
    /// </summary>
    public event EventHandler<ObserverEventArgs>? FlushOperation;

    private void AddFlushCallback()
    {
        cairo_surface_observer_callback_t callback = &Callback;
        Status status                              = cairo_surface_observer_add_flush_callback(this.Handle, callback, _thisHandle);

        status.ThrowIfNotSuccess();

        static void Callback(void* observer, void* target, void* state)
        {
            Debug.Assert(observer == state);

            GCHandle thisHandle   = GCHandle.FromIntPtr(new IntPtr(state));
            ObserverSurface @this = (ObserverSurface)thisHandle.Target!;

            @this.OnEvent(@this.FlushOperation);
        }
    }

    /// <summary>
    /// An event for glyph operations on the observed surface.
    /// </summary>
    public event EventHandler<ObserverEventArgs>? GlyphsOperation;

    private void AddGlyphsCallback()
    {
        cairo_surface_observer_callback_t callback = &Callback;
        Status status                              = cairo_surface_observer_add_glyphs_callback(this.Handle, callback, _thisHandle);

        status.ThrowIfNotSuccess();

        static void Callback(void* observer, void* target, void* state)
        {
            Debug.Assert(observer == state);

            GCHandle thisHandle   = GCHandle.FromIntPtr(new IntPtr(state));
            ObserverSurface @this = (ObserverSurface)thisHandle.Target!;

            @this.OnEvent(@this.GlyphsOperation);
        }
    }

    /// <summary>
    /// An event for mask operations on the observed surface.
    /// </summary>
    public event EventHandler<ObserverEventArgs>? MaskOperation;

    private void AddMaskCallback()
    {
        cairo_surface_observer_callback_t callback = &Callback;
        Status status                              = cairo_surface_observer_add_mask_callback(this.Handle, callback, _thisHandle);

        status.ThrowIfNotSuccess();

        static void Callback(void* observer, void* target, void* state)
        {
            Debug.Assert(observer == state);

            GCHandle thisHandle   = GCHandle.FromIntPtr(new IntPtr(state));
            ObserverSurface @this = (ObserverSurface)thisHandle.Target!;

            @this.OnEvent(@this.MaskOperation);
        }
    }

    /// <summary>
    /// An event for paint operations on the observed surface.
    /// </summary>
    public event EventHandler<ObserverEventArgs>? PaintOperation;

    private void AddPaintCallback()
    {
        cairo_surface_observer_callback_t callback = &Callback;
        Status status                              = cairo_surface_observer_add_paint_callback(this.Handle, callback, _thisHandle);

        status.ThrowIfNotSuccess();

        static void Callback(void* observer, void* target, void* state)
        {
            Debug.Assert(observer == state);

            GCHandle thisHandle   = GCHandle.FromIntPtr(new IntPtr(state));
            ObserverSurface @this = (ObserverSurface)thisHandle.Target!;

            @this.OnEvent(@this.PaintOperation);
        }
    }

    /// <summary>
    /// An event for stroke operations on the observed surface.
    /// </summary>
    public event EventHandler<ObserverEventArgs>? StrokeOperation;

    private void AddStrokeCallback()
    {
        cairo_surface_observer_callback_t callback = &Callback;
        Status status                              = cairo_surface_observer_add_stroke_callback(this.Handle, callback, _thisHandle);

        status.ThrowIfNotSuccess();

        static void Callback(void* observer, void* target, void* state)
        {
            Debug.Assert(observer == state);

            GCHandle thisHandle   = GCHandle.FromIntPtr(new IntPtr(state));
            ObserverSurface @this = (ObserverSurface)thisHandle.Target!;

            @this.OnEvent(@this.StrokeOperation);
        }
    }

    private void OnEvent(EventHandler<ObserverEventArgs>? handler)
    {
        this.CheckDisposed();

        if (handler is not null)
        {
            ObserverEventArgs ea = new(_target);
            handler(this, ea);
        }
    }

    /// <summary>
    /// Returns the total observation time in nanoseconds
    /// </summary>
    public double Elapsed
    {
        get
        {
            this.CheckDisposed();
            return cairo_surface_observer_elapsed(this.Handle);
        }
    }

    /// <summary>
    /// Creates a string representation of the observer log.
    /// </summary>
    public string GetObserverLog()
    {
        this.CheckDisposed();

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        StringBuilder sb             = new();
        cairo_write_func_t writeFunc = &WriteFunc;

        Status status = cairo_surface_observer_print(this.Handle, writeFunc, &sb);

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
}
