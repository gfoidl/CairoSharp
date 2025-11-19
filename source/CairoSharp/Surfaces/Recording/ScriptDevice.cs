// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Cairo.Surfaces.Recording.ScriptSurfaceNative;

namespace Cairo.Surfaces.Recording;

/// <summary>
/// An output device for emitting scripts.
/// </summary>
public sealed unsafe class ScriptDevice : Device
{
    private GCHandle _streamHandle;     // mutable struct

    private ScriptDevice((IntPtr Device, GCHandle StreamHandle) arg)
        : base((cairo_device_t*)arg.Device.ToPointer(), needsReference: false)
        => _streamHandle = arg.StreamHandle;

    /// <summary>
    /// Creates a output device for emitting the script, used when creating the individual surfaces.
    /// </summary>
    /// <param name="fileName">the name (path) of the file to write the script to</param>
    /// <exception cref="CairoException">when construction fails</exception>
    public ScriptDevice(string fileName) : base(cairo_script_create(fileName), needsReference: false)
        => this.Status.ThrowIfNotSuccess();

    /// <summary>
    /// Creates a output device for emitting the script, used when creating the individual surfaces.
    /// </summary>
    /// <param name="stream">The stream to which the script is written to</param>
    /// <exception cref="CairoException">when construction fails</exception>
    /// <exception cref="ArgumentException">the stream is not writeable</exception>
    public ScriptDevice(Stream stream) : this(CreateForWriteStream(stream)) { }

    private static (IntPtr, GCHandle) CreateForWriteStream(Stream stream)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("Stream must be writeable");
        }

        GCHandle streamHandle        = GCHandle.Alloc(stream, GCHandleType.Normal);
        void* state                  = GCHandle.ToIntPtr(streamHandle).ToPointer();
        cairo_write_func_t writeFunc = &WriteFunc;

        void* device = cairo_script_create_for_stream(writeFunc, state);

        return (new IntPtr(device), streamHandle);

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
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

    protected override void DisposeCore(cairo_device_t* device)
    {
        base.DisposeCore(device);

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
