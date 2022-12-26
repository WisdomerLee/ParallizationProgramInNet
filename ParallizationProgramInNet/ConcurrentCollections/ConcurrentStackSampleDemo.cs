using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ConcurrentCollections
{
    internal class ConcurrentStackSampleDemo
    {
        void Main()
        {
            var stack = new ConcurrentStack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            if(stack.TryPeek(out int result))
            {
                Console.WriteLine($"{result} is on top");
            }
            if(stack.TryPop(out int t))
            {
                Console.WriteLine($"Popped {t}");
            }

            var items = new int[5];
            if(stack.TryPopRange(items, 0, 5)> 0)
            {
                var text = string.Join(",", items.Select(i => i.ToString()));
                Console.WriteLine($"Popped these items : {text}");
            }
        }
    }
}
