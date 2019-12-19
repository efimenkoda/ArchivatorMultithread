using System;
using System.Collections.Generic;
using System.IO;
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
            int blockLength = 0;            
            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                Console.WriteLine("Decompress...");
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
                        DataBlocksDictionary.Add(i, buffer);
                        Console.WriteLine("ReadFile {0}", Thread.CurrentThread.ManagedThreadId);
                    }
                }
            }
        
        }

        protected override void BlockOperation()
        {
            //foreach (KeyValuePair<int, byte[]> keyValue in inputDictionary)
            //{                
            //    var outputDecompress = MethodProcess.Decompress(keyValue.Value);
            //    inputDictionary.Add(keyValue.Key, outputDecompress);                
            //    Console.WriteLine("DecompressBlock {0}", Thread.CurrentThread.ManagedThreadId);
            //}

            for (int i = 0; i < DataBlocksDictionary.Keys.Count; i++)
            {
                var outCompress = MethodProcess.Decompress(DataBlocksDictionary[i]);
                DataBlocksDictionary[i] = outCompress;
                Console.WriteLine("DecompressBlock {0}", Thread.CurrentThread.ManagedThreadId);
            }
        }

        protected override void WriteToFile()
        {
            Console.WriteLine("WriteToFile...");
            using (FileStream destinationStream = File.Create(OutputFile))
            {
                foreach (KeyValuePair<int, byte[]> keyValue in DataBlocksDictionary)
                {
                    destinationStream.Write(keyValue.Value, 0, keyValue.Value.Length);

                }
            }
            Console.WriteLine("WriteFile {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
