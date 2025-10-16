// (c) gfoidl, all rights reserved

global using unsafe cairo_write_func_t   = delegate*<void*, byte*, uint, Cairo.Status>;
global using unsafe cairo_read_func_t    = delegate*<void*, byte*, uint, Cairo.Status>;
global using unsafe cairo_destroy_func_t = delegate*<void*, void>;
