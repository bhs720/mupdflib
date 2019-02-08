#include <stdio.h>
#include "mupdf\fitz.h"

extern int run_displaylist(fz_context* ctx, fz_display_list* list, fz_pixmap* pix, float scale, int rotate, int x0, int y0);


int main(int argc, char **argv)
{
	const char* filename = "C:\\temp\\scan.pdf";

	fz_context* ctx = fz_new_context(NULL, NULL, FZ_STORE_DEFAULT);
	fz_register_document_handlers(ctx);
	printf("Opening file: %s\n", filename);
	fz_document* doc = fz_open_document(ctx, filename);
	fz_page* page = fz_load_page(ctx, doc, 0);
	fz_rect bounds;
	fz_bound_page(ctx, page, &bounds);

	fz_irect bbox;
	fz_irect_from_rect(&bbox, &bounds);

	fz_display_list* list = fz_new_display_list_from_page(ctx, page);

	int w = bbox.x1 - bbox.x0;
	int h = bbox.y1 - bbox.y0;
	int stride = w * 4;
	size_t datasize = sizeof(unsigned char) * stride * h;
	//unsigned char* samples = (unsigned char*)malloc(datasize);
	unsigned char* samples = (unsigned char*)fz_malloc(ctx, datasize);
	
	if (samples == NULL)
	{
		printf("malloc failed\n");
	}

	fz_pixmap* pix = fz_new_pixmap_with_data(ctx, fz_device_bgr(ctx), w, h, NULL, 1, stride, samples);
		
	run_displaylist(ctx, list, pix, 1, 0, 20, 20);

	fz_output* out = fz_new_output_with_path(ctx, "C:\\temp\\out.png", 0);
	fz_write_pixmap_as_png(ctx, out, pix);

	fz_close_output(ctx, out);
	fz_drop_output(ctx, out);


	fz_pixmap* pix2 = fz_new_pixmap_from_page(ctx, page, &fz_identity, fz_device_bgr(ctx), 1);

	fz_drop_pixmap(ctx, pix2);

	
	//free(samples);
	fz_free(ctx, samples);

	fz_drop_display_list(ctx, list);
	fz_drop_page(ctx, page);
	fz_drop_context(ctx);

	printf("Program finished\n");
	getc(stdin);
	return 0;
}
