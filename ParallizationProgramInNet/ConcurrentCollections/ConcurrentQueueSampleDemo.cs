using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ConcurrentCollections
{
    internal class ConcurrentQueueSampleDemo
    {
        void Main()
        {
            var q = new ConcurrentQueue<int>();
            q.Enqueue(1);
            q.Enqueue(2);

            
            if(q.TryDequeue(out int result))
            {
                Console.WriteLine($"Removed element {result}");
            }
            if(q.TryPeek(out int resul))
            {
                Console.WriteLine($"Front element is {resul}");
            }
        }
    }
}
