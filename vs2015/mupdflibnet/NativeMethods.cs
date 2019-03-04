using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace mupdflibnet
{
    public class NativeMethods
    {
        public static IntPtr NewContext()
        {
            IntPtr ctx = new_context();
            if (ctx == IntPtr.Zero)
                throw new NativeException("Failed to create context");
            return ctx;
        }

        /// <summary>
        /// Create a new MuPDF context. Returns IntPtr.Zero on failure
        /// </summary>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr new_context();

        public void DropContext(IntPtr ctx)
        {
            if (ctx != IntPtr.Zero)
                drop_context(ctx);
        }

        /// <summary>
        /// Free a MuPDF context
        /// </summary>
        /// <param name="ctx">Pointer to context</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_context(IntPtr ctx);
        
        public static IntPtr OpenDocument(IntPtr ctx, string filename)
        {
            IntPtr doc = open_document(ctx, filename, System.IO.Path.GetExtension(filename));
            if (doc == IntPtr.Zero)
                throw new NativeException("Failed to open document");
            return doc;
        }

        /// <summary>
        /// Open a file (PDF, TIF, JPG, PNG, CBZ, and others). Returns IntPtr.Zero on failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="filename">Filename of the document to open</param>
        /// <param name="mimetype">Mimetype or filename extension. example: ".pdf"</param>
        /// <returns>Pointer to document. IntPtr.Zero on failure</returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr open_document(IntPtr ctx, [MarshalAs(UnmanagedType.LPWStr)]string filename, [MarshalAs(UnmanagedType.LPStr)]string mimetype);
        
        public static void DropDocument(IntPtr ctx, IntPtr doc)
        {
            if (doc != IntPtr.Zero)
                drop_document(ctx, doc);
        }

        /// <summary>
        /// Free the MuPDF document
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="doc">Pointer to document</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_document(IntPtr ctx, IntPtr doc);

        public static int GetPageCount(IntPtr ctx, IntPtr doc)
        {
            int pagecount = get_pagecount(ctx, doc);
            if (pagecount == 0)
                throw new NativeException("Failed to get page count");
            return pagecount;
        }

        /// <summary>
        /// Get page count of a document. Returns 0 on failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="doc">MuPDF document</param>
        /// <returns>The page count. Zero on failure</returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_pagecount(IntPtr ctx, IntPtr doc);

        public static IntPtr LoadPage(IntPtr ctx, IntPtr doc, int pageNumber)
        {
            IntPtr page = load_page(ctx, doc, pageNumber);
            if (page == IntPtr.Zero)
                throw new NativeException("Failed to load page");
            return page;
        }

        /// <summary>
        /// Load a page from a document. Returns IntPtr.Zero on failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="doc">MuPDF document</param>
        /// <param name="pagenumber">Which page to load</param>
        /// <returns>Pointer to the page. IntPtr.Zero on failure</returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr load_page(IntPtr ctx, IntPtr doc, int pagenumber);

        public static System.Drawing.SizeF GetPageSize(IntPtr ctx, IntPtr page)
        {
            float width, height;
            get_pagesize(ctx, page, out width, out height);
            if (width == 0 || height == 0)
                throw new NativeException("Failed to get page size");
            return new System.Drawing.SizeF(width, height);
        }

        /// <summary>
        /// Get size of a page in points (1 point = 1/72 inch). Sets width & height to 0 in case of failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="page">MuPDF page</param>
        /// <param name="width">Page width in points</param>
        /// <param name="height">Page height in points</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void get_pagesize(IntPtr ctx, IntPtr page, out float width, out float height);

        public static void DropPage(IntPtr ctx, IntPtr page)
        {
            if (page != IntPtr.Zero)
                drop_page(ctx, page);
        }

        /// <summary>
        /// Free a MuPDF page
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="page">MuPDF page</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_page(IntPtr ctx, IntPtr page);

        public static IntPtr LoadDisplayList(IntPtr ctx, IntPtr page)
        {
            IntPtr list = load_displaylist(ctx, page);
            if (list == IntPtr.Zero)
                throw new NativeException("Failed to load display list");
            return list;
        }

        /// <summary>
        /// Load resources required to render a page. Can be used multiple times with run_displaylist. Returns IntPtr.Zero on failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="page">MuPDF page</param>
        /// <returns>Pointer to a displaylist. IntPtr.Zero on failure</returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr load_displaylist(IntPtr ctx, IntPtr page);

        public static void RunDisplayList(IntPtr ctx, IntPtr list, IntPtr pix, float scale, int rotation, int x0, int y0)
        {
            if (0 != run_displaylist(ctx, list, pix, scale, rotation, x0, y0))
                throw new NativeException("Failed to run display list");
        }

        /// <summary>
        /// Draw a display list onto a pixmap. Returns 0 for success, 1 for failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="list">MuPDF displaylist</param>
        /// <param name="pix">MuPDF pixmap</param>
        /// <param name="scale">Scale of 1 means draw at 72 dpi</param>
        /// <param name="rotation">Rotation in degrees</param>
        /// <param name="x0">x-offset. Positive numbers shift the image left</param>
        /// <param name="y0">y-offset. Positive numbers shift the image up</param>
        /// <returns>0 for success, 1 for failure</returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int run_displaylist(IntPtr ctx, IntPtr list, IntPtr pix, float scale, int rotation, int x0, int y0);

        public static void DropDisplayList(IntPtr ctx, IntPtr list)
        {
            if (list != IntPtr.Zero)
                drop_displaylist(ctx, list);
        }

        /// <summary>
        /// Free a MuPDF displaylist
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="list">MuPDF displaylist</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_displaylist(IntPtr ctx, IntPtr list);

        public static IntPtr NewPixmapARGB(IntPtr ctx, int width, int height)
        {
            IntPtr pix = new_pixmap_argb(ctx, width, height);
            if (pix == IntPtr.Zero)
                throw new NativeException(string.Format("Failed to create pixmap: {0}x{1} px", width, height));
            return pix;
        }

        /// <summary>
        /// New MuPDF Pixmap. ARGB image is compatible with System.Drawing.Imaging.PixelFormat.Format32bppPArgb. This is a pixmap struct. Use get_pixmap_data() to get pixel data. Returns IntPtr.Zero on failure.
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="width">Desired width in pixels</param>
        /// <param name="height">Desired height in pixels</param>
        /// <returns></returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr new_pixmap_argb(IntPtr ctx, int width, int height);

        public static void DropPixmap(IntPtr ctx, IntPtr pix)
        {
            if (pix != IntPtr.Zero)
                drop_pixmap(ctx, pix);
        }

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_pixmap(IntPtr ctx, IntPtr pix);

        public static IntPtr GetPixmapData(IntPtr ctx, IntPtr pix, out int width, out int height)
        {
            IntPtr data = get_pixmap_data(ctx, pix, out width, out height);
            if (data == IntPtr.Zero)
                throw new NativeException("Failed to get pixmap data");
            return data;
        }

        public static IntPtr GetPixmapData(IntPtr ctx, IntPtr pix)
        {
            int w, h;
            IntPtr data = get_pixmap_data(ctx, pix, out w, out h);
            if (data == IntPtr.Zero)
                throw new NativeException("Failed to get pixmap data");
            return data;
        }

        /// <summary>
        /// Get the pixel data from a pixmap struct. Returns IntPtr.Zero on failure.
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="pix">MuPDF pixmap struct</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <returns></returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_pixmap_data(IntPtr ctx, IntPtr pix, out int width, out int height);
    }
}
