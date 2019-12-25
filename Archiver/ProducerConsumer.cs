using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Archiver
{
    public class ProducerConsumer
    {
        object locker = new object();       
        bool stop = false;
        private ConcurrentDictionary<int, byte[]> compressDataBlocksDictionary1 = new ConcurrentDictionary<int, byte[]>();
        private int index=0;
        public void Add(int id, byte[] data)
        {
            lock (locker)
            {
                compressDataBlocksDictionary1.TryAdd(id, data);
                Monitor.Pulse(locker);
            }
        }

        public byte[] GetValueById(int id)
        {
            //byte[] value;
            //for (int i = 0; i < compressDataBlocksDictionary.Keys.Count(); i++)
            //    value = compressDataBlocksDictionary[i];
            return compressDataBlocksDictionary1[id];
        }

        public byte[] GetValue()
        {
            byte[] result;
            lock (locker)
            {
                while (!compressDataBlocksDictionary1.ContainsKey(index))
                {
                    if (stop)
                    {
                        result = new byte[0];
                        return result;
                    }
                    Monitor.Wait(locker);
                }
                result = compressDataBlocksDictionary1[index++];
                Monitor.PulseAll(locker);
                return result;
            }
            
        }

        public void SetValue(int id,byte[] data)
        {
            lock (locker)
            {
                compressDataBlocksDictionary1[id] = data;
                Monitor.Pulse(locker);
            }
        }
        public int KeysCount()
        {
            return compressDataBlocksDictionary1.Keys.Count;
        }

        public void StopThread()
        {
            lock (locker)
            {
                stop = true;
                Monitor.PulseAll(locker);
            }
        }

    }
}
