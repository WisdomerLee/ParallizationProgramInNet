using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ParallelLoops
{
    //벤치마켓 Nuget Package를 프로젝트에 더한 상태를 가정 : BenchmarkDotNet
    //저걸 둔 이유는 처리속도 차이를 알아보기 위한 것...
    internal class PartitioningDemoSample
    {
        [Benchmark]
        void SquareEachValue()
        {
            int count = 100000;
            var values = Enumerable.Range(0, count);
            var results = new int[count];
            //아래와 같은 상황은 좋지 않음... 매번 함수를 만들어 처리하기 때문
            //
            Parallel.ForEach(values, x => { results[x] =(int) Math.Pow(x, 2); });
        }
        [Benchmark]
        void SquareEachValueChunked()
        {
            int count = 100000;
            var values = Enumerable.Range(0, count);
            var results = new int[count];
            //Partitioner를 만들고 만든 것을 병렬처리하면 속도가 몇 배 이상 빨라짐
            //데이터를 나눔
            //임의의 함수를 매번 만들어 처리하는 것등을 방지
            //Partitioner.Create에 들어간 첫번째 변수 : 처음 숫자, 두번째 변수 : 처음 숫자와 두번째 변수의 차이 만큼의 데이터 숫자를 처리할 예정, 세번째 변수: 데이터 숫자를 나눌 크기
            //아래의 예시에서는 0부터 10만이므로 데이터 숫자가 10만개이고 그 데이터를 1만개 단위로 10개로 쪼개는 것
            var part = Partitioner.Create(0, count, 10000);
            Parallel.ForEach(part, range =>
            {
                for(int i = range.Item1; i< range.Item2; i++)
                {
                    results[i] = (int) Math.Pow(i, 2);
                }
            });
        }
        void Main()
        {
            var summary = BenchmarkRunner.Run<PartitioningDemoSample>();
            Console.WriteLine(summary);
        }
    }
}
