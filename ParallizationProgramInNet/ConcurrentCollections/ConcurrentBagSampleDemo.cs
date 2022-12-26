using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ConcurrentCollections
{
    internal class ConcurrentBagSampleDemo
    {
        void Main()
        {
            //Stack : LIFO
            //queue : FIFO
            //bag = order가 없음... : 순서대로 정렬하지 않으므로 더하고 빼는 순서가 몹시 빠름...
            var bag = new ConcurrentBag<int>();
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    bag.Add(i1);
                    Console.WriteLine($"{Task.CurrentId} has added {i1}");
                    if(bag.TryPeek(out int result))
                    {
                        Console.WriteLine($"{Task.CurrentId} has peeked the value {result}");
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            if(bag.TryTake(out int re))
            {
                Console.WriteLine($"I got {re}");
            }
        }
    }
}
