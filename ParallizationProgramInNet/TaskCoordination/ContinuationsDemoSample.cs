using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskCoordination
{
    internal class ContinuationsDemoSample
    {
        void Main()
        {
            #region 태스크 순서대로 진행될 경우
            var task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Boiling water");
            });
            var task2 = task.ContinueWith(t =>
            {
                Console.WriteLine($"Completed task {t.Id}, pour water into cup");
            });
            task2.Wait();
            #endregion
            #region 여러 태스크의 완료를 기다려서 완료된 이후에 실행되어야 할 경우
            var task3 = Task.Factory.StartNew(() => "Task3");
            var task4 = Task.Factory.StartNew(() => "Task4");
            var task5 = Task.Factory.ContinueWhenAll(new[] { task3, task4 }, tasks =>
            {
                Console.WriteLine("Tasks Completed:");
                foreach(var t in tasks)
                {
                    Console.WriteLine("-" + t.Result);
                }
                Console.WriteLine("All tasks done");
            });
            //task3,4에 Wait을 걸 필요 없이 3,4가 완료되고 실행되는 5번째만 Wait을 걸면 태스크 전체가 실행됨을 기다리게 됨
            task5.Wait();

            #endregion
            #region 여러 태스크 중 하나라도 완료 시킨 이후 진행되어야 한다면
            var task6 = Task.Factory.StartNew(() => "Task3");
            var task7 = Task.Factory.StartNew(() => "Task4");
            //
            var task8 = Task.Factory.ContinueWhenAny(new[] { task3, task4 }, t =>
            {
                Console.WriteLine("Tasks Completed:");
                Console.WriteLine(t.Result);
                Console.WriteLine("All tasks done");
            });
            //task 8은 6, 7중에 하나라도 완료되면 그 결과를 도출하고 태스크 실행을 진행
            task8.Wait();
            #endregion
        }
    }
}
