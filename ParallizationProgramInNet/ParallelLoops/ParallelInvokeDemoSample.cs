using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ParallelLoops
{
    internal class ParallelInvokeDemoSample
    {
        void ParallelMain()
        {
            var a = new Action(() => Console.WriteLine($"First {Task.CurrentId}"));
            var b = new Action(() => Console.WriteLine($"Second {Task.CurrentId}"));
            var c = new Action(() => Console.WriteLine($"Third {Task.CurrentId}"));

            Parallel.Invoke(a, b, c);
            //Parallel For에서는 첫번째 들어가는 숫자는 처음 조건 : for문과 유사함, 두 번째 숫자는 for문이 끝날 숫자, 세번째 변수는 함수

            Parallel.For(1, 11, i =>
            {
                Console.WriteLine($"{i * i}\t");
            });
            //만약 1씩 증가하는 것이 아니라 특정 숫자(2 혹은 3, 등등)씩 증가하는 경우에만 동시에 여럿을 불러야 할 경우는 어떻게..?
            //함수를 만들고 Foreach활용
            Parallel.ForEach(RangeFunction(1, 20, 3), Console.WriteLine);
            //ParallelOptions로 함수 호출 옵션을 변경할 수 있음
            var parallelOption = new ParallelOptions();
            //취소되는 지점을 설정하거나
            parallelOption.CancellationToken = new CancellationTokenSource().Token;
            //태스크들 스케쥴을 지정하거나
            parallelOption.TaskScheduler = TaskScheduler.Current;
            //태스크들이 동시에 진행될 수 있는 최대 숫자를 지정하여 특정 이상 넘어가지 않게..
            parallelOption.MaxDegreeOfParallelism = 5;
            Parallel.For(1, 10, parallelOption, j =>
            {

            });

            string[] words = { "oh", "what", "a", "night" };
            //Parallel Foreach의 첫번째 변수는 루프를 돌릴 배열, 리스트 등, 두 번째 변수는 함수, 
            Parallel.ForEach(words, word =>
            {
                Console.WriteLine($"{word} has length {word.Length}");
            });
        }

        ParallelLoopResult result;
        void Demo()
        {
            var cts = new CancellationTokenSource();

            var parallelOption = new ParallelOptions();
            parallelOption.CancellationToken = cts.Token;

            //Parallel For에 집어넣을 수 있는 함수는 2개의 변수를 넣을 수도 있음
            result = Parallel.For(0, 20, parallelOption, (x, state)=>
            {
                Console.WriteLine($"{x}[{Task.CurrentId}]\t");
                //특정 상황인 경우는 작성하지 않게....
                if (x== 10)
                {
                    //해당 상태만 멈추기
                    state.Stop();
                    //그 뒤의 반복작업도 멈추기
                    state.Break();
                    //만약 예외가 발생한다면...
                    //throw new Exception();
                    cts.Cancel();
                }
            });
            Console.WriteLine();
            Console.WriteLine($"Was loop completed? {result.IsCompleted}");
            if (result.LowestBreakIteration.HasValue)
            {
                Console.WriteLine($"Lowest break iteration is {result.LowestBreakIteration}");
            }
        }
        void BreakingMain()
        {
            //예외처리를 위해 try catch로 
            try
            {
                Demo();
            }
            catch(AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine(e.Message);
                    return true;
                });
            }
            catch(OperationCanceledException oce)
            {

            }
        }
        //스텝을 건너뛰어 호출하게 만들고 싶으면 아래와 같은 형태로 만들어 Parallel.Foreach로 호출하기
        IEnumerable<int> RangeFunction(int start, int end, int step)
        {
            for (int i = start; i < end; i += step)
            {
                yield return i;
            }
        }
    }
}
