using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace mupdflibnet
{
    public class PDFPage : IDisposable
    {
        public PDFDocument Parent { get; private set; }

        /// <summary>
        /// Zero-based page number
        /// </summary>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Page width in points
        /// </summary>
        public float Width { get; private set; }

        /// <summary>
        /// Page height in points
        /// </summary>
        public float Height { get; private set; }

        public IntPtr NativePage { get; private set; }
        public IntPtr NativeDisplayList { get; private set; }

        public PDFPage(PDFDocument parent, int pageNumber)
        {
            Parent = parent;
            PageNumber = pageNumber;
            NativePage = NativeMethods.load_page(parent.NativeContext, parent.NativeDocument, PageNumber);
            if (NativePage == IntPtr.Zero)
                throw new Exception("Failed to load page");

            float w, h;
            NativeMethods.get_pagesize(parent.NativeContext, NativePage, out w, out h);
            if (w == 0 || h == 0)
                throw new Exception("Failed to get page size");

            Width = w;
            Height = h;

            Debug.Print(this.ToString());
        }

        public override string ToString()
        {
            return string.Format("Page={0} Width={1:F2} Height={2:F2}", PageNumber + 1, Width, Height);
        }

        public void Dispose()
        {
            //Debug.Print("Page={0} Dispose()", PageNumber + 1);

            if (NativeDisplayList != IntPtr.Zero)
            {
                NativeMethods.drop_displaylist(Parent.NativeContext, NativeDisplayList);
                NativeDisplayList = IntPtr.Zero;
            }

            if (NativePage != IntPtr.Zero)
            {
                NativeMethods.drop_page(Parent.NativeContext, NativePage);
                NativePage = IntPtr.Zero;
            }   

            return;
        }
    }
}
