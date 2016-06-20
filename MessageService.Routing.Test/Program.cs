using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Routing.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Router_Test test = new Router_Test();
            test.Setup();
            // test.Route_Setup_when_table_null_Test();
            test.Route_Multi_Mapping_with_diff_message_type_Test();
        }
    }
}
