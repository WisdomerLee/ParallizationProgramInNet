using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskProgramming
{
    internal class TaskExceptionHandlingClassDemo
    {
        void Main()
        {
            Test();

            //Handle을 통해 예외를 처리하므로 ...처리되는 예외들을 아래와 같은 형태로 얻어낼 수 있음...?
            try
            {
                Test2();
            }
            catch(AggregateException ae)
            {
                foreach(var e in ae.InnerExceptions)
                {
                    Console.WriteLine($"Handled elsewhere: {e.GetType()}");
                }
            }

        }
        void Test()
        {
            //아무 것도 없이 그냥 실행하면....
            var t = Task.Factory.StartNew(() =>
            {
                throw new InvalidOperationException("Can't do this!") { Source = "t" };
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                throw new AccessViolationException("Can't access this!") { Source = "t2" };
            });

            //메인 함수와 별도의 태스크로 실행되기 때문에 try, catch문구 없이 그냥 실행하면 예외가 발생해도 발생한 사실 자체가 화면에 표시되지 않음...
            //또한 그냥 Task.WaitAll(t, t2)를 실행하게 되면 기다려야 하는 Task들이 모두 실행이 예외처리가 발생하여 멈추게 되므로 태스크의 예외와는 다른 Task가 완료되지 않았다는 예외처리 결과만 얻게 됨
            try
            {
                Task.WaitAll(t, t2);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    Console.WriteLine($"Exception {e.GetType()} from {e.Source}");
                }
            }
        }
        void Test2()
        {
            //아무 것도 없이 그냥 실행하면....
            var t = Task.Factory.StartNew(() =>
            {
                throw new InvalidOperationException("Can't do this!") { Source = "t" };
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                throw new AccessViolationException("Can't access this!") { Source = "t2" };
            });

            //메인 함수와 별도의 태스크로 실행되기 때문에 try, catch문구 없이 그냥 실행하면 예외가 발생해도 발생한 사실 자체가 화면에 표시되지 않음...
            //또한 그냥 Task.WaitAll(t, t2)를 실행하게 되면 기다려야 하는 Task들이 모두 실행이 예외처리가 발생하여 멈추게 되므로 태스크의 예외와는 다른 Task가 완료되지 않았다는 예외처리 결과만 얻게 됨
            try
            {
                Task.WaitAll(t, t2);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    if(e is InvalidOperationException)
                    {
                        Console.WriteLine("Invalid Op!");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
        }
    }
}
