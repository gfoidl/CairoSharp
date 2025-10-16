// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Cairo.Surfaces;
using static Cairo.Drawing.Patterns.RasterSourceNative;

// TODO: add other user friednly overloads

namespace Cairo.Drawing.Patterns;

/// <summary>
/// Raster Sources — Supplying arbitrary image data
/// </summary>
/// <remarks>
/// The raster source provides the ability to supply arbitrary pixel data whilst rendering.
/// The pixels are queried at the time of rasterisation by means of user callback functions,
/// allowing for the ultimate flexibility. For example, in handling compressed image sources,
/// you may keep a MRU cache of decompressed images and decompress sources on the fly and
/// discard old ones to conserve memory.
/// <para>
/// For the raster source to be effective, you must at least specify the acquire and release
/// callbacks which are used to retrieve the pixel data for the region of interest and demark
/// when it can be freed afterwards. Other callbacks are provided for when the pattern is copied
/// temporarily during rasterisation, or more permanently as a snapshot in order to keep the
/// pixel data available for printing.
/// </para>
/// </remarks>
public sealed unsafe class RasterSource : Pattern
{
    /// <summary>
    /// <see cref="Acquire"/> is the type of delegate which is called when a pattern is being rendered
    /// from. It should create a surface that provides the pixel data for the region of interest as
    /// defined by extents, though the surface itself does not have to be limited to that area.
    /// </summary>
    /// <param name="pattern">the pattern being rendered from</param>
    /// <param name="userData">the user data as given by construction</param>
    /// <param name="target">the rendering target surface</param>
    /// <param name="extents">rectangular region of interest in pixels in sample space</param>
    /// <returns>a <see cref="Surface"/></returns>
    /// <remarks>
    /// For convenience the surface should probably be of image type, created with
    /// <see cref="Surface.CreateSimilarImage(Format, int, int)"/> for the target (which
    /// enables the number of copies to be reduced during transfer to the device). Another option, might be to
    /// return a similar surface to the target for explicit handling by the application of a set of cached
    /// sources on the device. The region of sample data provided should be defined using
    /// <see cref="Surface.DeviceOffset"/> to specify the top-left corner of the sample data (along with
    /// width and height of the surface).
    /// </remarks>
    public delegate Surface Acquire(Pattern? pattern, object? userData, Surface target, ref RectangleInt extents);

    /// <summary>
    /// <see cref="Release"/> is the type of delegate which is called when the pixel data is no longer
    /// being access by the pattern for the rendering operation. Typically this delegate will simply
    /// destroy / dispose the surface created during acquire.
    /// </summary>
    /// <param name="surface">the surface created during <see cref="Acquire"/></param>
    public delegate void Release(Surface surface);

    /// <summary>
    /// <see cref="Snapshot"/> is the type of delegate which is called when the pixel data needs to be preserved
    /// for later use during printing. This pattern will be accessed again later, and it is expected to provide
    /// the pixel data that was current at the time of snapshotting.
    /// </summary>
    /// <param name="pattern">the pattern being rendered from</param>
    /// <returns>
    /// <see cref="Status.Success"/> on success, or one of the <see cref="Status"/> error codes for failure.
    /// </returns>
    public delegate Status Snapshot(Pattern? pattern);

    /// <summary>
    /// cairo_raster_source_copy_func_t is the type of delegate which is called when the pattern gets copied as
    /// a normal part of rendering.
    /// </summary>
    /// <param name="pattern">the <see cref="Pattern"/> that was copied to</param>
    /// <param name="other">the <see cref="Pattern"/> being used as the source for the copy</param>
    /// <returns>
    /// <see cref="Status.Success"/> on success, or one of the <see cref="Status"/> error codes for failure.
    /// </returns>
    public delegate Status Copy(Pattern pattern, Pattern other);

    /// <summary>
    /// <see cref="Finish"/> is the type of delegate which is called when the pattern (or a copy thereof)
    /// is no longer required.
    /// </summary>
    /// <param name="pattern">the pattern being rendered from</param>
    public delegate void Finish(Pattern? pattern);

    private readonly State? _state;
    private GCHandle        _stateHandle;

    internal RasterSource(cairo_pattern_t* pattern, bool isOwnedByCairo, bool needsDestroy = true)
        : base(pattern, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Creates a new user pattern for providing pixel data.
    /// </summary>
    /// <param name="userData">the user data to be passed to all callbacks</param>
    /// <param name="content">
    /// content type for the pixel data that will be returned. Knowing the content type ahead of time
    /// is used for analysing the operation and picking the appropriate rendering path.
    /// </param>
    /// <param name="width">maximum size of the sample area</param>
    /// <param name="height">maximum size of the sample area</param>
    /// <remarks>
    /// Note: with this overload <paramref name="userData"/> has to be provided, and the
    /// raw / unmanaged delegates need to be used. <see cref="RasterSource(object?, Content, int, int, Acquire)"/>
    /// allows the use of managed callbacks.
    /// <para>
    /// Use the setter functions to associate callbacks with the returned pattern.
    /// The only mandatory callback is acquire.
    /// </para>
    /// </remarks>
    public RasterSource(IntPtr userData, Content content, int width, int height)
        : base(cairo_pattern_create_raster_source(userData.ToPointer(), content, width, height)) { }

    /// <summary>
    /// Creates a new user pattern for providing pixel data.
    /// </summary>
    /// <param name="userData">the user data to be passed to all callbacks</param>
    /// <param name="content">
    /// content type for the pixel data that will be returned. Knowing the content type ahead of time
    /// is used for analysing the operation and picking the appropriate rendering path.
    /// </param>
    /// <param name="width">maximum size of the sample area</param>
    /// <param name="height">maximum size of the sample area</param>
    /// <param name="acquire">acquire delegate</param>
    /// <remarks>
    /// Use the setter functions to associate callbacks with the returned pattern.
    /// The only mandatory callback is acquire.
    /// </remarks>
    public RasterSource(object? userData, Content content, int width, int height, Acquire acquire)
        : this(IntPtr.Zero, content, width, height)
    {
        _state       = new State() { UserData = userData };
        _stateHandle = GCHandle.Alloc(_state, GCHandleType.Normal);

        this.CallbackData = GCHandle.ToIntPtr(_stateHandle);

        this.SetAcquire(acquire);
    }

    protected override void DisposeCore(cairo_pattern_t* pattern)
    {
        base.DisposeCore(pattern);

        if (_stateHandle.IsAllocated)
        {
            _stateHandle.Free();
        }
    }

    /// <summary>
    /// Gets or sets the user data that is provided to all callbacks.
    /// </summary>
    /// <remarks>
    /// Note: when the <see cref="CallbackData"/> is set manually, then the raw / unmanaged
    /// delegates have to be used.
    /// <para>
    /// With <see cref="ResetCallbackDataToDefault"/> the <see cref="CallbackData"/> can be
    /// reset.
    /// </para>
    /// </remarks>
    public IntPtr CallbackData
    {
        get
        {
            this.CheckDisposed();

            void* userData = cairo_raster_source_pattern_get_callback_data(this.Handle);
            return new IntPtr(userData);
        }
        set
        {
            this.CheckDisposed();

            if (value != GCHandle.ToIntPtr(_stateHandle) && _stateHandle.IsAllocated)
            {
                _stateHandle.Free();
            }

            cairo_raster_source_pattern_set_callback_data(this.Handle, value.ToPointer());
        }
    }

    /// <summary>
    /// Resets the <see cref="CallbackData"/> to the default value, so that the managed delegates
    /// can be used.
    /// </summary>
    public void ResetCallbackDataToDefault()
    {
        if (!_stateHandle.IsAllocated)
        {
            _stateHandle = GCHandle.Alloc(_state, GCHandleType.Normal);
        }

        this.CallbackData = GCHandle.ToIntPtr(_stateHandle);
    }

    /// <summary>
    /// Specifies the callbacks used to generate the image surface for a rendering operation
    /// (acquire) and the function used to cleanup that surface afterwards.
    /// </summary>
    /// <param name="acquire">
    /// acquire callback
    /// <para>
    /// See <a href="https://www.cairographics.org/manual/cairo-Raster-Sources.html#cairo-raster-source-acquire-func-t">cairo docs</a>
    /// for description.
    /// </para>
    /// </param>
    /// <param name="release">
    /// release callback
    /// <para>
    /// See <a href="https://www.cairographics.org/manual/cairo-Raster-Sources.html#cairo-raster-source-release-func-t">cairo docs</a>
    /// for description.
    /// </para>
    /// </param>
    /// <remarks>
    /// The acquire callback should create a surface (preferably an image surface created to
    /// match the target using <see cref="Surface.CreateSimilarImage(Format, int, int)"/>)
    /// that defines at least the region of interest specified by extents. The surface is allowed to be
    /// the entire sample area, but if it does contain a subsection of the sample area, the surface extents
    /// should be provided by setting the device offset (along with its width and height) using
    /// <see cref="Surface.DeviceOffset"/>.
    /// </remarks>
    public void SetAcquire(cairo_raster_source_acquire_func_t acquire, cairo_raster_source_release_func_t release)
    {
        this.CheckDisposed();
        cairo_raster_source_pattern_set_acquire(this.Handle, acquire, release);
    }

    [MemberNotNull(nameof(_state))]
    private void ValidateCallbackData()
    {
        IntPtr userData     = this.CallbackData;
        IntPtr statePointer = GCHandle.ToIntPtr(_stateHandle);

        if (userData != statePointer)
        {
            throw new InvalidOperationException($"""
                When {nameof(CallbackData)} is changed, the managed delegates can't be used.
                It can be reset with {nameof(ResetCallbackDataToDefault)}.
                """);
        }

        if (_state is null)
        {
            throw new InvalidOperationException($"""
                When the overloads with the managed delegates are used, then this type has
                to be constructed with the ctor that takes the managed delegate.
                """);
        }
    }

    /// <summary>
    /// Specifies the callbacks used to generate the image surface for a rendering operation
    /// (acquire).
    /// </summary>
    /// <param name="acquire">acquire delegate</param>
    /// <remarks>
    /// The implicit release disposes the created <see cref="Surface"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// When <see cref="CallbackData"/> was changed from the default value.
    /// </exception>
    public void SetAcquire(Acquire acquire)
    {
        this.CheckDisposed();
        this.ValidateCallbackData();

        _state.Acquire = acquire;

        cairo_raster_source_pattern_set_acquire(this.Handle, &AcquireCore, &ReleaseCore);

        static cairo_surface_t* AcquireCore(cairo_pattern_t* pattern, void* callbackData, cairo_surface_t* target, ref RectangleInt extents)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(new IntPtr(callbackData));
            State state       = (State)gcHandle.Target!;

            Debug.Assert(state.Acquire is not null);

            // Here just wrapper objects, w/o memory management, thus no Dispose needed.
            Pattern? patternObj = Pattern.Lookup(pattern, isOwnedByCairo: true, needsDestroy: false);
            Surface? surfaceObj = Surface.Lookup(target , isOwnedByCairo: true, needsDestroy: false);

            Surface result = state.Acquire(patternObj, state.UserData, surfaceObj, ref extents);
            return result.Handle;
        }

        static void ReleaseCore(cairo_pattern_t* pattern, void* callbackData, cairo_surface_t* surface)
        {
            SurfaceNative.cairo_surface_destroy(surface);
        }
    }

    /// <summary>
    /// Queries the current acquire and release callbacks.
    /// </summary>
    /// <param name="acquire">return value for the current acquire callback</param>
    /// <param name="release">return value for the current release callback</param>
    public void GetAcquire(out cairo_raster_source_acquire_func_t acquire, out cairo_raster_source_release_func_t release)
    {
        this.CheckDisposed();
        cairo_raster_source_pattern_get_acquire(this.Handle, out acquire, out release);
    }

    /// <summary>
    /// Sets the callback that will be used whenever a snapshot is taken of the pattern, that is
    /// whenever the current contents of the pattern should be preserved for later use.
    /// This is typically invoked whilst printing.
    /// </summary>
    /// <param name="snapshot">
    /// snapshot callback
    /// <para>
    /// See <a href="https://www.cairographics.org/manual/cairo-Raster-Sources.html#cairo-raster-source-snapshot-func-t">cairo docs</a>
    /// for description.
    /// </para>
    /// </param>
    public void SetSnapshot(cairo_raster_source_snapshot_func_t snapshot)
    {
        this.CheckDisposed();
        cairo_raster_source_pattern_set_snapshot(this.Handle, snapshot);
    }

    /// <summary>
    /// Queries the current snapshot callback.
    /// </summary>
    /// <param name="snapshot"> the current snapshot callback</param>
    public void GetSnapshot(out cairo_raster_source_snapshot_func_t snapshot)
    {
        this.CheckDisposed();
        snapshot = cairo_raster_source_pattern_get_snapshot(this.Handle);
    }

    /// <summary>
    /// Updates the copy callback which is used whenever a temporary copy of the pattern is taken.
    /// </summary>
    /// <param name="copy">
    /// the copy callback
    /// <para>
    /// See <a href="https://www.cairographics.org/manual/cairo-Raster-Sources.html#cairo-raster-source-copy-func-t">cairo docs</a>
    /// for description.
    /// </para>
    /// </param>
    public void SetCopy(cairo_raster_source_copy_func_t copy)
    {
        this.CheckDisposed();
        cairo_raster_source_pattern_set_copy(this.Handle, copy);
    }

    /// <summary>
    /// Queries the current copy callback.
    /// </summary>
    /// <param name="copy"> the current copy callback</param>
    public void GetCopy(out cairo_raster_source_copy_func_t copy)
    {
        this.CheckDisposed();
        copy = cairo_raster_source_pattern_get_copy(this.Handle);
    }

    /// <summary>
    /// Updates the finish callback which is used whenever a pattern (or a copy thereof)
    /// will no longer be used.
    /// </summary>
    /// <param name="finish">
    /// the finish callback
    /// <para>
    /// See <a href="https://www.cairographics.org/manual/cairo-Raster-Sources.html#cairo-raster-source-finish-func-t">cairo docs</a>
    /// for description.
    /// </para>
    /// </param>
    public void SetFinish(cairo_raster_source_finish_func_t finish)
    {
        this.CheckDisposed();
        cairo_raster_source_pattern_set_finish(this.Handle, finish);
    }

    /// <summary>
    /// Queries the current finish callback.
    /// </summary>
    /// <param name="finish">the current finish callback</param>
    public void GetFinish(out cairo_raster_source_finish_func_t finish)
    {
        this.CheckDisposed();
        finish = cairo_raster_source_pattern_get_finish(this.Handle);
    }

    private class State
    {
        public object? UserData   { get; set; }
        public Acquire? Acquire   { get; set; }
        public Release? Release   { get; set; }
        public Snapshot? Snapshot { get; set; }
        public Copy? Copy         { get; set; }
        public Finish? Finish     { get; set; }
    }
}
