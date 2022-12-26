using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskCoordination
{
    internal class AutoManualResetEventDemoSample
    {
        void Main()
        {
            //Barrier, Countdown과의 차이...?
            //Barrier, Countdown은 생성자에 Signal을 대기해야 하는 숫자를 집어넣고 Signal이 모두 0 혹은 증가할 때까지 기다려야 한다면
            //ResetEvent는 생성자에 숫자를 넣을 필요 없고 Set, Wait를 변경할 수 있음
            //Auto와 Manual의 차이는..?
            //Manual은 Wait이후에 추가로 Wait을 넣을 수 있음 : 이미 Set이 되어 Wait도 모두 통과됨..
            //

            var evt = new ManualResetEventSlim();
            //자동은 생성자에 bool값을 넣어야 함, 기본은 false로 넣어줌
            var evat = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Boiling water");
                //ManualResetEvent에 카운트 넘기듯이 씀 : Countdown의 Signal과 유사...?
                evt.Set();
                evat.Set();
            });
            var makeTea = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Waiting for water");
                //evt의 Set태스크가 완료될 때까지 대기
                evt.Wait();
                //자동은 Wait대신 WaitOne함수를 호출
                evat.WaitOne();
                
                Console.WriteLine("Here is your tea");
                //매뉴얼은 자동으로 리셋되지 않으므로 하나만 처리되었어도 그 밑에서 대기하는 것들이 자동으로 지나감....
                evt.Wait();
                //자동은 해당 Set, Wait 사이클이 한 번 돌면 변수가 자동으로 초기화되므로...Set은 하나인데 Wait이 여럿이면 멈춤현상이 발생
                //멈춤 현상을 방지하기 위해서는... WaitOne()함수에 숫자를 넣게 되면 해당 시간 동안 Set함수 호출을 대기하고 그 이상의 시간이 지나면 다음으로 넘김

                var ok = evat.WaitOne(1000);
                if (ok)
                {
                    Console.WriteLine("Enjoy your tea");
                }
                else
                {
                    Console.WriteLine("No tea for you");
                }
            });
            makeTea.Wait();
        }
    }
}
