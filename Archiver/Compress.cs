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
                    CompressDataBlocksDictionary.TryAdd(i, buffer);
                    //CompressDataBlocksDictionary.Add(new BlockingCollection(i, buffer));
                    Console.WriteLine("ReadFile {0}", Thread.CurrentThread.ManagedThreadId);
                }

            }

        }

        protected override void BlockProcessing(/*int indexThread*/)
        {
            //for (int i = 0; i < CompressDataBlocksDictionary.Keys.Count; i++)
            //{
            //    var outCompress = MethodProcess.Compress(CompressDataBlocksDictionary[i]);
            //    CompressDataBlocksDictionary[i] = outCompress;
            //    Console.WriteLine("CompressBlock {0}", Thread.CurrentThread.ManagedThreadId);
            //}

            for (int i = 0; i < CompressDataBlocksDictionary.Keys.Count; i++)
            {
                var outCompress = MethodProcess.Compress(CompressDataBlocksDictionary[i]);
                CompressDataBlocksDictionary[i] = outCompress;
                Console.WriteLine("CompressBlock {0}", Thread.CurrentThread.ManagedThreadId);
            }
            //blockProcces[indexThread].Set();
        }

        protected override void WriteToFile()
        {
            Console.WriteLine("WriteToFile...");
            using (FileStream destinationStream = File.Create(OutputFile + ".gz"))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(destinationStream))
                {
                    foreach (KeyValuePair<int, byte[]> keyValue in CompressDataBlocksDictionary)
                    {
                        binaryWriter.Write(keyValue.Value.Length);
                        binaryWriter.Write(keyValue.Value, 0, keyValue.Value.Length);

                    }
                }
            }
            Console.WriteLine("WriteFile {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
