// (c) gfoidl, all rights reserved

global using unsafe FT_Face    = Cairo.Extensions.Fonts.FreeType.FT_FaceRec_*;
global using unsafe FT_Library = Cairo.Extensions.Fonts.FreeType.FT_LibraryRec_*;
global using FT_Long           = System.Runtime.InteropServices.CLong;

namespace Cairo.Extensions.Fonts.FreeType;

internal struct FT_LibraryRec_ { }
internal struct FT_FaceRec_    { }
