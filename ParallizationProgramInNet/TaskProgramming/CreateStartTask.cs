using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.TaskProgramming
{
    internal class CreateStartTask
    {
        void Write(char c)
        {
            int i = 1000;
            while(i-- > 0)
            {
                Console.Write(c);
            }
        }

        void Write(object o)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(o);
            }
        }

        int TextLength(object o)
        {
            Console.WriteLine($"\nTask with id{Task.CurrentId} processing object {o}");
            return o.ToString().Length;
        }
        void Main()
        {
            string text1 = "testing", text2 = "this";
            var task1 = new Task<int>(TextLength, text1);
            task1.Start();
            Task<int> task2 = Task.Factory.StartNew(TextLength, text2);

            Console.WriteLine($"Length of '{text1}' is {task1.Result}");
            Console.WriteLine($"Length of '{text2}' is {task2.Result}");
            //Task t = new Task(Write, "hello");
            //t.Start();
            //Task.Factory.StartNew(Write, 123);


            //Write('-');
        }
    }
}
