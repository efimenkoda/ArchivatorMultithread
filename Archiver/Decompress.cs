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

        protected override void StartReadFile()
        {
            try
            {
                TryStartReadFile();
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка извлечения файла {0}: {1}", (new FileInfo(InputFile)).Name, e.Message);
                throw new Exception();
            }
        }

        private void TryStartReadFile()
        {
            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                using (var binaryReader = new BinaryReader(sourceStream))
                {
                    int i = 0;
                    FileInfo file = new FileInfo(InputFile);
                    var sizeFileInput = file.Length;
                    while (sourceStream.Position < sourceStream.Length)
                    {
                        int sizeCompressBlock = binaryReader.ReadInt32();
                        byte[] buffer = binaryReader.ReadBytes(sizeCompressBlock);
                        processingDataBlocks.Add(new Blocks(i, buffer));
                        //processingDataBlocks1.Enqueue(new Blocks(i, buffer));
                        sizeFileInput = sizeFileInput - (sizeCompressBlock + 4);

                        Console.WriteLine("Reading thead {0} block {1}", Thread.CurrentThread.ManagedThreadId, i++);
                    }
                    if (sourceStream.Position == sourceStream.Length)
                    {
                        processingDataBlocks.CompleteAdding();
                    }


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
                    
                    var outCompress = DecompressBlock(data.Block);
                    dataBlocksToWrite.Add(data.ID, outCompress);
                    Console.WriteLine("Processing thead {0} block {1}", Thread.CurrentThread.ManagedThreadId, data.ID);
                }
                autoResetEvents[threadNumber].Set();


            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка распаковки блока данных: {0}", e.Message);
            }
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
            try
            {
                using (FileStream destinationStream = File.Create(OutputFile))
                {
                    int i = 0;                    
                    while (dataBlocksToWrite.GetValue(out var data))
                    {

                            destinationStream.Write(data, 0, data.Length);
                            Console.WriteLine("Writting thead {0} block {1}", Thread.CurrentThread.ManagedThreadId, i++);
                        

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
