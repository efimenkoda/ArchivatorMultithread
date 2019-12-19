using System;
using System.Collections.Concurrent;
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

        Dictionary<int, byte[]> inputDictionary = new Dictionary<int, byte[]>();
        Dictionary<int, byte[]> compressDictionary = new Dictionary<int, byte[]>();
        private void ReadFileInput(string sourceFile)
        {            
            int blockLength = 0;
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            { 
                Console.WriteLine("Compress...");
                {
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
                        inputDictionary.Add(i, buffer);
                    }
                }
                }
            Console.WriteLine("ReadFile {0}", Thread.CurrentThread.ManagedThreadId);
            
        }

        private byte[] Compress(byte[] dataBlock)
        {                   
           using (MemoryStream outStream=new MemoryStream())
            {                
                using (GZipStream compressionStream = new GZipStream(outStream, CompressionMode.Compress))
                {
                    compressionStream.Write(dataBlock, 0, dataBlock.Length);
                }
                return outStream.ToArray();
            }
        }

        private void WriteFile(string destinationFile)
        {
            using (FileStream destinationStream = File.Create(destinationFile))
            {
                foreach (KeyValuePair<int, byte[]> keyValue in compressDictionary)
                {
                    destinationStream.Write(keyValue.Value, 0, keyValue.Value.Length);
                    
                }
            }
            Console.WriteLine("WriteFile {0}", Thread.CurrentThread.ManagedThreadId);
        }

        private void CompresDecompress(string sourceFile, string destinationFile)
        {
            if (operation.Equals("compress"))
            {
                //var threadReadStream = new Thread(() =>
                //{
                    //using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
                    //{                                                               
                            ReadFileInput(sourceFile);
                                                 
                    //}
                //});                

                //var threadCompress = new Thread(() =>
                //  {

                      foreach (KeyValuePair<int, byte[]> keyValue in inputDictionary)
                      {
                          var outCompress = Compress(keyValue.Value);
                          compressDictionary.Add(keyValue.Key, outCompress);
                          Console.WriteLine("CompressBlock {0}", Thread.CurrentThread.ManagedThreadId);
                      }
                  //});
                

                //var threadWriteToFile = new Thread(() =>
                //  {
                      WriteFile(destinationFile);
                     
                //  });
                //threadReadStream.Start();
                //threadCompress.Start();

                //threadWriteToFile.Start();
                //threadWriteToFile.Join();

            }
            else if (operation.Equals("decompress"))
            {
                
                var threadReadStream = new Thread(() =>
                {
                    using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
                    {
                        using (FileStream targetStream = File.Create(destinationFile))
                        {
                            WriteToFileOutput(sourceStream, targetStream);                            
                            Console.WriteLine("\t\t {0}",Thread.CurrentThread.ManagedThreadId);

                        }
                    }
                });
                threadReadStream.Start();
            }
        }

        private static object locker = new object();

        

        private void WriteToFileOutput(FileStream sourceStream, FileStream destinationStream)
        {

            using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            {
                //int blockSizeLength = 0;
                Thread[] pool;
                int read = 0;
                
                int blockLength = 0;
                Console.WriteLine("Decompress...");
                //while((read=decompressionStream.Read(buffer, 0, buffer.Length))>0)
                //while (decompressionStream.BaseStream.Position < decompressionStream.BaseStream.Length)
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
                    inputDictionary.Add(i, buffer);
                    //pool = new Thread[threadNumber];
                    //for (int i = 0; i < threadNumber; i++)
                    //{
                    //    dataArray[i] = new byte[blockSize];
                    //    decompressionStream.Read(dataArray[i], 0, blockSize);

                    //    pool[i] = new Thread(() =>
                    //      {
                    //          int j = i;
                    //          lock (locker)
                    //          {
                    //              destinationStream.Write(dataArray[i], 0, blockSize);
                    //          }

                    //      });
                    //    pool[i].Start();
                    //    Console.WriteLine(pool[i].ManagedThreadId);
                    //    pool[i].Join();
                    //}
                }
            }
            Console.WriteLine("Done");
        }






    }
}
