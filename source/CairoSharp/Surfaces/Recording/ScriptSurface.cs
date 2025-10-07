// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Cairo.Surfaces.Recording.ScriptSurfaceNative;

namespace Cairo.Surfaces.Recording;

/// <summary>
/// Script Surfaces — Rendering to replayable scripts
/// </summary>
/// <remarks>
/// The script surface provides the ability to render to a native script that matches
/// the cairo drawing model. The scripts can be replayed using tools under the
/// util / cairo-script directory, or with cairo-perf-trace.
/// </remarks>
public sealed unsafe class ScriptSurface : Device
{
    private GCHandle _streamHandle;     // mutable struct

    private ScriptSurface((IntPtr Handle, GCHandle StreamHandle) arg)
        : base(arg.Handle.ToPointer())
        => _streamHandle = arg.StreamHandle;

    /// <summary>
    /// Creates a output device for emitting the script, used when creating the individual surfaces.
    /// </summary>
    /// <param name="fileName">the name (path) of the file to write the script to</param>
    /// <exception cref="CairoException">when construction fails</exception>
    public ScriptSurface(string fileName)
        : base(cairo_script_create(fileName))
        => this.Status.ThrowIfNotSuccess();

    /// <summary>
    /// Creates a output device for emitting the script, used when creating the individual surfaces.
    /// </summary>
    /// <param name="stream">The stream to which the script is written to</param>
    /// <exception cref="CairoException">when construction fails</exception>
    /// <exception cref="ArgumentException">the stream is not writeable</exception>
    public ScriptSurface(Stream stream) : this(CreateForWriteStream(stream)) { }

    private static (IntPtr, GCHandle) CreateForWriteStream(Stream stream)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("Stream must be writeable");
        }

        GCHandle streamHandle        = GCHandle.Alloc(stream, GCHandleType.Normal);
        void* state                  = GCHandle.ToIntPtr(streamHandle).ToPointer();
        cairo_write_func_t writeFunc = &WriteFunc;

        void* handle = cairo_script_create_for_stream(writeFunc, state);

        return (new IntPtr(handle), streamHandle);

        static Status WriteFunc(void* state, byte* data, uint length)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr((nint)state);
            Debug.Assert(gcHandle.IsAllocated);

            Stream stream = (Stream)gcHandle.Target!;
            ReadOnlySpan<byte> span = new(data, (int)length);

            stream.Write(span);

            return Status.Success;
        }
    }

    protected override void DisposeCore(void* handle)
    {
        base.DisposeCore(handle);

        if (_streamHandle.IsAllocated)
        {
            _streamHandle.Free();
        }
    }

    /// <summary>
    /// Converts the record operations in <paramref name="recordingSurface"/> into a script.
    /// </summary>
    /// <param name="recordingSurface">the recording surface to replay</param>
    public void FromRecordingSurface(RecordingSurface recordingSurface)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(recordingSurface);

        Status status = cairo_script_from_recording_surface(this.Handle, recordingSurface.Handle);

        status.ThrowIfNotSuccess();
    }

    /// <summary>
    /// Gets or sets the output mode of the script.
    /// </summary>
    public ScriptMode Mode
    {
        get
        {
            this.CheckDisposed();
            return cairo_script_get_mode(this.Handle);
        }
        set
        {
            this.CheckDisposed();
            cairo_script_set_mode(this.Handle, value);
        }
    }

    /// <summary>
    /// Create a new surface that will emit its rendering through script.
    /// </summary>
    /// <param name="content">the content of the surface</param>
    /// <param name="width">width in pixels</param>
    /// <param name="height">height in pixels</param>
    /// <returns>
    /// a pointer to the newly created surface. The caller owns the surface and should call <see cref="CairoObject.Dispose()"/>
    /// when done with it.
    /// </returns>
    public Surface ToSurface(Content content, double width, double height)
    {
        this.CheckDisposed();

        void* handle = cairo_script_surface_create(this.Handle, content, width, height);
        return new Surface(handle, owner: true);
    }

    /// <summary>
    /// Create a proxy surface that will render to target and record the operations to device.
    /// </summary>
    /// <param name="target">a target surface to wrap</param>
    /// <returns>
    /// a pointer to the newly created surface. The caller owns the surface and should call <see cref="CairoObject.Dispose()"/>
    /// when done with it.
    /// </returns>
    public Surface CreateForTarget(Surface target)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(target);

        void* handle = cairo_script_surface_create_for_target(this.Handle, target.Handle);
        return new Surface(handle, owner: true);
    }

    /// <summary>
    /// Emit a string verbatim into the script.
    /// </summary>
    /// <param name="comment">the string to emit</param>
    public void WriteComment(string comment)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(comment);

        // the length of the string to write, or -1 to use strlen()
        cairo_script_write_comment(this.Handle, comment, -1);
    }
}
