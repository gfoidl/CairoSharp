// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Cairo;

[CustomMarshaller(typeof(string), MarshalMode.Default, typeof(NativeConstCharMarshaller))]
internal static unsafe class NativeConstCharMarshaller
{
    public static string? ConvertToManaged(sbyte* utf8)
    {
        return utf8 is null
            ? null
            : new string(utf8);
    }
}
//-----------------------------------------------------------------------------
[CustomMarshaller(typeof(ReadOnlySpan<byte>), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToUnmanagedIn))]
internal static unsafe class Utf8SpanMarshaller
{
    public ref struct ManagedToUnmanagedIn
    {
        public static int BufferSize => 0x100;

        private ReadOnlySpan<byte> _managedArray;
        private bool               _allocated;

        public void FromManaged(ReadOnlySpan<byte> managed, Span<byte> buffer)
        {
            if (managed.IsEmpty)
            {
                return;
            }

            // ptr[length] is 0-terminated by .NET like for strings and utf8-literals "..."u8.
            // Reading just after the end is safe, but not beyond that.
            if (managed[^1] == 0 || Unsafe.Add(ref MemoryMarshal.GetReference(managed), (uint)managed.Length) == 0)
            {
                _managedArray = managed;
                return;
            }

            // >= for null terminator
            if (managed.Length >= buffer.Length)
            {
                int requiredLength = managed.Length + 1;    // +1 for null terminator
                buffer             = new Span<byte>((byte*)NativeMemory.Alloc((uint)requiredLength), requiredLength);
                _allocated         = true;
            }

            managed.CopyTo(buffer);
            buffer[managed.Length] = 0;     // null termination

            _managedArray = buffer;
        }
        //-------------------------------------------------------------------------
        public readonly ref byte GetPinnableReference() => ref MemoryMarshal.GetReference(_managedArray);
        public readonly byte* ToUnmanaged()             => (byte*)Unsafe.AsPointer(ref this.GetPinnableReference());
        //-------------------------------------------------------------------------
        public readonly void Free()
        {
            if (_allocated)
            {
                byte* nativeMemory = this.ToUnmanaged();
                NativeMemory.Free(nativeMemory);
            }
        }
    }
}
