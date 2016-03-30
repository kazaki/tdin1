using System;
using System.Collections.Generic;
using System.Linq;
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




            RestaurantHandler restaurant = new RestaurantHandler();

            restaurant.printTables();

            Console.ReadLine();

            restaurant.addOrder(0, "Sandes de Fiambre", 1, Order.BAR, 10);
            restaurant.addOrder(0, "Sumo de Laranja", 1, Order.BAR, 10);

            restaurant.printOrders();

            Console.ReadLine();

            restaurant.printTables();

            Console.ReadLine();

            restaurant.consultTable(0);

            Console.ReadLine();

        }



    }

    
}
