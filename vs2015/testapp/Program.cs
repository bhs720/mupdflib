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
            using (var doc = new PDFDocument(@"C:\temp\corrupt.pdf"))
            {
                for (int i = 0; i < doc.PageCount; i++)
                {
                    using (var page = doc.LoadPage(i))
                    {
                        Console.WriteLine(page.ToString());
                    }
                }
            }

            Console.WriteLine("Program finished");
            Console.ReadKey();
        }
    }
}
