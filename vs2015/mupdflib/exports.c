#include <stdio.h>
#include "mupdf\fitz.h"

__declspec(dllexport) fz_context* new_context()
{
	fz_context* ctx;

	fz_var(ctx);

	ctx = fz_new_context(NULL, NULL, FZ_STORE_DEFAULT);
	if (ctx == NULL)
	{
		return NULL;
	}

	fz_try(ctx)
	{
		fz_register_document_handlers(ctx);
	}
	fz_catch(ctx)
	{
		fz_drop_context(ctx);
		return NULL;
	}

	return ctx;
}

__declspec(dllexport) void drop_context(fz_context* ctx)
{
	fz_drop_context(ctx);
}

__declspec(dllexport) fz_document* open_document(fz_context* ctx, const wchar_t* filename, const char* mimetype)
{
	fz_stream* file = NULL;
	fz_document* doc = NULL;

	fz_try(ctx)
	{
		file = fz_open_file_w(ctx, filename);
	}
	fz_catch(ctx)
	{
		return NULL;
	}

	fz_try(ctx)
	{
		doc = fz_open_document_with_stream(ctx, mimetype, file);
	}
	fz_always(ctx)
	{
		fz_drop_stream(ctx, file);
	}
	fz_catch(ctx)
	{
		doc = NULL;
	}

	return doc;
}

__declspec(dllexport) void drop_document(fz_context* ctx, fz_document* doc)
{
	fz_drop_document(ctx, doc);
}

__declspec(dllexport) int get_pagecount(fz_context* ctx, fz_document* doc)
{
	int pagecount = 0;
	fz_var(doc);

	fz_try(ctx)
	{
		pagecount = fz_count_pages(ctx, doc);
	}
	fz_catch(ctx)
	{
		pagecount = 0;
	}

	return pagecount;
}

__declspec(dllexport) fz_page* load_page(fz_context* ctx, fz_document* doc, int pagenumber)
{
	fz_page* page;

	fz_var(doc);
	fz_var(page);

	fz_try(ctx)
	{
		page = fz_load_page(ctx, doc, pagenumber);
	}
	fz_catch(ctx)
	{
		if (page != NULL)
		{
			fz_drop_page(ctx, page);
		}

		page = NULL;
	}

	return page;
}

__declspec(dllexport) void get_pagesize(fz_context* ctx, fz_page* page, float* width, float* height)
{
	fz_rect bbox;
	fz_try(ctx)
	{
		fz_bound_page(ctx, page, &bbox);
		*width = bbox.x1 - bbox.x0;
		*height = bbox.y1 - bbox.y0;
	}
	fz_catch(ctx)
	{
		*width = 0;
		*height = 0;
	}
}

__declspec(dllexport) void drop_page(fz_context* ctx, fz_page* page)
{
	fz_drop_page(ctx, page);
}

__declspec(dllexport) fz_display_list* load_displaylist(fz_context* ctx, fz_page* page)
{
	fz_display_list* list = NULL;

	fz_try(ctx)
	{
		list = fz_new_display_list_from_page(ctx, page);
	}
	fz_catch(ctx)
	{
		if (list != NULL)
		{
			fz_drop_display_list(ctx, list);
		}
		
		list = NULL;
	}

	return list;
}

__declspec(dllexport) int run_displaylist(fz_context* ctx, fz_display_list* list, fz_pixmap* pix, float scale, int rotation, int x0, int y0)
{
	fz_matrix ctm;
	fz_device* dev = NULL;
	int rc = 0;
	
	fz_scale(&ctm, scale, scale);
	fz_pre_rotate(&ctm, rotation);
	
	fz_rect tbounds;
	fz_bound_display_list(ctx, list, &tbounds);
	fz_transform_rect(&tbounds, &ctm);

	fz_irect ibounds;
	fz_round_rect(&ibounds, &tbounds);

	pix->x = ibounds.x0 + x0;
	pix->y = ibounds.y0 + y0;
	
	fz_var(dev);

	fz_try(ctx)
	{
		fz_clear_pixmap_with_value(ctx, pix, 255);
		dev = fz_new_draw_device(ctx, &ctm, pix);
		fz_run_display_list(ctx, list, dev, &fz_identity, NULL, NULL);
	}
	fz_always(ctx)
	{
		if (dev != NULL)
		{
			fz_close_device(ctx, dev);
			fz_drop_device(ctx, dev);
		}
	}
	fz_catch(ctx)
	{
		rc = 1;
	}

	return rc;
}

__declspec(dllexport) void drop_displaylist(fz_context* ctx, fz_display_list* list)
{
	fz_drop_display_list(ctx, list);
}

__declspec(dllexport) fz_pixmap* new_pixmap_argb(fz_context* ctx, int width, int height)
{
	fz_pixmap* pix = NULL;

	fz_var(pix);

	fz_try(ctx)
	{
		pix = fz_new_pixmap(ctx, fz_device_bgr(ctx), width, height, NULL, 1);
	}
	fz_catch(ctx)
	{
		if (pix != NULL)
		{
			fz_drop_pixmap(ctx, pix);
			pix = NULL;
		}
	}

	return pix;
}

__declspec(dllexport) void drop_pixmap(fz_context* ctx, fz_pixmap* pix)
{
	fz_drop_pixmap(ctx, pix);
}

__declspec(dllexport) char* get_pixmap_data(fz_context* ctx, fz_pixmap* pix, int* width, int* height)
{
	if (pix == NULL)
	{
		return NULL;
	}

	*width = pix->w;
	*height = pix->h;
	return pix->samples;
}