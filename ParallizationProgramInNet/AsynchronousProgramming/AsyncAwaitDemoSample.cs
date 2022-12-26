using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.AsynchronousProgramming
{
    internal class AsyncAwaitDemoSample
    {
        #region async await없을 때
        Task<int> CalculateValueAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return 123;
            });
        }

        void ButtonClick()
        {
            var calculation = CalculateValueAsync();
            calculation.ContinueWith(t =>
            {
                Console.WriteLine(t.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
        #endregion
        #region async await활용
        //async를 활용한 함수는 await를 이용하여 태스크에서 호출되는 함수가 결과를 반환할 때까지 대기함(태스크 블록 없이)
        //async 함수 내부에선 await를 많이 활용할 수 있음
        //async, await를 활용하면 각 태스크들이 모두 내부에 statemachine을 갖게 됨, 각각의 상태를 알려주는 것을 품게 됨(실제 컴파일 과정에서)

        async Task<int> CalculateValueAsyncAdvance()
        {
            await Task.Delay(1000);
            return 123;
        }
        

        
        async void ButtonClickAsync()
        {
            int value = await CalculateValueAsyncAdvance();
            Console.WriteLine(value);
            
            await Task.Delay(1000);
            using(var wc = new WebClient())
            {
                string data = await wc.DownloadStringTaskAsync("http://google.com/robots.txt");
                Console.WriteLine(data.Split('\n')[0].Trim());
            }
        }

        #endregion
        #region Task.Run
        //Task.Run함수는 Task.Factory.StartNew함수와 같은 일을 처리함 그리고 그 후에 UnWrap함수를 호출함 : 훨씬 간편..
        //추가적인 이점으로 비동기 함수도 Task.Run함수로 호출할 수 있음...?
        void CheckFactoryStartNew()
        {
            //아래와 같은 경우는 Task내부에서 Task를 생성하여 호출하여 그 결과를 반환하면 : Task<Task>가 됨 우리가 원하지 않는 결과
            var t = Task.Factory.StartNew(() =>
            {
                Task inner = Task.Factory.StartNew(() => { });
                return inner;
            });
        }
        void CheckFactoryStartNewAsync()
        {
            //아래와 같이 되면...? : Task<Task<int>>의형태로 되돌려 주게 됨.. 우리가 원하지 않는 결과
            var t = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                return 123;
            });
        }
        void CheckFactoryStartNewUnwrap()
        {
            //아래처럼 처리하게 되면.. Task<int>형태로 얻게 됨
            var t = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                return 123;
            }).Unwrap();
        }
        async void CheckTaskRun()
        {
            int result = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                return 42;
            });
        }
        #endregion
        #region TaskUtility Combinators
        //Task.WaitAll, WaitAny를 쓰면 Task 완료될 때까지 대기할 수 있으나.. 완료될 때까지 다른 일을 처리할 수 없게 됨
        //저것들을 모두 완료하기를 기다리는 Task가 추가로 필요하게 됨
        async void CheckTaskWhenAny()
        {
            //WhenAny로 제공된 함수 중 하나라도 완료될 때까지 대기하는 태스크를 생성하여 완료를 대기함
            await Task.WhenAny(Task.Run(async () => 
            {
                await Task.Delay(1000);
                Console.WriteLine("download from http");
            }), Task.Run(async () => {
                await Task.Delay(1000);
                Console.WriteLine("download from ftp");
            }));
        }
        async void CheckTaskWhenAll()
        {
            //WhenAll로 제공된 함수가 모두 완료될 때까지 대기하는 태스크를 생성하여 완료를 대기
            await Task.WhenAll(Task.Run(async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine("온도 측정");
            }), Task.Run(async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine("압력 측정");
            }));
        }
        #endregion
    }
}
