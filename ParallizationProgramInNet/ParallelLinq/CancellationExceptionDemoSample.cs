using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ParallelLinq
{
    internal class CancellationExceptionDemoSample
    {
        void Main()
        {
            var cts = new CancellationTokenSource();


            //Enumerable과의 차이는 병렬로 처리된다는 것
            var items = ParallelEnumerable.Range(1, 20);
            //중간에 멈추게 될 상황을 넣고 싶으면...
            //WithCancellation()함수를 중간에 넣고 함수의 변수로 취소 토큰을 넣어줌
            var results = items.WithCancellation(cts.Token).Select(i =>
            {
                double result = Math.Log10(i);
                //만약 중간에 문제가 생기면
                //if (result > 1)
                //{
                //    throw new InvalidOperationException();
                //}

                Console.WriteLine($"i = {i}, tid = {Task.CurrentId} \t");
                return result;
            });
            try
            {
                foreach(var c in results)
                {
                    //값이 1보다 커지면 취소하도록 처리 : 취소 토큰을 넣었으므로 반응함...넣지 않으면? 아무 영향 없음
                    //취소를 호출하더라도 기본적으로 병렬처리이므로 일부 값들은 조건보다 큰 상황도 나올 수 있음
                    if (c > 1)
                    {
                        cts.Cancel();
                    }
                    Console.WriteLine($"result = {c}");
                }
            }
            catch(AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine($"{e.GetType().Name}: {e.Message}");
                    return true;
                });
            }
            catch(OperationCanceledException e)
            {
                Console.WriteLine("Canceled");
            }
        }
    }
}
