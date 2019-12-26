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
        protected string InputFile { get; set; }
        protected string OutputFile { get; set; }
        protected Dictionary<int, byte[]> compressDataBlocksDictionary;
        protected Dictionary<int,byte[]> readDataBlocks;
        protected AutoResetEvent[] eventCountActiveProcess = new AutoResetEvent[countThreads];
        public AbstractArchiver(string inputFile, string outputFile)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
            FileInfo fileSize = new FileInfo(inputFile);
            var capasity = ((fileSize.Length) / blockSize)+1;
            compressDataBlocksDictionary = new Dictionary<int, byte[]>((int)capasity);
             readDataBlocks = new Dictionary<int,byte[]>((int)capasity);
    }
        protected abstract void ReadFromFile();
        protected abstract void BlockProcessing(int threadsNumber);
        protected abstract void WriteToFile();
        public void GetProccess()
        {
            ////GC.AddMemoryPressure(InputFile.Length);
            //Thread threadReadFile = new Thread(()=> { ReadFromFile(); WriteToFile(); });
            //threadReadFile.Start();
            ////threadReadFile.Join(3000);
            //List<Thread> threadCompressBlocks = new List<Thread>();
            //Thread threadWriteFile = new Thread(WriteToFile);

            //for (int i = 0; i < countThreads; i++)
            //{
            //    int j = i;
            //    eventCountActiveProcess[j] = new AutoResetEvent(false);
            //    threadCompressBlocks.Add(new Thread(() => BlockProcessing(j)));
            //}

            //foreach (var threads in threadCompressBlocks)
            //{
            //    threads.Start();
            //}

            //WaitHandle.WaitAll(eventCountActiveProcess);
            ////threadWriteFile.Start();
            ////threadWriteFile.Join();
            
           ReadFromFile();
            
        }

    }
}
