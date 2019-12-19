namespace Archiver
{
    public class BlockingCollection
    {
        public BlockingCollection(int Id, byte[] byteBlock)
        {
            this.Id = Id;
            this.byteBlock = byteBlock;
        }

        public int Id { get; set; }
        public byte[] byteBlock { get; set; }

       
    }
}