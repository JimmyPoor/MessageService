using MessageService.Test.Core;
using MessageService.Test.Core.Middleware;
using System;

namespace MessageService.Test
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            var test = new ServiceBusTest();
            test.Setup();
            test.Bus_Start_Stop_with_Components();
            //test.Bus_bind_test();
            //test.MultiMessage_with_same_invoker_test();
            //test.Send_Message_with_full_paras_and_single_thread();
            // test.Receive_Message() ;
            // test.Endpoint_send_test();
            // test.Send_Message_with_full_paras_and_mulit_thread();
            // test.Receive_message_test();
           // test.Validate_contract_test();
            Console.WriteLine("not waiting");
            Console.Read();
        }
    }
}
