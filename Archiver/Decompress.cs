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

            Console.WriteLine("Decompress...");
            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                using (var binaryReader = new BinaryReader(sourceStream))
                {
                    FileInfo file = new FileInfo(InputFile);
                    var sizeFileInput = file.Length;
                    {
                        for (int i = 0; sizeFileInput > 0; i++)
                        {
                            int sizeCompressBlock = binaryReader.ReadInt32();
                            byte[] buffer = binaryReader.ReadBytes(sizeCompressBlock);
                            CompressDataBlocksDictionary.TryAdd(i, buffer);
                            sizeFileInput =sizeFileInput-(sizeCompressBlock+4);
                            Console.WriteLine("ReadFile {0}", Thread.CurrentThread.ManagedThreadId);

                        }
                    }
                }
            }

        }

        protected override void BlockProcessing(/*int indexThread*/)
        {

            for (int i = 0; i < CompressDataBlocksDictionary.Keys.Count; i++)
            {
                var outCompress = MethodProcess.Decompress(CompressDataBlocksDictionary[i]);
                CompressDataBlocksDictionary[i] = outCompress;
                Console.WriteLine("DecompressBlock {0}", Thread.CurrentThread.ManagedThreadId);
            }
            //blockProcces[indexThread].Set();
        }

        protected override void WriteToFile()
        {
            Console.WriteLine("WriteToFile...");
            using (FileStream destinationStream = File.Create(OutputFile))
            {
                foreach (KeyValuePair<int, byte[]> keyValue in CompressDataBlocksDictionary)
                {
                    destinationStream.Write(keyValue.Value, 0, keyValue.Value.Length);

                }
            }
            Console.WriteLine("WriteFile {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
