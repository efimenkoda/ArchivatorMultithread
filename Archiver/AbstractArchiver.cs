using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Archiver
{
    public abstract class AbstractArchiver
    {
        protected static int blockSize = 1024 * 1024;
        protected static int countThreads = Environment.ProcessorCount;
        protected static object locker = new object();
        protected string InputFile { get; set; }
        protected string OutputFile { get; set; }
        protected Dictionary<int, byte[]> compressDataBlocksDictionary= new Dictionary<int, byte[]>();
        protected Dictionary<int,byte[]> readDataBlocks= new Dictionary<int, byte[]>();        
        public AbstractArchiver(string inputFile, string outputFile)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
    }
        protected abstract void StartReadFile();
        protected abstract void BlockProcessing(int threadsNumber);
        //protected abstract void WriteToFile();
        public void GetProccess()
        {         
           StartReadFile();            
        }

    }
}
