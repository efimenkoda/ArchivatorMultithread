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
                using (FileStream destinationStream = File.Create(OutputFile + ".gz"))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(destinationStream))
                    {
                        while (sourceStream.Position < sourceStream.Length)
                        {
                            Thread[] threadCompress = new Thread[countThreads];
                            int blockLength = 0;
                            processingDataBlocks.Clear();
                            for (int i = 0; i < countThreads; i++)
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
                                processingDataBlocks.TryAdd(i, buffer);
                                int j = i;
                                threadCompress[j] = new Thread(() => BlockProcessing(j));
                                threadCompress[j].Start();
                            }

                            WriteToFile(binaryWriter, threadCompress);
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
                var outCompress = CompressBlock(data);
                processingDataBlocks[threadNumber] = outCompress;
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

        private void WriteToFile(BinaryWriter binaryWriter, Thread[] threadCompress)
        {

            for (int i = 0; i < countThreads; i++)
            {
                threadCompress[i].Join();
                processingDataBlocks.TryGetValue(i, out byte[] block);
                binaryWriter.Write(block.Length);
                binaryWriter.Write(block, 0, block.Length);
            }


        }
    }
}
