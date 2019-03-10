using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace mupdflibnet
{
    public class Page : IDisposable
    {
        bool disposed = false;
        IntPtr _listPtr;
        string _pagePlainText;

        public int PageNumber { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public string PagePlainText
        {
            get
            {
                if (_pagePlainText == null)
                    _pagePlainText = NativeMethods.GetPagePlainText(CtxPtr, ListPtr);
                return _pagePlainText;
            }
        }

        public IntPtr CtxPtr { get; private set; }
        public IntPtr DocPtr { get; private set; }
        public IntPtr PagePtr { get; private set; }
        public IntPtr ListPtr
        {
            get
            {
                if (_listPtr == IntPtr.Zero)
                    _listPtr = NativeMethods.LoadDisplayList(CtxPtr, PagePtr);
                return _listPtr;
            }
        }

        public Page(IntPtr ctx, IntPtr doc, int pageNumber)
        {
            CtxPtr = ctx;
            DocPtr = doc;
            PageNumber = pageNumber;
            PagePtr = NativeMethods.LoadPage(CtxPtr, DocPtr, pageNumber);
            SizeF size = NativeMethods.GetPageSize(CtxPtr, PagePtr);
            Width = size.Width;
            Height = size.Height;

            Debug.Print(this.ToString());
        }

        public Bitmap Render32(Pixmap pix, float scale, int rotation, int xoff, int yoff)
        {
            NativeMethods.RunDisplayList(CtxPtr, ListPtr, pix.PixPtr, scale, rotation, xoff, yoff);
            return pix.Bmp;
        }

        public unsafe Bitmap Render32(int destWidth, int destHeight)
        {
            Bitmap bmp = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppPArgb);
            BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

            IntPtr pix = NativeMethods.NewPixmapARGB(CtxPtr, destWidth, destHeight);
            IntPtr pixData = NativeMethods.GetPixmapData(CtxPtr, pix);
            float scale = Math.Min(destWidth / Width, destHeight / Height);
            NativeMethods.RunDisplayList(CtxPtr, ListPtr, pix, scale, 0, 0, 0);

            ulong length = (ulong)(destWidth * destHeight * 4);
            Buffer.MemoryCopy(pixData.ToPointer(), bmd.Scan0.ToPointer(), length, length);

            NativeMethods.DropPixmap(CtxPtr, pix);
            bmp.UnlockBits(bmd);
            return bmp;
        }

        public override string ToString()
        {
            return string.Format("Page={0} Width={1:F2} Height={2:F2}", PageNumber + 1, Width, Height);
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
            NativeMethods.DropDisplayList(CtxPtr, ListPtr);
            _listPtr = IntPtr.Zero;
            NativeMethods.DropPage(CtxPtr, PagePtr);
            PagePtr = IntPtr.Zero;

            disposed = true;
        }

        ~Page()
        {
            Dispose(false);
        }
    }
}
