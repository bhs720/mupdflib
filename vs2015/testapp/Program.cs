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

            var rects = NativeMethods.SearchForText(ctx, list, "Addendum");

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

            IntPtr doc = NativeMethods.open_document(ctx, @"C:\temp\corrupt.pdf", ".pdf");
            IntPtr page = NativeMethods.load_page(ctx, doc, 0);
            float ptWidth, ptHeight;
            NativeMethods.get_pagesize(ctx, page, out ptWidth, out ptHeight);
            IntPtr list = NativeMethods.load_displaylist(ctx, page);
            int pxWidth = 850;
            int pxHeight = 1100;
            IntPtr pix = NativeMethods.new_pixmap_argb(ctx, 850, 1100);
            IntPtr scan0 = NativeMethods.get_pixmap_data(ctx, pix, out pxWidth, out pxHeight);
            float scale = Math.Min(pxWidth / ptWidth, pxHeight / ptHeight);
            int rc = NativeMethods.run_displaylist(ctx, list, pix, scale, 0, 0, 0);

            Bitmap bmp = new Bitmap(pxWidth, pxHeight, pxWidth * 4, PixelFormat.Format32bppPArgb, scan0);
            bmp.Save(@"C:\temp\out\1.tif");

            NativeMethods.drop_pixmap(ctx, pix);
            NativeMethods.drop_displaylist(ctx, list);
            NativeMethods.drop_page(ctx, page);
            NativeMethods.drop_context(ctx);
        }
    }
}
