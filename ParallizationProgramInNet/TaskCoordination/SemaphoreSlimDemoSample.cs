using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskCoordination
{
    internal class SemaphoreSlimDemoSample
    {
        
        void Main()
        {
            //세마포어는 생성자에 들어가는 첫번째 변수는 배리어, 카운트다운과 비슷하게 연산이 처리될 때 참여하는 함수 갯수 혹은 Signal이 쓰이는 갯수
            //두번째 변수는 최대 참여 가능한 숫자.
            //아래의 예시는 최소 2개로 시작하여 10개까지 연산이 참여할 수 있음
            var semaphore = new SemaphoreSlim(2, 10);

            for(int i = 0; i< 20; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Entering task {Task.CurrentId}");
                    semaphore.Wait();//세마포어에 카운트를 날림, 숫자를 줄이게 됨 ReleaseCount--;
                    Console.WriteLine($"Processing task {Task.CurrentId}");
                });
            }

            while(semaphore.CurrentCount <= 2)
            {
                Console.WriteLine($"Semaphore count: {semaphore.CurrentCount}");
                Console.ReadKey();
                semaphore.Release(2); //세마포어에 카운트를 날림, 숫자를 증가 ReleaseCount+=2 Release에 넣은 숫자만큼...

            }
        }
    }
}
