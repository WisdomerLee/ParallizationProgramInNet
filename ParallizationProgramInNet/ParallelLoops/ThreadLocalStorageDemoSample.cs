using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ParallelLoops
{
    internal class ThreadLocalStorageDemoSample
    {
        void Main()
        {
            //만약 여러 태스크에서 동시에 작업을 시켰는데 결과를 저장할 곳이 한군데라면...?
            int sum = 0;
            Parallel.For(1, 1001, x =>
            {
                //한 번에 태스크 하나씩 접근하므로 병렬로 처리할 이유가 없음..., 속도 느려짐은 덤
                Interlocked.Add(ref sum, x);
            });
            //Parallel For에 들어간 3번째 변수로 들어간 람다식은 Thread local stage에 최초로 등록할 변수를 지정하는 것
            //4번째 변수로 들어간 람다식은 변수를 3개를 들고 있는데 순서대로 첫번째는 iteration변수, 두번째는 태스크 루프 상태, 세번째는 스레드로컬스토리지 값임
            //5번째 변수로 들어간 람다식은 각 태스크들이 4번째 변수로 들어간 값을 끝내고 들고 있는 값을 처리하는 것
            Parallel.For(1, 1001, () => 0, (x, state, tls) =>
            {
                //tls : 해당 스레드에서만 접근 가능한 영역이므로 다른 태스크들이 끼어들 수 없음 > Interlocked를 쓸 필요가 없음
                //각각 태스크에서 처리한 부분 합들을 모두 모아서 : 병렬로 처리되므로 속도 처리가 빠름
                tls += x;
                Console.WriteLine($"Task {Task.CurrentId} has sum {tls}");
                return tls;
            }, partialSum =>
            {
                Console.WriteLine($"Partial value of task {Task.CurrentId} is {partialSum}");
                //부분적으로 처리된 값을 한번에 하나씩 더해 완료 함 : 여기서 병목 현상 발생
                Interlocked.Add(ref sum, partialSum);
            });
            Console.WriteLine($"Sum of 1...1000 = {sum}");
        }
    }
}
