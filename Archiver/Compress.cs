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

        protected override void StartReadFile()
        {
            try
            {
                TryStartReadFile();

            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка сжатия файла {0}: {1}", (new FileInfo(InputFile)).Name, e.Message);
                throw new Exception();

            }
        }

        private void TryStartReadFile()
        {
            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                int i = 0;
                while (sourceStream.Position < sourceStream.Length)
                {
                    int blockLength = 0;

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
                    processingDataBlocks.Add(new Blocks(i, buffer));
                    //processingDataBlocks1.Enqueue(new Blocks(i, buffer));

                    Console.WriteLine("Reading thead {0} block {1}", Thread.CurrentThread.ManagedThreadId, i++);
                }
                if (sourceStream.Position == sourceStream.Length)
                {
                    processingDataBlocks.CompleteAdding();

                }
                

            }
        }


        protected override void BlockProcessing(int threadNumber)
        {
            try
            {
                //while (processingDataBlocks1.TryDequeue(out Blocks data))
                foreach (var data in processingDataBlocks.GetConsumingEnumerable())
                {
                    var outCompress = CompressBlock(data.Block);
                    dataBlocksToWrite.Add(data.ID, outCompress);

                    Console.WriteLine("Processing thead {0} block {1}", Thread.CurrentThread.ManagedThreadId, data.ID);
                }

                autoResetEvents[threadNumber].Set();

            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка сжатия блока данных: {0}", e.Message);
            }
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
            try
            {
                using (FileStream destinationStream = File.Create(OutputFile + ".gz"))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(destinationStream))
                    {
                        int i = 0;
                        while (dataBlocksToWrite.GetValue(out var data))
                        {
                            binaryWriter.Write(data.Length);
                            binaryWriter.Write(data, 0, data.Length);
                            Console.WriteLine("Writting thead {0} block {1}", Thread.CurrentThread.ManagedThreadId, i++);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка записи в файл: {0}", e.Message);
            }


        }
    }
}
