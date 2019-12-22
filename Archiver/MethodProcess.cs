using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiver
{
    public class MethodProcess
    {
        public static byte[] Compress(byte[] dataBlock)
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

        public static byte[] Decompress(byte[] dataBlock)
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
    }
}
