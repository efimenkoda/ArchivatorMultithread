namespace Archiver
{
    public class BlockingCollection
    {
        public BlockingCollection(int Id, byte[] byteBlock)
        {
            this.Id = Id;
            this.byteBlock = byteBlock;
            lengthBlock = byteBlock.Length;
        }

        public int Id { get; set; }
        public byte[] byteBlock { get; set; }

        public int lengthBlock { get; set; }

       
    }
}