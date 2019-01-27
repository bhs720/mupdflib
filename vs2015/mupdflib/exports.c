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

__declspec(dllexport) void drop_page(fz_context* ctx, fz_page* page)
{
	fz_drop_page(ctx, page);
}
