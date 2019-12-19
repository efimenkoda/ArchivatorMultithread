using System;
using System.Collections.Generic;
using System.IO;
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
            int blockLength = 0;
            Console.WriteLine("Compress...");
            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open,FileAccess.Read))
            {
                
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
            for (int i = 0; i < DataBlocksDictionary.Keys.Count; i++)
            {
                var outCompress = MethodProcess.Compress(DataBlocksDictionary[i]);
                DataBlocksDictionary[i] = outCompress;
                Console.WriteLine("CompressBlock {0}", Thread.CurrentThread.ManagedThreadId);
            }
            //foreach (KeyValuePair<int, byte[]> keyValue in inputDictionary)
            //{
                
            //    var outCompress = MethodProcess.Compress(keyValue.Value);
            //    outputDictionary.Add(keyValue.Key, outCompress);                
            //    Console.WriteLine("CompressBlock {0}", Thread.CurrentThread.ManagedThreadId);
            //}
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
