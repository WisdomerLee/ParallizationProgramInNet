using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.ConcurrentCollections
{
    internal class ConcurrentDictionarySampleDemo
    {
        ConcurrentDictionary<string, string> capitals = new ConcurrentDictionary<string, string>();

        void AddParis()
        {
            //일반 Add대신 TryAdd는 불리언을 통해 이미 키값을 갖고 있으면 false를 반환하고 값을 더하지 않고 없을 경우 true를 반환하며 키, 값을 더하게 됨
            bool success = capitals.TryAdd("France", "Paris");
            string who = Task.CurrentId.HasValue ? ("Task" + Task.CurrentId): "Main thread";
            Console.WriteLine($"{who} {(success ? "added" : "did not add")} the element.");
        }
        void Main()
        {
            Task.Factory.StartNew(AddParis).Wait();
            AddParis();

            //아래와 같은 형태로도 더하고 업데이트 하면 되지만..
            capitals["Russia"] = "Leingrad";
            capitals["Russia"] = "Moscow";
            Console.WriteLine(capitals["Russia"]);
            //아래의 함수는 키 값이 없으면 더하고 있으면 해당 키의 값을 업데이트 하게 됨, 업데이트 하게 될 함수를 지정해야 함..
            capitals.AddOrUpdate("Russia", "Moscow", (k, old) => old + "--> Moscow");
            Console.WriteLine($"The capial of Russia is {capitals["Russia"]}");

            capitals["Sweden"] = "Uppsala";
            var capofSweden = capitals.GetOrAdd("Sweden", "Stockholm");
            Console.WriteLine($"The capital of Sweden is {capofSweden}");
            //TryRemove : 키 값있으면 없애고 없앤 값을 돌려주게 됨
            var didRemove = capitals.TryRemove("Russia", out string removed);
            if (didRemove)
            {
                Console.WriteLine($"We just removed {removed}");
            }
            else
            {
                Console.WriteLine("Failed to remove the capital of Russia");
            }
            //ConcurrentDictionary의 Count를 구하는 것은 리소스를 많이 소모하는 것이므로 되도록 적게 호출하도록 할 것
            
            foreach(var cap in capitals)
            {
                Console.WriteLine($" - {cap.Value} is the capital of {cap.Key}");
            }

        }
    }
}
