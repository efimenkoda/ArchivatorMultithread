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

            ArchiverManager manager = new ArchiverManager(args);
            manager.Start();

            Console.ReadLine();
        }
    }


}
