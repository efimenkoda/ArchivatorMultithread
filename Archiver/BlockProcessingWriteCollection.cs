using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Archiver
{
    public class BlockProcessingWriteCollection
    {
        private static int countThreads =  Environment.ProcessorCount;
        Dictionary<int,byte[]> dataBlocksToWrite;
        private static object locker = new object();
        private bool completed = false;
        private int index = 0;

        public BlockProcessingWriteCollection()
        {
            dataBlocksToWrite = new Dictionary<int, byte[]>();
        }
        public void Add(int id, byte[] bytes)
        {
            lock (locker)
            {
                while (dataBlocksToWrite.Keys.Count >= countThreads* 10)
                {
                    Monitor.Wait(locker);
                }
                dataBlocksToWrite.Add(id, bytes);
                Monitor.PulseAll(locker);
            }
        }
      
        public bool GetValue(out byte[] data)
        {
            lock (locker)
            {
                while (!dataBlocksToWrite.ContainsKey(index))
                {
                    if (completed)
                    {
                        data = new byte[0];
                        return false;
                    }
                    Monitor.Wait(locker);
                }
                data = dataBlocksToWrite[index];
                dataBlocksToWrite.Remove(index++);                
                Monitor.PulseAll(locker);
                return true;
            }
        }

        public void Completed()
        {
            lock (locker)
            {
                completed = true;
                Monitor.PulseAll(locker);
            }
        }
    }
}
