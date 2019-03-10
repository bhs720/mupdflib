#include <stdio.h>
#include "mupdf\fitz.h"
#include "mupdf\fitz\writer.h"

extern int run_displaylist(fz_context* ctx, fz_display_list* list, fz_pixmap* pix, float scale, int rotate, int x0, int y0);
extern fz_buffer* load_plaintext_buffer(fz_context* ctx, fz_display_list* list);
extern void drop_buffer(fz_context* ctx, fz_buffer* buff);
extern char* get_string_from_buffer(fz_context* ctx, fz_buffer* buff);
extern int write_page_plaintext_to_file(fz_context* ctx, fz_page* page, const char* filename);
extern fz_rect* search_for_text(fz_context* ctx, fz_display_list* list, const char* needle, int hit_max, int* hit_count);

void test2(char*, char*);

int main(int argc, char **argv)
{
	const char* filename = "C:\\temp\\plans.pdf";
	const char* output = "C:\\temp\\stext.xml";
	test2(filename, output);
	printf("Program finished\n");
	getc(stdin);
	return 0;
}

void extractText(char* filename, char* output)
{
	fz_context* ctx = fz_new_context(NULL, NULL, FZ_STORE_DEFAULT);
	fz_register_document_handlers(ctx);
	printf("Opening file: %s\n", filename);
	fz_document* doc = fz_open_document(ctx, filename);
	fz_page* page = fz_load_page(ctx, doc, 1);
	fz_rect bounds;
	fz_bound_page(ctx, page, &bounds);

	fz_irect bbox;
	fz_irect_from_rect(&bbox, &bounds);

	fz_display_list* list = fz_new_display_list_from_page(ctx, page);

	fz_rect* hit_bbox = fz_malloc_array(ctx, sizeof(fz_rect), 256);
	const char* needle = "accounts";
	int hits = fz_search_display_list(ctx, list, needle, hit_bbox, 256);


	fz_free(ctx, hit_bbox);

	fz_stext_page* stext = fz_new_stext_page_from_display_list(ctx, list, NULL);


	fz_output* out = fz_new_output_with_path(ctx, output, 0);
	//fz_print_stext_page_as_html(ctx, out, stext);
	//fz_print_stext_page_as_xml(ctx, out, stext);
	fz_buffer* buf = fz_new_buffer_from_stext_page(ctx, stext);
	const char* str = fz_string_from_buffer(ctx, buf);



	fz_drop_buffer(ctx, buf);
	fprintf(stdout, "%s\r\n", str);

	fz_flush_output(ctx, out);
	fz_close_output(ctx, out);
	fz_drop_output(ctx, out);

	fz_drop_stext_page(ctx, stext);

	fz_drop_display_list(ctx, list);
	fz_drop_page(ctx, page);
	fz_drop_context(ctx);
}

void test2(char* filename, char* output)
{
	fz_context* ctx = fz_new_context(NULL, NULL, FZ_STORE_DEFAULT);
	fz_register_document_handlers(ctx);
	printf("Opening file: %s\n", filename);
	fz_document* doc = fz_open_document(ctx, filename);
	fz_page* page = fz_load_page(ctx, doc, 8);
	fz_display_list* list = fz_new_display_list_from_page(ctx, page);

	fz_buffer* buff = load_plaintext_buffer(ctx, list);
	char* string = get_string_from_buffer(ctx, buff);
	fprintf(stdout, "%s\r\n", string);
	fflush(stdout);
	drop_buffer(ctx, buff);

	write_page_plaintext_to_file(ctx, page, output);

	int hit_count = 0;
	fz_rect* rects = search_for_text(ctx, list, "Addendum", 256, &hit_count);

	fz_drop_display_list(ctx, list);
	fz_drop_page(ctx, page);
	fz_drop_context(ctx);
}

void test1(char* filename, char* output)
{
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
}