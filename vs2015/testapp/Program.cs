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
using System.Runtime.InteropServices;

namespace testapp
{
    class Program
    {
        static void Main(string[] args)
        {
            Main4();

            Console.WriteLine("Program finished");
            Console.ReadKey();
        }

        static void Main4()
        {
            IntPtr ctx = NativeMethods.NewContext();

            IntPtr doc = NativeMethods.OpenDocument(ctx, @"C:\temp\plans.pdf");
            IntPtr page = NativeMethods.LoadPage(ctx, doc, 8);
            SizeF pageSize = NativeMethods.GetPageSize(ctx, page);
            IntPtr list = NativeMethods.LoadDisplayList(ctx, page);

            string text = NativeMethods.GetPagePlainText(ctx, list);
            Console.WriteLine(text);
            Console.WriteLine();

            NativeMethods.WritePagePlainTextToFile(ctx, page, @"C:\temp\page.txt");

            var rects = NativeMethods.SearchForText(ctx, list, "31.02");

            NativeMethods.DropDisplayList(ctx, list);
            NativeMethods.DropPage(ctx, page);
            NativeMethods.DropContext(ctx);
        }

        static void Main1()
        {
            using (var doc = new Document(@"C:\temp\corrupt.pdf"))
            using (var pix = new Pixmap(doc.CtxPtr, 850, 1100))
            {
                for (int i = 0; i < doc.PageCount; i++)
                {
                    using (var page = doc.LoadPage(i))
                    {
                        float scale = Math.Min(pix.Width / page.Width, pix.Height / page.Height);
                        Bitmap bmp = pix.Render(page, scale, 90, 0, 0);
                        string filename = string.Format(@"C:\temp\out\{0}.tif", i + 1);
                        bmp.Save(filename);
                        bmp.Dispose();
                        Console.WriteLine(page.ToString());
                    }
                }
            }
        }

        static void Main3()
        {
            using (var doc = new Document(@"C:\temp\corrupt.pdf"))
            using (var pix = new Pixmap(doc.CtxPtr, 850, 1100))
            {
                for (int i = 0; i < doc.PageCount; i++)
                {
                    using (var page = doc.LoadPage(i))
                    {
                        Bitmap bmp = page.Render32(85, 110);
                        string filename = string.Format(@"C:\temp\out\{0}.tif", i + 1);
                        //bmp.Save(filename);
                        bmp.Dispose();
                        Console.WriteLine(page.ToString());
                    }
                }
            }
        }

        static void Main2()
        {
            IntPtr ctx = NativeMethods.NewContext();

            IntPtr doc = NativeMethods.OpenDocument(ctx, @"C:\temp\corrupt.pdf");
            IntPtr page = NativeMethods.LoadPage(ctx, doc, 0);
            var ptSize = NativeMethods.GetPageSize(ctx, page);
            IntPtr list = NativeMethods.LoadDisplayList(ctx, page);
            int pxWidth = 850;
            int pxHeight = 1100;
            IntPtr pix = NativeMethods.NewPixmapARGB(ctx, pxWidth, pxHeight);
            IntPtr scan0 = NativeMethods.GetPixmapData(ctx, pix);
            float scale = Math.Min(pxWidth / ptSize.Width, pxHeight / ptSize.Height);
            NativeMethods.RunDisplayList(ctx, list, pix, scale, 0, 0, 0);

            Bitmap bmp = new Bitmap(pxWidth, pxHeight, pxWidth * 4, PixelFormat.Format32bppPArgb, scan0);
            bmp.Save(@"C:\temp\out\1.tif");

            NativeMethods.DropPixmap(ctx, pix);
            NativeMethods.DropDisplayList(ctx, list);
            NativeMethods.DropPage(ctx, page);
            NativeMethods.DropContext(ctx);
        }
    }
}
