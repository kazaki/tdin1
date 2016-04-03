using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("Server.exe.config", false);

            //Obtem o obj do server
            OrdersList orderManager = (OrdersList)Activator.GetObject(typeof(OrdersList), "tcp://localhost:9000/Server/RestaurantServer");

            Console.WriteLine("printTables");
            orderManager.printTables();
            Console.ReadLine();

            Console.WriteLine("printOrders");
            orderManager.printOrders();
            Console.ReadLine();

            Console.WriteLine("printTables");
            orderManager.printTables();
            Console.ReadLine();

            Console.WriteLine("consultTable(0)");
            orderManager.consultTable(0);
            Console.ReadLine();

        }
    }
}
