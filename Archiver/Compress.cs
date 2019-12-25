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
            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                int blockLength = 0;
                for (int i = 0; sourceStream.Position < sourceStream.Length; i++)
                {
                    if (sourceStream.Length - sourceStream.Position > blockSize)
                    {
                        blockLength = blockSize;
                    }
                    else
                    {
                        blockLength = (int)(sourceStream.Length - sourceStream.Position);
                    }
                    byte[] buffer = new byte[blockLength];
                    sourceStream.Read(buffer, 0, blockLength);
                    readDataBlocks.TryAdd(new Blocks(i, buffer));
                    Console.WriteLine("Чтение блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);

                }
                
                if (sourceStream.Position == sourceStream.Length)
                {
                    readDataBlocks.CompleteAdding();                    

                }

            }
            
        }
        protected override void BlockProcessing(int threadNumber)
        {  

            while(readDataBlocks.TryTake(out Blocks block))
            {                
                //readDataBlocks.TryTake(out Blocks block);
                var outCompress = CompressBlock(block.byteBlock);
                compressDataBlocksDictionary.TryAdd(block.Id, outCompress);
               
                //writeDataBlocks.Add(new Blocks(block.Id, outCompress));
                Console.WriteLine("Обработка блока {0}, Процесс {1} ",block.Id,Thread.CurrentThread.ManagedThreadId);
            }
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
            Console.WriteLine("WriteToFile..."); 
            using (FileStream destinationStream = File.Create(OutputFile+".gz"))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(destinationStream))
                {

                    var count = compressDataBlocksDictionary.Keys.Count();
                    for (int i = 0; i < count; i++)
                    {
                        while (compressDataBlocksDictionary.TryRemove(i, out byte[] data))
                        {
                            binaryWriter.Write(data.Length);
                            binaryWriter.Write(data, 0, data.Length);
                            Console.WriteLine("Запись блока {0}, Процесс {1} ", i, Thread.CurrentThread.ManagedThreadId);
                        }
                    }
                    
                }
            }
            
            //Console.WriteLine("WriteFile {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
