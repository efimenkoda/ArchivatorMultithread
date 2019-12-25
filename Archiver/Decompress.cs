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
        public Decompress(string inputFile, string outputFile) : base(inputFile, outputFile)
        {
        }

        protected override void ReadFromFile()
        {

            Console.WriteLine("Decompress...");
            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                using (var binaryReader = new BinaryReader(sourceStream))
                {
                    FileInfo file = new FileInfo(InputFile);
                    var sizeFileInput = file.Length;                    
                        for (int i = 0; sizeFileInput > 0; i++)
                        {
                            int sizeCompressBlock = binaryReader.ReadInt32();
                            byte[] buffer = binaryReader.ReadBytes(sizeCompressBlock);
                            readDataBlocks.Add(new Blocks(i, buffer));
                            //compressDataBlocksDictionary.TryAdd(i, buffer);
                            sizeFileInput = sizeFileInput - (sizeCompressBlock + 4);
                            Console.WriteLine("Чтение блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);

                    }
                    if (sizeFileInput == 0)
                    {
                        readDataBlocks.CompleteAdding();
                        readDataBlocks.OrderBy(p => p.Id);
                        
                    }
                }
            }

        }

        protected override void BlockProcessing(int threadsNumber)
        {
            //for (int i = 0; i < readDataBlocks.Count; i++)
            while(readDataBlocks.TryTake(out Blocks block))
            {
                //var block = readDataBlocks.Take();
                var outDecompress = DecompressBlock(block.byteBlock);
                compressDataBlocksDictionary.TryAdd(block.Id, outDecompress);
                //writeDataBlocks.Add(new Blocks(block.Id, outDecompress));
                Console.WriteLine(block.Id);
                Console.WriteLine("Обработка блока {0}, Процесс {1} ", block.Id, Thread.CurrentThread.ManagedThreadId);

            }
          
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
            Console.WriteLine("WriteToFile...");
            
            using (FileStream destinationStream = File.Create(OutputFile))
            {
                //while (writeDataBlocks.TryTake(out Blocks block))
                //{
                //    destinationStream.Write(block.byteBlock, 0, block.byteBlock.Length);
                //}

                var count = compressDataBlocksDictionary.Keys.Count();
                for (int i = 0; i < count; i++)
                {
                    while (compressDataBlocksDictionary.TryRemove(i, out byte[] data))
                    {
                        destinationStream.Write(data, 0, data.Length);
                        Console.WriteLine("Запись блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);
                    }
                }

                



            }
            //Console.WriteLine("WriteFile {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
