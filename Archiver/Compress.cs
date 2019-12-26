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
    public class Compress : AbstractArchiver
    {
        public Compress(string inputFile, string outputFile) : base(inputFile, outputFile)
        {
        }


        protected override void ReadFromFile()
        {
            Console.WriteLine("Compress...");
            //using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            //{
            //    int blockLength = 0;
            //    for (int i = 0; sourceStream.Position < sourceStream.Length; i++)
            //    {
            //        if (sourceStream.Length - sourceStream.Position > blockSize)
            //        {
            //            blockLength = blockSize;
            //        }
            //        else
            //        {
            //            blockLength = (int)(sourceStream.Length - sourceStream.Position);
            //        }
            //        byte[] buffer = new byte[blockLength];
            //        sourceStream.Read(buffer, 0, blockLength);
            //        readDataBlocks.TryAdd(new Blocks(i, buffer));

            //        Console.WriteLine("Чтение блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);
            //    }

            //    if (sourceStream.Position == sourceStream.Length)
            //    {
            //        readDataBlocks.CompleteAdding();                    

            //    }

            //}

            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream destinationStream = File.Create(OutputFile + ".gz"))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(destinationStream))
                    {
                        while (sourceStream.Position < sourceStream.Length)
                        {
                            Thread[] threadCompress = new Thread[countThreads];
                            int blockLength = 0;
                            compressDataBlocksDictionary.Clear();
                            readDataBlocks.Clear();
                            for (int i = 0; i < countThreads && sourceStream.Position < sourceStream.Length; i++)
                            {
                                if (sourceStream.Length - sourceStream.Position > blockSize)
                                {
                                    blockLength = blockSize;
                                }
                                else
                                {
                                    blockLength = (int)(sourceStream.Length - sourceStream.Position);
                                }
                                Console.WriteLine("Чтение блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);
                                byte[] buffer = new byte[blockLength];
                                sourceStream.Read(buffer, 0, blockLength);
                                readDataBlocks.Add(i, buffer);
                                
                                int j = i;
                                eventCountActiveProcess[j] = new AutoResetEvent(false);
                                threadCompress[j] = new Thread(() => BlockProcessing(j));
                                threadCompress[j].Start();
                            }


                            //WaitHandle.WaitAll(eventCountActiveProcess);
                            for (int i = 0; i < countThreads && (threadCompress[i] != null); i++)
                            {
                                threadCompress[i].Join();
                                Console.WriteLine("Запись блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);
                                binaryWriter.Write(compressDataBlocksDictionary[i].Length);
                                binaryWriter.Write(compressDataBlocksDictionary[i], 0, compressDataBlocksDictionary[i].Length);
                            }
                        }

                    }

                }

            }
        }


        protected override void BlockProcessing(int threadNumber)
        {
                Console.WriteLine("Обработка блока {0}, Процесс {1} ", threadNumber, Thread.CurrentThread.ManagedThreadId);
                var outCompress = CompressBlock(readDataBlocks[threadNumber]);
                compressDataBlocksDictionary[threadNumber]=outCompress;
                eventCountActiveProcess[threadNumber].Set();
        }


        private byte[] CompressBlock(byte[] dataBlock)
        {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (GZipStream compressionStream = new GZipStream(outputStream, CompressionMode.Compress))
                    {
                        compressionStream.Write(dataBlock, 0, dataBlock.Length);
                    }
                    return outputStream.ToArray();
                }
        }
        
        protected override void WriteToFile()
        {
            //Console.WriteLine("WriteToFile...");
            //using (FileStream destinationStream = File.Create(OutputFile+".gz"))
            //{
            //    using (BinaryWriter binaryWriter = new BinaryWriter(destinationStream))
            //    {
            //        for (int i = 0; i < compressDataBlocksDictionary.Keys.Count(); i++)
            //        {
            //            while (compressDataBlocksDictionary.TryGetValue(i, out byte[] data))
            //            {
            //                binaryWriter.Write(data.Length);
            //                binaryWriter.Write(data, 0, data.Length);
            //                Console.WriteLine("Запись блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);
            //            }
            //        }

                    
            //    }
            //}
            
        }
    }
}
