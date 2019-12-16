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

    public class GZipFileManager
    {
        static int threadNumber = Environment.ProcessorCount;
        static int blockSize = 1024 * 1024;
        static byte[][] dataArray = new byte[threadNumber][];

        //static int dataArraySize = dataBlockSize * threadNumber;


        private string operation;
        private string sourceFile;
        private string destinationFile;
        public GZipFileManager(string operation, string sourceFile, string destinationFile)
        {
            this.operation = operation;
            this.sourceFile = sourceFile;
            this.destinationFile = destinationFile;

            CompresDecompress(sourceFile, destinationFile);
        }

        private void CompresDecompress(string sourceFile, string destinationFile)
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                using (FileStream targetStream = File.Create(destinationFile))
                {
                    if (operation.Equals("compress"))
                    {
                        CreateGzip(sourceStream, targetStream);

                    }
                    else if (operation.Equals("decompress"))
                    {
                        ExtractFile(sourceStream, targetStream);

                    }
                }
            }
        }
        static object locker = new object();
        private void CreateGzip(FileStream sourceStream, FileStream destinationStream)
        {

            Thread[] pool;
            using (GZipStream compressionStream = new GZipStream(destinationStream, CompressionMode.Compress))
            {
                Console.WriteLine("Compress...");

                while (sourceStream.Position < sourceStream.Length)
                {

                    pool = new Thread[threadNumber];
                    for (int i = 0; i < threadNumber; i++)
                    {

                        dataArray[i] = new byte[blockSize];
                        sourceStream.Read(dataArray[i], 0, blockSize);

                        pool[i] = new Thread(() =>
                        {
                            lock (locker)
                            {
                                compressionStream.Write(dataArray[i], 0, blockSize);
                            }
                        });
                        pool[i].Start();
                        Console.WriteLine(pool[i].ManagedThreadId);
                        pool[i].Join();

                    }

                }
            }

        }

        private void ExtractFile(FileStream sourceStream, FileStream destinationStream)
        {
            Thread[] pool;
            using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            {

                while (decompressionStream.BaseStream.Position < decompressionStream.BaseStream.Length)
                {
                    pool = new Thread[threadNumber];
                    for (int i = 0; i < threadNumber && decompressionStream.BaseStream.Position < decompressionStream.BaseStream.Length; i++)
                    {
                        dataArray[i] = new byte[blockSize];
                        decompressionStream.Read(dataArray[i], 0, blockSize);

                        pool[i] = new Thread(() =>
                          {
                              lock (locker)
                              {

                                  destinationStream.Write(dataArray[i], 0, blockSize);
                              }

                          });
                        pool[i].Start();
                        Console.WriteLine(pool[i].ManagedThreadId);
                        pool[i].Join();
                    }
                }
            }

        }






    }
}
