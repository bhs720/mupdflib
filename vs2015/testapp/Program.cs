using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security;
using System.Security.Permissions;
using mupdflibnet;

namespace testapp
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Initializing MuPDF context");
            IntPtr ctx = NativeMethods.new_context();
            if (ctx == IntPtr.Zero)
                throw new Exception("Failed to initialize MuPDF context");

            //CountPages(ctx);
            //AllocatePixmap(ctx);

            int pxWidth = 300;
            int pxHeight = 400;

            Console.WriteLine("Allocating pixmap");
            IntPtr pix = NativeMethods.new_pixmap_argb(ctx, pxWidth, pxHeight);
            if (pix == IntPtr.Zero)
                throw new Exception("Failed to allocate pixmap");

            int w, h;
            IntPtr pixData = NativeMethods.get_pixmap_data(ctx, pix, out w, out h);
            if (pixData == IntPtr.Zero)
                throw new Exception("Failed to get pixmap data");

            Console.WriteLine("Pixmap w={0} h={1}", w, h);

            

            string filename = @"C:\temp\ocr.pdf";
            Console.WriteLine("Opening file: {0}", filename);
            IntPtr doc = NativeMethods.open_document(ctx, filename, ".pdf");
            if (doc == IntPtr.Zero)
                throw new Exception("Failed to open file: " + filename);

            int pagecount = NativeMethods.get_pagecount(ctx, doc);
            if (pagecount < 1)
                throw new Exception("Pagecount is less than 1");

            Console.WriteLine("Pagecount={0}", pagecount);

            for (int i = 0; i < pagecount; i++)
            {
                IntPtr page = NativeMethods.load_page(ctx, doc, i);
                float width, height;
                NativeMethods.get_pagesize(ctx, page, out width, out height);
                Console.WriteLine("Page={0} Width={1:F2} Height={2:F2}", i + 1, width / 72, height / 72);

                IntPtr list = NativeMethods.load_displaylist(ctx, page);
                if (list == IntPtr.Zero)
                    throw new Exception("Failed to load display list");

                float scale = Math.Min(pxWidth / width, pxHeight / height);
                int x0 = 0;
                int y0 = 0;
                int rotation = 0;

                int rc = NativeMethods.run_displaylist(ctx, list, pix, scale, rotation, x0, y0);
                if (rc != 0)
                    throw new Exception("Failed to run display list");

                var bmp = new Bitmap(pxWidth, pxHeight, pxWidth * 4, PixelFormat.Format32bppPArgb, pixData);
                
                var outFilename = string.Format(@"C:\temp\out\{0:D4}.tif", i+1);
                bmp.Save(outFilename, ImageFormat.Tiff);
                bmp.Dispose();

                NativeMethods.drop_displaylist(ctx, list);
                NativeMethods.drop_page(ctx, page);
            }

            Console.WriteLine("Dropping pixmap");
            NativeMethods.drop_pixmap(ctx, pix);

            Console.WriteLine("Closing PDF document");
            NativeMethods.drop_document(ctx, doc);

            Console.WriteLine("Dropping pixmap");
            NativeMethods.drop_pixmap(ctx, pix);

            Console.WriteLine("Dropping MuPDF context");
            NativeMethods.drop_context(ctx);

            Console.WriteLine("Program finished");
            Console.ReadKey();
        }

        private static void AllocatePixmap(IntPtr ctx, int width, int height)
        {
            
        }

        private static void CountPages(IntPtr ctx)
        {
            
        }
    }
}
