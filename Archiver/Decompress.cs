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
                    using (FileStream destinationStream = File.Create(OutputFile))
                    {
                        FileInfo file = new FileInfo(InputFile);
                        var sizeFileInput = file.Length;
                        while (sourceStream.Position < sourceStream.Length)
                        {
                            processingDataBlocks.Clear();
                            Thread[] threadCompress = new Thread[countThreads];
                            for (int i = 0; i < countThreads && sizeFileInput > 0; i++)
                            {
                                int sizeCompressBlock = binaryReader.ReadInt32();
                                byte[] buffer = binaryReader.ReadBytes(sizeCompressBlock);
                                processingDataBlocks.Add(i, buffer);
                                sizeFileInput = sizeFileInput - (sizeCompressBlock + 4);
                                int j = i;
                                threadCompress[j] = new Thread(() => BlockProcessing(j));
                                threadCompress[j].Start();
                            }

                            WriteToFile(destinationStream, threadCompress);
                        }

                    }
                }
            }
        }



        protected override void BlockProcessing(int threadNumber)
        {
            try
            {
                processingDataBlocks.TryGetValue(threadNumber, out byte[] data);
                var outCompress = DecompressBlock(data);
                processingDataBlocks[threadNumber] = outCompress;
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

        private void WriteToFile(FileStream destinationStream, Thread[] threadCompress)
        {
            for (int i = 0; i < countThreads; i++)
            {
                threadCompress[i].Join();
                processingDataBlocks.TryGetValue(i, out byte[] block);
                destinationStream.Write(block, 0, block.Length);
            }
        }

    }
}
