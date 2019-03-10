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

__declspec(dllexport) fz_buffer* load_plaintext_buffer(fz_context* ctx, fz_display_list* list)
{
	fz_stext_page* stext = NULL;
	fz_buffer* buff = NULL;

	fz_var(stext);
	fz_var(buff);

	fz_try(ctx)
	{
		stext = fz_new_stext_page_from_display_list(ctx, list, NULL);
		buff = fz_new_buffer_from_stext_page(ctx, stext);
	}
	fz_always(ctx)
	{
		if (stext != NULL)
		{
			fz_drop_stext_page(ctx, stext);
		}
	}
	fz_catch(ctx)
	{
		buff = NULL;
	}

	return buff;
}

__declspec(dllexport) void drop_buffer(fz_context* ctx, fz_buffer* buff)
{
	if (buff != NULL)
	{
		fz_drop_buffer(ctx, buff);
	}
}

__declspec(dllexport) const char* get_string_from_buffer(fz_context* ctx, fz_buffer* buff)
{
	const char* string = NULL;

	fz_try(ctx)
	{
		string = fz_string_from_buffer(ctx, buff);
	}
	fz_catch(ctx)
	{
		string = NULL;
	}

	return string;
}

__declspec(dllexport) int write_page_plaintext_to_file(fz_context* ctx, fz_page* page, const char* filename)
{
	fz_stext_page* stext = NULL;
	fz_buffer* buff = NULL;
	fz_output* out = NULL;
	int rc = 0;

	fz_var(stext);
	fz_var(buff);
	fz_var(out);

	fz_try(ctx)
	{
		stext = fz_new_stext_page_from_page(ctx, page, NULL);
		buff = fz_new_buffer_from_stext_page(ctx, stext);
		out = fz_new_output_with_path(ctx, filename, 0);
		fz_print_stext_page_as_text(ctx, out, stext);
		fz_flush_output(ctx, out);
		fz_close_output(ctx, out);
	}
	fz_always(ctx)
	{
		if (stext != NULL)
		{
			fz_drop_stext_page(ctx, stext);
		}

		if (buff != NULL)
		{
			fz_drop_buffer(ctx, buff);
		}

		if (out != NULL)
		{
			fz_drop_output(ctx, out);
		}
	}
	fz_catch(ctx)
	{
		rc = 1;
	}

	return rc;
}

__declspec(dllexport) fz_rect* search_for_text(fz_context* ctx, fz_display_list* list, const char* needle, int hit_max, int* hit_count)
{
	fz_rect* hit_bbox = NULL;

	fz_try(ctx)
	{
		hit_bbox = (fz_rect*)fz_malloc_array(ctx, hit_max, sizeof(fz_rect));
		*hit_count = fz_search_display_list(ctx, list, needle, hit_bbox, hit_max);
	}
	fz_catch(ctx)
	{
		if (hit_bbox != NULL)
		{
			fz_free(ctx, hit_bbox);
		}

		hit_bbox = NULL;
		*hit_count = 0;
	}

	return hit_bbox;
}

__declspec(dllexport) void free_fz(fz_context* ctx, void* p)
{
	if (p != NULL)
		fz_free(ctx, p);
}
