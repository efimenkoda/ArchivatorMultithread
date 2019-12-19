using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Archiver
{
    public abstract class AbstractArchiver
    {
        protected static int threadNumber = Environment.ProcessorCount;
        protected static int blockSize = 1024 * 1024;

        private static int countThread = Environment.ProcessorCount;

        private static object locker = new object();
        protected string InputFile { get; set; }
        protected string OutputFile { get; set; }

        protected Dictionary<int, byte[]> DataBlocksDictionary = new Dictionary<int, byte[]>();
        //protected Dictionary<int, byte[]> outputDictionary = new Dictionary<int, byte[]>();

        AutoResetEvent[] blockProcces = new AutoResetEvent[countThread];

        public AbstractArchiver(string inputFile, string outputFile)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
        }

        protected abstract void ReadFromFile();
        protected abstract void BlockOperation();
        protected abstract void WriteToFile();
        public void GetProccess()
        {
            //var threadRead = new Thread(ReadFromFile);
            //List<Thread> threadCompressBlocks = new List<Thread>();
            //for (int i = 0; i < countThread; i++)
            //{
            //    int j = i;
            //    blockProcces[j] = new AutoResetEvent(false);
            //    threadCompressBlocks.Add(new Thread(()=> { }));
            //}
            //var threadWrite = new Thread(WriteToFile);
            ReadFromFile();
            BlockOperation();
            WriteToFile();





        }

    }
}
