using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskCoordination
{
    internal class CountdownEventDemoSample
    {
        //카운트 다운은 여러 태스크가 완료되어야 그 다음 태스크의 내용이 실행되어야 할 경우 쓰임
        //카운트 다운 이벤트 생성, Barrier와 마찬가지로 생성자 첫번째에는 실행에 연관되는 함수(태스크) 숫자를 넣어줌
        //배리어에서는 호출되는 함수에 barrier.SignalAndWait()를 넣어주어야 하나 
        //카운트 다운에서는 호출되는 함수에 Signal, Wait가 나뉘어 쓰임

        CountdownEvent cte = new CountdownEvent(5);

        Random random = new Random();
        void Main()
        {
            for(int i =0; i<5; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Entering task {Task.CurrentId}");
                    Thread.Sleep(random.Next(3000));
                    //카운트 다운을 하여 생성자에 넣은 숫자에서 1을 차감, 태스크가 여럿 실행되어 숫자가 0이 되면 그 순간 cte.Wait()가 있는 함수가 호출되게 됨
                    cte.Signal();
                    Console.WriteLine($"Exiting task {Task.CurrentId}");
                });
            }
            var finalTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Waiting for other tasks to complete in {Task.CurrentId}");
                cte.Wait();
                Console.WriteLine("All tasks completed");

            });
            finalTask.Wait();
        }
    }
}
