using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiver
{
    public class GZipFileManager
    {
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

        private void CreateGzip(FileStream sourceStream, FileStream destinationStream)
        {
            using (GZipStream compressionStream = new GZipStream(destinationStream, CompressionMode.Compress))
            {
                sourceStream.CopyTo(compressionStream);
                Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                            sourceFile, sourceStream.Length.ToString(), destinationStream.Length.ToString());
            }

        }

        private void ExtractFile(FileStream sourceStream,FileStream destinationStream)
        {
            using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(destinationStream);
                Console.WriteLine("Восстановлен файл: {0}", destinationFile);
            }
            
        }


    }
}
