using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.AsynchronousProgramming
{
    //만약 생성될 때 홈페이지에서 다운받아 그 값을 토대로 클래스가 초기화되어야 한다고 가정하면..
    //Factory 생성 패턴을 활용하여 async 방식으로 호출할 수 있음..
    class HomPage
    {
        //기본 생성자
        HomPage() 
        {
            
        }
        async Task<HomPage> InitAsync()
        {
            //시간이 걸리는 연산, 다운로드 등
            await Task.Delay(1000);
            return this;
        }
        public static Task<HomPage> CreateAsync()
        {
            var result = new HomPage();
            return result.InitAsync();
        }
    }
    internal class AsyncFactoryMethodDemoSample
    {
        //async는 언제나 호출될 수 있는 것이 아님

        async void Main()
        {
            //var hom = new HomPage();
            //await hom.InitAsync();
            var x = await HomPage.CreateAsync();
            
        }
    }
}
