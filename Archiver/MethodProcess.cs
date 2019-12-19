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
            using (MemoryStream inputStream = new MemoryStream(dataBlock))
            {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (GZipStream compressionStream = new GZipStream(outputStream, CompressionMode.Compress))
                    {
                        byte[] buffer = new byte[inputStream.Length];
                        int read = 0;
                        while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            compressionStream.Write(buffer, 0, read);
                        }
                            //compressionStream.Write(dataBlock, 0, dataBlock.Length);
                            //inputStream.CopyTo(compressionStream);
                    }
                    return outputStream.ToArray();
                }
            }
        }

        public static byte[] Decompress(byte[] dataBlock)
        {
            using (MemoryStream inputStream = new MemoryStream(dataBlock))
            {
                using (GZipStream decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[inputStream.Length];
                        int read = 0;
                        while ((read = decompressionStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, read);
                        }
                        //decompressionStream.CopyTo(outputStream);
                        return outputStream.ToArray();
                    }
                }

            }





        }
    }
}
