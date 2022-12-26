using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskProgramming
{
    internal class WaitingforTasks
    {
        void Main()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var t = new Task(() =>
            {
                Console.WriteLine("I take 5 seconds");
                for(int i = 0; i<5; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(1000);
                }
                Console.WriteLine("I am done");
            }, token);
            t.Start();

            t.Wait(token);

            Task t2 = Task.Factory.StartNew(() => Thread.Sleep(3000), token);

            //WaitAny의 경우 하나의 태스크가 완료되는 순간 다른 태스크 완료를 기다리지 않고 그대로 넘어감
            Task.WaitAny(t, t2);
            //시간 제한을 두면 해당 시간 내에 태스크가 완료되지 않더라도 넘김...
            Task.WaitAny(new[] { t, t2 }, 4000);
            //혹은 취소되면 그 경우에도 바로 넘어가게 할 수 있음
            Task.WaitAny(new[] { t, t2 }, 4000, token);
            //WaitAll의 경우 파라미터로 넘겨진 모든 태스크가 완료될 때까지 대기
            Task.WaitAll(t, t2);

            ////아래의 함수는 Task 모두 완료될 때까지 대기함
            //Task.WaitAll();
        }
    }
}
