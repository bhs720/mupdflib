using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testapp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing MuPDF context");
            IntPtr ctx = MuPDFNativeMethods.new_context();
            if (ctx == IntPtr.Zero)
                throw new Exception("Failed to initialize MuPDF context");

            string filename = @"C:\temp\corrupt.pdf";
            Console.WriteLine("Opening file: {0}", filename);
            IntPtr doc = MuPDFNativeMethods.open_document(ctx, filename, ".pdf");
            if (doc == IntPtr.Zero)
                throw new Exception("Failed to open file: " + filename);

            int pagecount = MuPDFNativeMethods.get_pagecount(ctx, doc);
            if (pagecount < 1)
                throw new Exception("Pagecount is less than 1");

            Console.WriteLine("Pagecount={0}", pagecount);

            for (int i = 0; i < pagecount; i++)
            {
                IntPtr page = MuPDFNativeMethods.load_page(ctx, doc, i);
                float width, height;
                MuPDFNativeMethods.get_pagesize(ctx, page, out width, out height);
                Console.WriteLine("Page={0} Width={1:F2} Height={2:F2}", i + 1, width / 72, height / 72);
                MuPDFNativeMethods.drop_page(ctx, page);
            }

            Console.WriteLine("Closing PDF document");
            MuPDFNativeMethods.drop_document(ctx, doc);
            Console.WriteLine("Dropping MuPDF context");
            MuPDFNativeMethods.drop_context(ctx);

            Console.WriteLine("Program finished");
            Console.ReadKey();
        }
    }
}
