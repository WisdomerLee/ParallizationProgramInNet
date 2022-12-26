using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskCoordination
{
    //배리어는 언제 쓸까? 어떤 단계가 순차적으로 진행되어야 할 경우... 서로 다른 상황들이 동시에 진행되면서 특정 상황들이 특정 단계가 완료되어야 그 다음으로 넘어갈 수 있을 때 쓰임
    internal class BarrierDemoSample
    {
        //Barrier 생성자에 들어가는 요소는 첫번째 : Barrier가 관여하는 알고리즘이나 연산에 쓰일 스레드(태스크)숫자, 두 번째는 그 Barrier가 진행되었을 때 실행되어야 할 함수
        //해당 예시에서는 Barrier가 쓰이는 곳이 함수 두군데이므로 숫자를 2를 넣음
        Barrier barrier = new Barrier(2, b =>
        {
            Console.WriteLine($"Phase {b.CurrentPhaseNumber} is finished");
            #region 단계를 더 추가한다면
            //아래의 것은 해당 연산에서 남은 단계 숫자를 알려줌
            Console.WriteLine(b.ParticipantsRemaining);
            //아래의 경우는 연계된 함수를 또 하나 추가해 줌
            b.AddParticipant();
            //여럿을 추가할 경우는 추가할 숫자를 넣어주면 됨...
            b.AddParticipants(2);
            #endregion
        });

        void Water()
        {
            Console.WriteLine("Putting the kettle on a (takes a bit longer)");
            //긴 연산을 대체하여...
            Thread.Sleep(2000);
            //배리어를 추가하고 기다리게 하는 함수
            //최초 배리어 추가 되는 부분 이전 : Phase1 그 이후 Phase2, Phase3 등이 됨
            barrier.SignalAndWait();
            Console.WriteLine("Pouring water into cup");
            barrier.SignalAndWait();
            Console.WriteLine("Putting the kettle away");
        }

        void Cup()
        {
            Console.WriteLine("Finding the nicest cup of tea(fast)");
            //배리어를 추가하고 기다리게 하는 함수
            barrier.SignalAndWait();
            Console.WriteLine("Adding tea");
            barrier.SignalAndWait();
            Console.WriteLine("Adding sugar");

        }
        void Main()
        {
            var water = Task.Factory.StartNew(Water);
            var cup = Task.Factory.StartNew(Cup);

            var tea = Task.Factory.ContinueWhenAll(new[] { water, cup }, tasks =>
            {
                Console.WriteLine("Enjoy your cup of tea");
            });
            tea.Wait();
        }
    }
}
