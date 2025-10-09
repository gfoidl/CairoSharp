// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.PostScript.PostScriptSurfaceNative;

namespace Cairo.Surfaces.PostScript;

/// <summary>
/// <see cref="PostScriptLevel"/> is used to describe the language level of the PostScript
/// Language Reference that a generated PostScript file will conform to.
/// </summary>
public enum PostScriptLevel
{
    /// <summary>
    /// The language level 2 of the PostScript specification. (Since 1.6)
    /// </summary>
    Level2,

    /// <summary>
    /// The language level 3 of the PostScript specification. (Since 1.6)
    /// </summary>
    Level3
}

public static unsafe class PostScriptLevelExtensions
{
    extension(PostScriptLevel level)
    {
        /// <summary>
        /// Get the string representation of the given level id. This method will return <c>null</c> if level
        /// id isn't valid. See <see cref="GetSupportedLevels"/> for a way to get the list of valid level ids.
        /// </summary>
        /// <returns>the string associated to given level.</returns>
        public string? GetString()
        {
            sbyte* tmp = cairo_ps_level_to_string(level);
            return tmp is not null ? new string(tmp) : null;
        }
    }

    extension(PostScriptSurface)
    {
        /// <summary>
        /// Used to retrieve the list of supported levels. See <see cref="PostScriptSurface.RestricToLevel(PostScriptLevel)"/>.
        /// </summary>
        /// <returns>supported level list</returns>
        public static ReadOnlySpan<PostScriptLevel> GetSupportedLevels()
        {
            cairo_ps_get_levels(out PostScriptLevel* levels, out int length);
            return new ReadOnlySpan<PostScriptLevel>(levels, length);
        }
    }
}
