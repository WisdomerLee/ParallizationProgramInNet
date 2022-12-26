using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ConcurrentCollections
{
    //BlockingCollection은 쓰는 동안 Collection에 값을 더할 수 없게 막음..., 특정 범위 이상의 값을 넣을 수 없도록 막음
    internal class ProducerConsumerPatternSampleDemo
    {
        //아래의 경우는 BlockingCollection으로 ConcurrentBag을 쓰고 값을 10개까지만 허용
        BlockingCollection<int> messages = new BlockingCollection<int>(new ConcurrentBag<int>(), 10);

        CancellationTokenSource cts = new CancellationTokenSource();
        Random random = new Random();
        void ProduceConsume()
        {
            var producer = Task.Factory.StartNew(RunProducer);
            var consumer = Task.Factory.StartNew(RunConsumer);
            try
            {
                Task.WaitAll(new[] { producer, consumer }, cts.Token);
            }
            catch (AggregateException ex)
            {
                ex.Handle(e => true);
            }
        }
        void Main()
        {
            Task.Factory.StartNew(ProduceConsume, cts.Token);
            Console.ReadKey();
            cts.Cancel();
        }
        void RunProducer()
        {
            while (true)
            {
                cts.Token.ThrowIfCancellationRequested();
                int i = random.Next(100);
                //BlockingCollection에 할당된 최대 갯수를 넘어서 값을 더하려고 하면 해당 Task가 Block  > BlockingCollection이라고 부르는 이유
                messages.Add(i);
                Console.WriteLine($"+{i}\t");
                Thread.Sleep(random.Next(1000));

            }
        }
        void RunConsumer()
        {
            foreach (var item in messages.GetConsumingEnumerable())
            {
                cts.Token.ThrowIfCancellationRequested();
                Console.WriteLine($"-{item}\t");
                Thread.Sleep(random.Next(1000));
            }
        }

    }
}
