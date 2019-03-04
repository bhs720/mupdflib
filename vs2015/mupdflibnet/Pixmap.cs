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

        public IntPtr NativePixmap { get; private set; }
        public IntPtr NativePixmapData { get; private set; }
        public IntPtr NativeContext { get; private set; }

        public Pixmap(IntPtr nativeContext, int width, int height)
        {
            this.NativeContext = nativeContext;
            this.Width = width;
            this.Height = height;

            NativePixmap = NativeMethods.new_pixmap_argb(NativeContext, Width, Height);
            if (NativePixmap == IntPtr.Zero)
                throw new NativeException("new_pixmap_argb() failed");

            NativePixmapData = NativeMethods.get_pixmap_data(nativeContext, NativePixmap, out width, out height);
            if (NativePixmapData == IntPtr.Zero)
                throw new NativeException("get_pixmap_data() failed");

            Bmp = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb, NativePixmapData);
        }

        public Bitmap Render(Page page, float scale, int rotation, int x0, int y0)
        {
            if (1 == NativeMethods.run_displaylist(NativeContext, page.ListPtr, NativePixmap, scale, rotation, x0, x0))
                throw new NativeException("run_displaylist() failed");

            return Bmp;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
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
            if (NativePixmap != IntPtr.Zero)
            {
                NativeMethods.drop_pixmap(NativeContext, NativePixmap);
                NativePixmap = IntPtr.Zero;
                NativePixmapData = IntPtr.Zero;
            }

            disposed = true;
        }

        ~Pixmap()
        {
            Dispose(false);
        }
    }
}
