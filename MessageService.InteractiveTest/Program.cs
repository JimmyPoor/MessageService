using MessageService.InteractiveTest;
using System;

namespace MessageService.Test
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            new Interactive_Routing_Diff_Endpoint_Test();
            Console.WriteLine("not waiting");
            Console.Read();
        }
    }
}
