using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskProgramming
{
    internal class CancellingTasks
    {
        void Main()
        {
            #region 취소 방법 1
            var ctk = new CancellationTokenSource();
            var token = ctk.Token;

            //토큰에 이벤트 추가 : Task 취소될 때...?
            token.Register(() =>
            {
                Console.WriteLine("취소 작업이 필요");
            });
            //무한 루프이므로 중간에 취소시켜야 함 : 그 중 하나는 token

            var t = new Task(() =>
            {
                int i = 0;
                while (true)
                {
                    #region 취소 방법 1-1
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"{i++}\t");
                    }
                    #endregion
                    #region 취소 방법1-2: 동작 취소 예외를 던짐
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }
                    else
                    {
                        Console.WriteLine($"{i++}\t");
                    }
                    #endregion
                    #region 취소 방법1-3 : 취소 1-2의 코드를 한 줄로
                    token.ThrowIfCancellationRequested();
                    Console.WriteLine($"{i++}\t");
                    #endregion
                }
            }, token);
            t.Start();
            //Task가 취소되면 취소된 순간 호출됨
            Task.Factory.StartNew(() =>
            {
                token.WaitHandle.WaitOne();
                Console.WriteLine("Wait handle released, cancelation was requested");
            });
            Console.ReadKey();
            ctk.Cancel();
            #endregion
            #region 
            //
            var planned = new CancellationTokenSource();
            var preventative = new CancellationTokenSource();
            var emergency = new CancellationTokenSource();
            //위의 취소 토큰을 모두 모아서..
            var paranoid = CancellationTokenSource.CreateLinkedTokenSource(planned.Token, preventative.Token, emergency.Token);

            Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    paranoid.Token.ThrowIfCancellationRequested();
                    Console.WriteLine($"{i++}\t");
                    //1초 대기
                    Thread.Sleep(1000);
                }
            }, paranoid.Token);

            Console.ReadKey();
            emergency.Cancel();

            #endregion
        }
    }
}
