// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.InteropServices;
using unsafe NativeFactory = delegate*<delegate*<void*, byte*, uint, Cairo.Status>, void*, double, double, void*>;

namespace Cairo.Surfaces;

/// <summary>
/// Base class for <see cref="Surface"/> that allow stream operations.
/// </summary>
public abstract unsafe class StreamSurface : Surface
{
    protected GCHandle _stateHandle;        // mutable struct

    protected StreamSurface(void* handle, bool owner, bool throwOnConstructionError)
        : base(handle, owner, throwOnConstructionError) { }

    protected StreamSurface((IntPtr Handle, GCHandle StateHandle) arg, bool throwOnConstructionError)
        : base(arg.Handle.ToPointer(), owner: true, throwOnConstructionError)
        => _stateHandle = arg.StateHandle;

    protected override unsafe void DisposeCore(void* handle)
    {
        base.DisposeCore(handle);

        // Need to free the surface first, so that the write function (if any)
        // can be called on a valid handle.
        // So it's like: dispose -> write func -> stream handle free
        if (_stateHandle.IsAllocated)
        {
            _stateHandle.Free();
        }
    }

    protected static (IntPtr, GCHandle) CreateForWriteStream(Stream stream, double width, double height, NativeFactory factory)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("Stream must be writeable");
        }

        GCHandle streamHandle        = GCHandle.Alloc(stream, GCHandleType.Normal);
        void* state                  = GCHandle.ToIntPtr(streamHandle).ToPointer();
        cairo_write_func_t writeFunc = &WriteFunc;

        void* handle = factory(writeFunc, state, width, height);

        return (new IntPtr(handle), streamHandle);

        static Status WriteFunc(void* state, byte* data, uint length)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr((nint)state);
            Debug.Assert(gcHandle.IsAllocated);

            Stream? stream = gcHandle.Target as Stream;
            Debug.Assert(stream is not null);

            ReadOnlySpan<byte> span = new(data, (int)length);
            stream.Write(span);

            return Status.Success;
        }
    }
}
