using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace mupdflibnet
{
    public class Pixmap : IDisposable
    {
        bool disposed = false;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Bitmap Bmp { get; private set; }

        public IntPtr PixPtr { get; private set; }
        public IntPtr PixDataPtr { get; private set; }
        public IntPtr CtxPtr { get; private set; }

        public Pixmap(IntPtr ctx, int width, int height)
        {
            CtxPtr = ctx;
            Width = width;
            Height = height;
            PixPtr = NativeMethods.NewPixmapARGB(CtxPtr, Width, Height);
            PixDataPtr = NativeMethods.GetPixmapData(CtxPtr, PixPtr);
            int stride = Width * 4;
            Bmp = new Bitmap(Width, Height, stride, PixelFormat.Format32bppPArgb, PixDataPtr);
        }

        public Bitmap Render(Page page, float scale, int rotation, int x0, int y0)
        {
            NativeMethods.RunDisplayList(CtxPtr, page.ListPtr, PixPtr, scale, rotation, x0, y0);
            return Bmp;
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
                //Free managed objects
                if (Bmp != null)
                    Bmp.Dispose();
            }

            //Free unmanaged resources
            NativeMethods.DropPixmap(CtxPtr, PixPtr);
            PixPtr = IntPtr.Zero;
            PixDataPtr = IntPtr.Zero;

            disposed = true;
        }

        ~Pixmap()
        {
            Dispose(false);
        }
    }
}
