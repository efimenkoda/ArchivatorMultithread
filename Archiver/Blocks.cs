using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiver
{
   public class Blocks
    {
        public Blocks()
        {
        }
        public Blocks(int id, byte[] block)
        {
            ID = id;
            Block = block;
        }

        public int ID { get; set; }
        public byte[] Block { get; set; }
    }
}
