using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskProgramming
{
    internal class WaitingforTimetoPass
    {
        void Main()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var t = new Task(() =>
            {
                Console.WriteLine("Press any key to disarm; you hanve 5 seconds");
                //token의 WaitHandle.WaitOne()함수를 이용하여 특정 시간 동안 대기를 할 수 있음
                //그 동안 버튼을 누르게 되면 취소되어 아래의 cts.Cancel()함수가 호출
                bool cancelled = token.WaitHandle.WaitOne(5000);
                Console.WriteLine(cancelled ? "Bomb disarm" : "BOMB!");
                //SpinWait.SpinUntil();
                //Thread.SpinWait();
            }, token);

            Console.ReadKey();
            cts.Cancel();
        }
    }
}
