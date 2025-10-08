// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.ErrorHandling;

namespace Cairo;

public static unsafe class Error
{
    extension(Status status)
    {
        /// <summary>
        /// Provides a human-readable description of a <see cref="Status"/>
        /// </summary>
        /// <returns>
        /// a string representation of the status
        /// </returns>
        public string GetString()
        {
            sbyte* raw = ErrorNative.cairo_status_to_string(status);
            return new string(raw);
        }

        /// <summary>
        /// Throws a <see cref="CairoException"/> when the status is not <see cref="Status.Success"/>.
        /// </summary>
        /// <exception cref="CairoException">status != <see cref="Status.Success"/></exception>
        [StackTraceHidden]
        [DebuggerNonUserCode]
        public void ThrowIfNotSuccess()
        {
            if (status != Status.Success)
            {
                throw new CairoException(status);
            }
        }

        [StackTraceHidden]
        [DebuggerNonUserCode]
        internal void ThrowIfStatus(Status value)
        {
            if (status == value)
            {
                throw new CairoException(status);
            }
        }
    }

    /// <summary>
    /// Resets all static data within cairo to its original state, (ie. identical to the state
    /// at the time of program invocation). For example, all caches within cairo will be flushed empty.
    /// </summary>
    /// <remarks>
    /// This method is intended to be useful when using memory-checking tools such as valgrind.
    /// When valgrind's memcheck analyzes a cairo-using program without a call to <see cref="ResetStaticData"/>,
    /// it will report all data reachable via cairo's static objects as "still reachable". Calling
    /// <see cref="ResetStaticData"/> just prior to program termination will make it easier to get squeaky
    /// clean reports from valgrind.
    /// <para>
    /// WARNING: It is only safe to call this method when there are no active cairo objects remaining,
    /// (ie. the appropriate destroy functions have been called as necessary). If there are active cairo
    /// objects, this call is likely to cause a crash, (eg. an assertion failure due to a hash table
    /// being destroyed when non-empty).
    /// </para>
    /// </remarks>
    public static void ResetStaticData()
    {
        ErrorNative.cairo_debug_reset_static_data();
    }
}
