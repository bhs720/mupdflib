using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace mupdflibnet
{
    public class PDFDocument : IDisposable
    {
        public string Filename { get; private set; }
        public int PageCount { get; private set; }

        public IntPtr NativeContext { get; private set; }
        public IntPtr NativeDocument { get; private set; }

        public PDFDocument(string filename)
        {
            Filename = filename;

            NativeContext = NativeMethods.new_context();
            if (NativeContext == IntPtr.Zero)
                throw new Exception("Failed to create context");
            
            NativeDocument = NativeMethods.open_document(NativeContext, Filename, Path.GetExtension(Filename));
            if (NativeDocument == IntPtr.Zero)
                throw new Exception("Failed to open document");

            PageCount = NativeMethods.get_pagecount(NativeContext, NativeDocument);
            if (PageCount < 1)
                throw new Exception("Failed to get page count");
        }

        /// <summary>
        /// Load a page by page number. Call <see cref="PDFPage.Dispose"/> when done
        /// </summary>
        /// <param name="pageNumber">Zero-based page number</param>
        /// <returns></returns>
        public PDFPage LoadPage(int pageNumber)
        {
            return new PDFPage(this, pageNumber);
        }

        public void Dispose()
        {
            //Debug.Print("Document.Dispose()");

            if (NativeDocument != IntPtr.Zero)
            {
                NativeMethods.drop_document(NativeContext, NativeDocument);
                NativeDocument = IntPtr.Zero;
            }

            if (NativeContext != IntPtr.Zero)
            {
                NativeMethods.drop_context(NativeContext);
                NativeContext = IntPtr.Zero;
            }

            return;
        }
    }
}
