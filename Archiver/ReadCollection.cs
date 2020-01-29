using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Archiver
{
    public class ReadCollection
    {
        private Queue<Blocks> processingDataBlocks = new Queue<Blocks>();
        private readonly object locker = new object();
        private bool completed = false;

        public void Enqueue(Blocks chunk)
        {
            lock (locker)
            {
                processingDataBlocks.Enqueue(chunk);
                Monitor.PulseAll(locker);
            }
        }

        public bool TryDequeue(out Blocks chunk)
        {
            lock (locker)
            {
                while (processingDataBlocks.Count == 0)
                {
                    if (completed)
                    {
                        chunk = new Blocks();
                        return false;
                    }
                    Monitor.Wait(locker);
                }
                chunk = processingDataBlocks.Dequeue();
                Monitor.PulseAll(locker);
                return true;
            }
        }

        public void ReadComplete()
        {
            lock (locker)
            {
                completed = true;
                Monitor.PulseAll(locker);
            }
        }
    }
}

