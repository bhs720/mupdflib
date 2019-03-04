using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace mupdflibnet
{
    public class Document : IDisposable
    {
        bool disposed = false;

        public string Filename { get; private set; }
        public int PageCount { get; private set; }

        public IntPtr CtxPtr { get; private set; }
        public IntPtr DocPtr { get; private set; }

        public Document(string filename)
        {
            Filename = filename;
            CtxPtr = NativeMethods.NewContext();
            DocPtr = NativeMethods.OpenDocument(CtxPtr, filename);
            PageCount = NativeMethods.GetPageCount(CtxPtr, DocPtr);
        }

        /// <summary>
        /// Load a page by page number. Remember to call <see cref="Page.Dispose"/> when done
        /// </summary>
        /// <param name="pageNumber">Zero-based page number</param>
        /// <returns></returns>
        public Page LoadPage(int pageNumber)
        {
            return new Page(CtxPtr, DocPtr, pageNumber);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free managed objects
            }

            // Free unmanaged objects

            NativeMethods.DropDocument(CtxPtr, DocPtr);
            DocPtr = IntPtr.Zero;
            NativeMethods.drop_context(CtxPtr);
            CtxPtr = IntPtr.Zero;

            disposed = true;
        }

        ~Document()
        {
            Dispose(false);
        }
    }
}
