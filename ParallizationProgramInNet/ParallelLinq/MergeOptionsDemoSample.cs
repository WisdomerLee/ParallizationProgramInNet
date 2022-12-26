using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ParallelLinq
{
    internal class MergeOptionsDemoSample
    {
        void Main()
        {
            var numbers = Enumerable.Range(1, 20).ToArray();
            //WithMergeOptions()함수
            //ParallelMergeOptions.NotBuffered: 함수가 처리되는대로 다음 것도 같이 처리
            //ParallelMergeOptions.FullyBuffered : 함수가 모두 처리된 다음에 다음 것이 진행됨
            //아래의 예시에서는 NotBuffered로 처리하게 되면 results는 병렬로 result값이 나온 직후 바로 다음 foreach문으로 넘어가 conume함수가 처리
            //FullyBuffered에서는 모든 결과 값이 다 처리되고 난 뒤에 foreach 문으로 넘어가 consume함수가 처리됨..
            //
            var results = numbers.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered).Select(x =>
            {
                var result = Math.Log10(x);
                Console.WriteLine($"Produced {result}");
                return result;
            });
            foreach(var result in results )
            {
                Console.WriteLine($"Consumed {result}");
            }
        }
    }
}
