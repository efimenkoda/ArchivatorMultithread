using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Archiver
{
    class Program
    {

        static void Main(string[] args)
        {
            AbstractArchiver archiver;
            
            string args0 = "compress";
            string args1 = @"d:\TEMP\1.doc";
            string args2 = @"d:\TEMP\1.doc.gz";
            //archiver = new Compress(args1, args2);

            args0 = "decompress";
            args1 = @"d:\TEMP\1.doc.gz";
            args2 = @"d:\TEMP\2.doc";
            archiver = new Decompress(args1, args2);

            //string args0 = "compress";
            //string args1 = @"d:\TEMP\office.iso";
            //string args2 = @"d:\TEMP\office.iso.gz";

            //args0 = "decompress";
            //args1 = @"d:\TEMP\office.iso.gz";
            //args2 = @"d:\TEMP\officeNew.iso";

            //string args0 = @"compress";
            //string args1 = @"d:\TEMP\1C_8.2_Education.iso";
            //string args2 = @"d:\TEMP\1C_8.2_Education.iso.gz";

            //args0 = "decompress";
            //args1 = @"d:\TEMP\1C_8.2_Education.gz";
            //args2 = @"d:\TEMP\1C_8.2_EducationNew.iso";

            //GZipFileManager gZipFile = new GZipFileManager(args0, args1, args2);


            archiver.GetProccess();

            Console.WriteLine("Main {0}", Thread.CurrentThread.ManagedThreadId);
            Console.ReadLine();
        }
    }


}
