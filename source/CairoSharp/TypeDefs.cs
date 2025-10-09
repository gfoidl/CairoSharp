// (c) gfoidl, all rights reserved

global using unsafe cairo_write_func_t   = delegate*<void*, byte*, uint, Cairo.Status>;
global using unsafe cairo_read_func_t    = delegate*<void*, byte*, uint, Cairo.Status>;
global using unsafe cairo_destroy_func_t = delegate*<void*, void>;

global using unsafe cairo_surface_observer_callback_t = delegate*<void*, void*, void*, void>;

global using unsafe cairo_user_scaled_font_init_func_t             = delegate*<void*, void*, ref Cairo.Fonts.FontExtents, Cairo.Status>;
global using unsafe cairo_user_scaled_font_render_glyph_func_t     = delegate*<void*, System.Runtime.InteropServices.CULong, void*, ref Cairo.Fonts.FontExtents, Cairo.Status>;
global using unsafe cairo_user_scaled_font_text_to_glyphs_func_t   = delegate*<void*, byte*, int, Cairo.Drawing.Text.Glyph**, ref int, Cairo.Drawing.Text.TextCluster**, ref int, out Cairo.Drawing.Text.ClusterFlags, Cairo.Status>;
global using unsafe cairo_user_scaled_font_unicode_to_glyph_func_t = delegate*<void*, System.Runtime.InteropServices.CULong, out System.Runtime.InteropServices.CULong, Cairo.Status>;

global using unsafe cairo_raster_source_acquire_func_t  = delegate*<void*, void*, void*, ref Cairo.RectangleInt, void*>;
global using unsafe cairo_raster_source_release_func_t  = delegate*<void*, void*, void*, void>;
global using unsafe cairo_raster_source_snapshot_func_t = delegate*<void*, void*, Cairo.Status>;
global using unsafe cairo_raster_source_copy_func_t     = delegate*<void*, void*, void*, Cairo.Status>;
global using unsafe cairo_raster_source_finish_func_t   = delegate*<void*, void*, void>;
