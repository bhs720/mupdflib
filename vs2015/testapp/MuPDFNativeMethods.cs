using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace testapp
{
    public class MuPDFNativeMethods
    {
        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr new_context();

        [DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void drop_context(IntPtr ctx);

        //[DllImport("mupdflib.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern IntPtr open_document(IntPtr ctx, [MarshalAs(UnmanagedType.LPStr)]string filename);

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
    }
}
