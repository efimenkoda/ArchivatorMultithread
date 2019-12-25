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
        protected ConcurrentDictionary<int, byte[]> compressDataBlocksDictionary;
        //protected ProducerConsumer compressDataBlocksDictionary;
        protected BlockingCollection<Blocks> readDataBlocks;
        //protected BlockingCollection<Blocks> writeDataBlocks = new BlockingCollection<Blocks>();

        //protected Queue<BlockingCollection> CompressDataBlocksDictionary = new System.Collections.Generic.Queue<BlockingCollection>();
        protected AutoResetEvent[] eventCountActiveProcess = new AutoResetEvent[countThreads];
        protected static AutoResetEvent eventReading = new AutoResetEvent(false);
        protected static AutoResetEvent eventWriting = new AutoResetEvent(false);
        public AbstractArchiver(string inputFile, string outputFile)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
            FileInfo fileSize = new FileInfo(inputFile);
            var capasity = ((fileSize.Length) / blockSize)+1;
            compressDataBlocksDictionary = new ConcurrentDictionary<int, byte[]>(countThreads, (int)capasity);
             readDataBlocks = new BlockingCollection<Blocks>((int)capasity);
        //compressDataBlocksDictionary = new ProducerConsumer();
    }
        protected abstract void ReadFromFile();
        protected abstract void BlockProcessing(int threadsNumber);
        protected abstract void WriteToFile();
        public void GetProccess()
        {
            GC.AddMemoryPressure(InputFile.Length);      
            Thread threadReadFile = new Thread(ReadFromFile);
            threadReadFile.Start();
            threadReadFile.Join(3000);
            List<Thread> threadCompressBlocks = new List<Thread>();
            Thread threadWriteFile = new Thread(WriteToFile);

            for (int i = 0; i < countThreads; i++)
            {
                int j = i;
                eventCountActiveProcess[j] = new AutoResetEvent(false);           
                threadCompressBlocks.Add(new Thread(() => BlockProcessing(j)));
            }

            foreach (var threads in threadCompressBlocks)
            {
                threads.Start();
            }            
            
            WaitHandle.WaitAll(eventCountActiveProcess);
            threadWriteFile.Start();
            threadWriteFile.Join();

        }

    }
}
