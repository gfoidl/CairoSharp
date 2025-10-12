// (c) gfoidl, all rights reserved

using System.Diagnostics;
using static Cairo.Extensions.Fonts.FreeType.FreeTypeNative;

namespace Cairo.Extensions.Fonts.FreeType;

public static unsafe class Error
{
    extension(FTError errorCode)
    {
        /// <summary>
        /// Provides a human-readable description of a <see cref="FTError"/>
        /// </summary>
        /// <returns>
        /// a string representation of the status
        /// </returns>
        public string? GetString()
        {
            sbyte* raw = FT_Error_String(errorCode);

            return raw is not null ? new string(raw) : null;
        }

        /// <summary>
        /// Throws a <see cref="FreeTypeException"/> when the status is not <see cref="FTError.FT_Err_Ok"/>.
        /// </summary>
        /// <exception cref="FreeTypeException">errorCode != <see cref="FTError.FT_Err_Ok"/></exception>
        [StackTraceHidden]
        [DebuggerNonUserCode]
        public void ThrowIfNotSuccess()
        {
            if (errorCode != FTError.FT_Err_Ok)
            {
                throw new FreeTypeException(errorCode);
            }
        }
    }
}
