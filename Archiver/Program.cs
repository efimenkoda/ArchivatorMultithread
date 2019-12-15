using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiver
{
    class Program
    {

        static void Main(string[] args)
        {

            //string args0= "compress";
            //string args1 = @"d:\TEMP\office.iso";
            //string args2 = @"d:\TEMP\office.gz";

            //args0 = "decompress";
            //args1 = @"d:\TEMP\office.gz";
            //args2 = @"d:\TEMP\officeNew.iso";

            string args0 = @"compress";
            string args1 = @"d:\TEMP\DatabaseVyrabotka.mdf";
            string args2 = @"d:\TEMP\DatabaseVyrabotka.gz";
            GZipFileManager gZipFile = new GZipFileManager(args0, args1, args2);

            args0 = "decompress";
            args1 = @"d:\TEMP\DatabaseVyrabotka.gz";
            args2 = @"d:\TEMP\DatabaseVyrabotkaNew.mdf";
            gZipFile = new GZipFileManager(args0, args1, args2);

            Console.ReadLine();
        }
    }


}
