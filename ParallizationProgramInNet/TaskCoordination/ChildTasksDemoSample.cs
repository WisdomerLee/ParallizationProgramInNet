using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskCoordination
{
    internal class ChildTasksDemoSample
    {
        void Main()
        {
            //태스크 내부에서 자식 태스크를 생성할 때 태스크 생성시 옵션을 부모에게 붙이기를 사용하면 자식 태스크들이 모두 완료될 때까지 대기 함 : 해당 옵션이 없으면 자식 태스크 완료를 기다리지 않고 그냥 넘어감...
            //TaskCreationOptions.AttachedToParent : 이것을 Task 생성자에 옵션으로 넣으면... 부모 Task에 딸린 Task로 바뀜 : 부모를 실행하면 자식 태스크가 모두 완료될 때까지 기다리게 됨
            var parent = new Task(() =>
            {
                var child = new Task(() =>
                {
                    Console.WriteLine("Child task starting");
                    Thread.Sleep(3000);
                    Console.WriteLine("Child task finishing");
                }, TaskCreationOptions.AttachedToParent);
                #region 태스크 연속시 조건에 따라 분기를 넣고 싶을 때
                //이것은 태스크가 무사히 완료되면... 아래의 함수가 실행
                var completionHandler = child.ContinueWith(t =>
                {
                    Console.WriteLine($"task {t.Id}'s state is {t.Status}");
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion);
                //태스크 실행 실패시 아래의 함수가 실행됨
                var failHandler = child.ContinueWith(t =>
                {
                    Console.WriteLine($"task {t.Id}'s state is {t.Status}");
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);
                #endregion
                child.Start();
            });
            parent.Start();
            try
            {
                parent.Wait();
            }
            catch(AggregateException ae)
            {
                ae.Handle(e => true);
            }
        }
    }
}
