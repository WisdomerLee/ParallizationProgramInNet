using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ParallelLinq
{
    internal class CustomAggregationDemoSample
    {
        void Main()
        {
            //총 합을 구하기
            var sum = Enumerable.Range(1, 1000).Sum();

            Console.WriteLine($"sum = {sum}");
            //또 다른 방법으로 Aggregate함수를 이용
            //아래의 Aggregate함수는 2개의 변수를 받아 처리하였음
            //첫번째 변수: 초기 값, 두번째 변수: 함수
            //두번째 함수의 변수로 들어간 첫번째 변수: Enumerable의 각각 돌려받는 값들 : 아래의 예시에서는 int값이 1부터 1000까지 
            //두번째 함수의 두번째 변수 : 함수의 계산된 값이 누적되는 값
            //아래의 함수는 1부터 1000까지 돌아가면서 함수의 누적된 값을 더하는 형태로 진행되므로 총합을 구하는 함수와 같은 결과를 얻게 됨

            var sum2 = Enumerable.Range(1, 1000).Aggregate(0, (i, acc) => i+acc);
            //병렬로 처리할 때는 ParallelEnumerable을 씀
            //Aggregate에서 첫번째 변수 : 초기 값, 두번째 변수 : 함수, 세번째 변수 : 함수, 네번째 변수 : 함수
            //두번째 함수 : 반복하면서 진행할 함수 : 아래의 예시에서는 계속 숫자를 더하게 되어있음
            //세번째 함수 : 태스크별로 반복진행된 함수를 합치는 함수 : 아래의 예시에서는 태스크별로 구한 합을 모두 합치는 형태
            //네번째 함수 : 돌려줄 최종 값을 반환하는 함수 : 아래의 경우는 그 더한 값을 돌려주는 형태
            var sumParallel = ParallelEnumerable.Range(1, 1000).Aggregate(
                0, (partialSum, i) => partialSum += i, (total, subtotal) => total += subtotal, i=>i
                );
        }
    }
}
