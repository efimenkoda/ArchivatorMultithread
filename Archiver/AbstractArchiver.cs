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
        protected static int countThreads =  Environment.ProcessorCount;
        protected string InputFile { get; set; }
        protected string OutputFile { get; set; }
        protected BlockingCollection<Blocks> processingDataBlocks = new BlockingCollection<Blocks>(countThreads*10);
        protected BlockProcessingWriteCollection dataBlocksToWrite = new BlockProcessingWriteCollection();
        protected AutoResetEvent[] autoResetEvents = new AutoResetEvent[countThreads];
        public AbstractArchiver(string inputFile, string outputFile)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
    }
        protected abstract void StartReadFile();

        protected abstract void BlockProcessing(int threadsNumber);
        protected abstract void WriteToFile();
        public void GetProccess()
        {
            Thread[] processThread = new Thread[countThreads];

            Thread threadRead = new Thread(StartReadFile);
            threadRead.Start();
            
            
            for (int i = 0; i < processThread.Length; i++)
            {
                int j = i;
                autoResetEvents[j] = new AutoResetEvent(false);
                processThread[j] = new Thread(() => BlockProcessing(j));
                processThread[j].Start();
            }


            Thread threadWrite = new Thread(WriteToFile);
            threadWrite.Start();
            WaitHandle.WaitAll(autoResetEvents);
            dataBlocksToWrite.Completed();
            threadWrite.Join();
            
                       
        }

    }
}
