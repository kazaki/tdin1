using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    class Program
    {
        static void Main(string[] args)
        {
            RestaurantHandler restaurant = new RestaurantHandler();

            restaurant.printTables();

            Console.ReadLine();

            restaurant.addOrder("Pedido exemplo", 10, 0, Order.BAR, 20);

            restaurant.printOrders();

            Console.ReadLine();

            restaurant.printTables();

            Console.ReadLine();
        }
    }

    class RestaurantHandler
    {
        private IList<Table> Tables;
        private IList<Order> Orders;

        public RestaurantHandler()
        {
            Tables = new List<Table> { new Table(), new Table(), new Table(), new Table(), new Table(), new Table() };
            Orders = new List<Order> {};
        }

        public bool addOrder(string description, int quantity, int tableID, int type, int price)
        {

            Console.WriteLine("[NEW ORDER]");
            Console.WriteLine("\tDescription: " + description);
            Console.WriteLine("\tQuantity: " + quantity);
            Console.WriteLine("\tTable ID: " + tableID);
            Console.WriteLine("\tType: " + type);
            Console.WriteLine("\tPrice: " + price);

            Table tbl = Tables.ElementAt(tableID);
            if (tbl.Occupied == true)
            {
                Console.WriteLine("<NEW ORDER DENIED> Table occupied. ");
                return true;
            }
            
            Orders.Add(new Order(description, quantity, tableID, type, price));
            tbl.Occupied = true;
            Console.WriteLine("<NEW ORDER ACCEPTED>");
            return true;
        }

        public void printTables()
        {
            foreach (var t in Tables)
            {
                string status = "FREE";
                if (t.Occupied) status = "OCCUPIED";
                Console.WriteLine("[" + t.Id + "] <" + status + ">");
            }
        }

        public void printOrders()
        {
            Console.WriteLine("Orders:");
            foreach (var o in Orders)
            {
                Console.WriteLine("[" + o.Id + "]");
                Console.WriteLine("\tDescription: " + o.Description);
                Console.WriteLine("\tQuantity: " + o.Quantity);
                Console.WriteLine("\tTableId: " + o.TableId);
                Console.WriteLine("\tType: " + o.Type);
                Console.WriteLine("\tPrice: " + o.Price);
                Console.WriteLine("\tStatus: " + o.Status);
            }
        }
    }

    class Table
    {
        private static int LastId = 0;

        public int Id { get; }
        public bool Occupied { get; set; }

        public Table()
        {
            Id = LastId++;
            Occupied = false;
        }
    }

    class Order
    {
        public const int BAR = 0, KITCHEN = 1, PENDING = 0, PREPERATION = 1, READY = 2;

        private static int LastId = 0;

        public int Id { get; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public int TableId { get; }

        public int Type { get; set; } // 0 - Bar, 1 - Kitchen

        public int Price { get; set; }

        public int Status { get; set; } // 0 - Pending, 1 - Preparation, 2 - Ready

        public Order(string description, int quantity, int tableID, int type, int price)
        {
            Id = LastId++;
            Description = description;
            Quantity = quantity;
            TableId = tableID;
            Type = type;
            Price = price;
            Status = 0;
        }

    }
}
