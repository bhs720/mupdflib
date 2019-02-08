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
        /// <summary>
        /// Create a new MuPDF context. Returns IntPtr.Zero on failure
        /// </summary>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr new_context();

        /// <summary>
        /// Free a MuPDF context
        /// </summary>
        /// <param name="ctx">Pointer to context</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_context(IntPtr ctx);

        /// <summary>
        /// Open a file (PDF, TIF, JPG, PNG, CBZ, and others). Returns IntPtr.Zero on failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="filename">Filename of the document to open</param>
        /// <param name="mimetype">Filename extension. example: ".pdf"</param>
        /// <returns>Pointer to document. IntPtr.Zero on failure</returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr open_document(IntPtr ctx, [MarshalAs(UnmanagedType.LPWStr)]string filename, [MarshalAs(UnmanagedType.LPStr)]string mimetype);

        /// <summary>
        /// Free the MuPDF document
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="doc">Pointer to document</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_document(IntPtr ctx, IntPtr doc);

        /// <summary>
        /// Get page count of a document. Returns 0 on failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="doc">MuPDF document</param>
        /// <returns>The page count. Zero on failure</returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_pagecount(IntPtr ctx, IntPtr doc);

        /// <summary>
        /// Load a page from a document. Returns IntPtr.Zero on failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="doc">MuPDF document</param>
        /// <param name="pagenumber">Which page to load</param>
        /// <returns>Pointer to the page. IntPtr.Zero on failure</returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr load_page(IntPtr ctx, IntPtr doc, int pagenumber);

        /// <summary>
        /// Get size of a page in points (1 point = 1/72 inch). Sets width & height to 0 in case of failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="page">MuPDF page</param>
        /// <param name="width">Page width in points</param>
        /// <param name="height">Page height in points</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void get_pagesize(IntPtr ctx, IntPtr page, out float width, out float height);

        /// <summary>
        /// Free a MuPDF page
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="page">MuPDF page</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_page(IntPtr ctx, IntPtr page);

        /// <summary>
        /// Load resources required to render a page. Can be used multiple times with run_displaylist. Returns IntPtr.Zero on failure
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="page">MuPDF page</param>
        /// <returns>Pointer to a displaylist. IntPtr.Zero on failure</returns>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr load_displaylist(IntPtr ctx, IntPtr page);

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

        /// <summary>
        /// Free a MuPDF displaylist
        /// </summary>
        /// <param name="ctx">MuPDF context</param>
        /// <param name="list">MuPDF displaylist</param>
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_displaylist(IntPtr ctx, IntPtr list);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr new_pixmap_argb(IntPtr ctx, int width, int height);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_pixmap(IntPtr ctx, IntPtr pix);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_pixmap_data(IntPtr ctx, IntPtr pix, out int width, out int height);
    }
}
