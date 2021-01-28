using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindVisualizer
{
    public class PriorityQueue<T>
    {
        private Dictionary<T, int> data;

        public PriorityQueue()
        {
            this.data = new Dictionary<T, int>();
        }

        public void Enqueue(T item, int priority)
        {
            if (data.ContainsKey(item))
                data.Remove(item);
            data[item] = priority;
        }

        public T Dequeue()
        {
            if (data.Count == 0)
            {
                throw new Exception("Queue is empty");
            }

            T bestItem = default(T);
            int bestPriority = int.MaxValue;

            foreach(var item in data)
            {
                if(item.Value < bestPriority)
                {
                    bestItem = item.Key;
                    bestPriority = item.Value;
                }
            }

            data.Remove(bestItem);
            return bestItem;
        }

        public bool Contains(T item)
        {
            return data.ContainsKey(item);
        }

        public void Clear()
        {
            data.Clear();
        }

        public int Count()
        {
            return data.Count;
        }

        public override string ToString()
        {
            string s = "{";
            foreach (var item in data.Keys)
                s += item.ToString() + ", ";
            s += "}, count = " + data.Count;
            return s;
        }
    }
}
