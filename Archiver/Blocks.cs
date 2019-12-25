using System.Collections.Concurrent;

namespace Archiver
{
    public class Blocks 
    {
        public Blocks(int Id, byte[] byteBlock)
        {
            this.Id = Id;
            this.byteBlock = byteBlock;
        }

        public int Id { get; set; }
        public byte[] byteBlock { get; set; }


       
    }
}