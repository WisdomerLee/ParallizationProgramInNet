using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.AsynchronousProgramming
{
    class Stuff
    {
        static int value;
        readonly Lazy<Task<int>> AutoIncValue = new Lazy<Task<int>>(async () =>
        {
            await Task.Delay(1000).ConfigureAwait(false);
            return value++;
        });
        readonly Lazy<Task<int>> AutoIncValue2 = new Lazy<Task<int>>(() => Task.Run(async()=>
        {
            await Task.Delay(1000).ConfigureAwait(false);
            return value++;
        }));
        //Nito.AsyncEx
        //readonly AsyncLazy<int> AutoIncValue3 = new AsyncLazy<int>(async () =>
        //{
        //    await Task.Delay(1000);
        //    return value++;
        //});


        public async void UseValue()
        {
            int value = await AutoIncValue.Value;
        }
        #region ValueTask
        ///<summary>
        ///Task : 어느 기능이 반드시 완료될 것이라고 약속된 것
        ///처음에 시작하는 함수가 있음
        ///기능이 완료되면 태스크가 종료됨
        ///진행을 잠시 막고 결과를 기다리는 것
        ///Task가 많이 생성되면 GC에 부하가 걸림 : Task자체도 클래스이므로
        /// </summary>
        


        
        async void ValueTask()
        {
            AsyncOperationFunctionSample().ContinueWith(task =>
            {
                try
                {
                    var result = task.Result;
                    UseResult(result);
                }
                catch(Exception ex)
                {

                }
            });
            //위와 같은 것을.. 아래와 같은 형태로 바꿀 수 있음!! : 코드가 단순해짐

            var result = await AsyncOperationFunctionSample();
            //await는 같은 함수에서 여러번 쓰일 수 있음 : 여러 상태를 대기할 수 있음
            //
            UseResult(result);
        }
        async Task<int> AsyncOperationFunctionSample()
        {
            await Task.Delay(1000);
            return 5;
        }
        void UseResult(int value)
        {
            Console.WriteLine(value);
        }
        ///<summary>
        ///MemoryStream.ReadAsync : .netCore 2.1이후
        ///ValueTask<T> : struct 꼴
        ///미리 준비된 task들은 ValueTasks보다 대기 및 반응이 빠름
        ///ValueTask/ValueTask<T> : Api사용자가 해당 함수를 직접적으로 대기할 경우, allocation-realated overhead를 방지할 때
        ///</summary>
        
        ValueTask<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                int bytesRead = Read(buffer, offset, count);
                return new ValueTask<int>(bytesRead);
            }
            catch (Exception ex)
            {
                return new ValueTask<int>(Task.FromException<int>(ex));
            }
        }
        int Read(byte[] buffer, int offset, int count)
        {

            return 400;
        }
        #endregion
    }
    internal class AsyncchronousLazyInitializationDemoSample
    {
        void Main()
        {

        }
    }
}
