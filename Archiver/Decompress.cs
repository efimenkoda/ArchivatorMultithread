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
    public class Decompress : AbstractArchiver
    {

        byte[][] dataBlock = new byte[countThreads][];
        private byte[][] compressdData = new byte[countThreads][];
        public Decompress(string inputFile, string outputFile) : base(inputFile, outputFile)
        {
        }


        protected override void ReadFromFile()
        {

            Console.WriteLine("Decompress...");
            //using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            //{
            //    using (var binaryReader = new BinaryReader(sourceStream))
            //    {
            //        FileInfo file = new FileInfo(InputFile);
            //        var sizeFileInput = file.Length;                    
            //            for (int i = 0; sizeFileInput > 0; i++)
            //            {
            //                int sizeCompressBlock = binaryReader.ReadInt32();
            //                byte[] buffer = binaryReader.ReadBytes(sizeCompressBlock);
            //                readDataBlocks.Add(new Blocks(i, buffer));
            //                //compressDataBlocksDictionary.TryAdd(i, buffer);
            //                sizeFileInput = sizeFileInput - (sizeCompressBlock + 4);
            //                Console.WriteLine("Чтение блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);

            //        }
            //        if (sizeFileInput == 0)
            //        {
            //            readDataBlocks.CompleteAdding();
            //            readDataBlocks.OrderBy(p => p.Id);

            //        }
            //    }
            //}



            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                using (var binaryReader = new BinaryReader(sourceStream))
                {
                    using (FileStream destinationStream = File.Create(OutputFile))
                    {
                        FileInfo file = new FileInfo(InputFile);
                        var sizeFileInput = file.Length;
                        while (sourceStream.Position < sourceStream.Length)
                        {
                            compressDataBlocksDictionary.Clear();
                            readDataBlocks.Clear();
                            Thread[] threadCompress = new Thread[countThreads];
                            for (int i = 0; i < countThreads && sizeFileInput > 0; i++)
                            {
                                Console.WriteLine("Чтение блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);
                                int sizeCompressBlock = binaryReader.ReadInt32();
                                byte[] buffer = binaryReader.ReadBytes(sizeCompressBlock);
                                readDataBlocks.Add(i, buffer);
                                sizeFileInput = sizeFileInput - (sizeCompressBlock + 4);
                                

                                int j = i;
                                eventCountActiveProcess[j] = new AutoResetEvent(false);
                                threadCompress[j] = new Thread(() => BlockProcessing(j));
                                threadCompress[j].Start();


                            }
                            WaitHandle.WaitAll(eventCountActiveProcess);
                            for (int i = 0; i < countThreads && threadCompress[i] != null; i++)
                            {
                                threadCompress[i].Join();
                                destinationStream.Write(compressDataBlocksDictionary[i], 0, compressDataBlocksDictionary[i].Length);
                            }
                        }

                    }
                }

            }
        }

        protected override void BlockProcessing(int threadsNumber)
        {
            
                Console.WriteLine("Обработка блока {0}, Процесс {1} ", threadsNumber, Thread.CurrentThread.ManagedThreadId);
                var outDecompress = DecompressBlock(readDataBlocks[threadsNumber]);
                compressDataBlocksDictionary[threadsNumber] =outDecompress;
            
            eventCountActiveProcess[threadsNumber].Set();
        }


        private byte[] DecompressBlock(byte[] dataBlock)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (MemoryStream inputStream = new MemoryStream(dataBlock))
                {
                    using (GZipStream decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        byte[] buffer = new byte[dataBlock.Length];
                        int read;
                        while ((read = decompressionStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, read);
                        }
                        return outputStream.ToArray();
                    }

                }

            }
        }

        protected override void WriteToFile()
        {
            //Console.WriteLine("WriteToFile...");

            //using (FileStream destinationStream = File.Create(OutputFile))
            //{
            //    var count = compressDataBlocksDictionary.Keys.Count();
            //    //for (int i = 0; i < count; i++)
            //    foreach(KeyValuePair<int,byte[]> item in compressDataBlocksDictionary)
            //    {
            //        //while (compressDataBlocksDictionary.TryRemove(i, out byte[] data))
            //        {
            //            //destinationStream.Write(data, 0, data.Length);
            //            destinationStream.Write(item.Value, 0, item.Value.Length);
            //            Console.WriteLine("Запись блока {0}, Процесс {1} ", item.Key, Thread.CurrentThread.ManagedThreadId);
            //        }
            //    }





            //}
            //Console.WriteLine("WriteFile {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
