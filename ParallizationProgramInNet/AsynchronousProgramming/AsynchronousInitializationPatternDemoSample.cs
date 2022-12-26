using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.AsynchronousProgramming
{
    //Factory 생성 패턴 말고 다른 방식으로도 할 수 있음
    public interface IAsyncInit
    {
        Task InitTask { get; }
    }
    class DemoClass : IAsyncInit
    {
        public DemoClass() 
        {
            InitTask = InitAsync();
        }
        async Task InitAsync()
        {
            await Task.Delay(1000);

        }
        public Task InitTask
        {
            get;
        }
    }
    class DemoOtherClass : IAsyncInit
    {
        readonly DemoClass demoClass;
        public DemoOtherClass(DemoClass demoClass)
        {
            this.demoClass = demoClass;
            InitTask = InitAsync();
        }
        async Task InitAsync()
        {
            if(demoClass is IAsyncInit ai)
            {
                await ai.InitTask;
            }

            await Task.Delay(1000);

        }
        public Task InitTask
        {
            get;
        }
    }
    internal class AsynchronousInitializationPatternDemoSample
    {
        async void Main()
        {
            var demo = new DemoClass();
            //if(demo is IAsyncInit ai)
            //{
            //    await ai.InitTask;
            //}
            var demoOther = new DemoOtherClass(demo);
            await demoOther.InitTask;
        }
    }
}
