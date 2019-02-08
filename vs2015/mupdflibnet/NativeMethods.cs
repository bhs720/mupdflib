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
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr new_context();

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_context(IntPtr ctx);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr open_document(IntPtr ctx, [MarshalAs(UnmanagedType.LPWStr)]string filename, [MarshalAs(UnmanagedType.LPStr)]string mimetype);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_document(IntPtr ctx, IntPtr doc);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_pagecount(IntPtr ctx, IntPtr doc);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr load_page(IntPtr ctx, IntPtr doc, int pagenumber);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void get_pagesize(IntPtr ctx, IntPtr page, out float width, out float height);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_page(IntPtr ctx, IntPtr page);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr load_displaylist(IntPtr ctx, IntPtr page);

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int run_displaylist(IntPtr ctx, IntPtr list, IntPtr pix, float scale, int rotation, int x0, int y0);

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
