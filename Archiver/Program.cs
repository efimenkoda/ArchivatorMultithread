using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace Archiver
{
    class Program
    {

        static void Main(string[] args)
        {
            

            //compress d:\TEMP\1.doc d:\TEMP\1.doc
            //decompress d:\TEMP\1.doc.gz d:\TEMP\1New.doc

            //compress d:\TEMP\office.iso d:\TEMP\office.iso
            //decompress d:\TEMP\office.iso.gz d:\TEMP\officeNew.iso

            //compress d:\TEMP\1C_8.2_Education.iso d:\TEMP\1C_8.2_Education.iso
            //decompress d:\TEMP\1C_8.2_Education.iso.gz d:\TEMP\1C_8.2_EducationNew.iso


            ArchiverManager manager = new ArchiverManager(args);
            manager.Start();

            Console.ReadLine();
        }
    }


}
