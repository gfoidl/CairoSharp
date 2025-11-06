// (c) gfoidl, all rights reserved

// The web colors are the same as we already have included, just all lowercase.
//#define EMIT_WEB_COLORS

using CreateKnownColors;

KnownColorGenerator.Run();

#if EMIT_WEB_COLORS
KnownColorWebGenerator.Run();
#endif

Console.WriteLine("bye.");
