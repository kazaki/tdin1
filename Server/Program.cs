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

            restaurant.addOrder(0, "Pedido exemplo", 10, Order.BAR, 20);

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

        public bool addOrder(int tableID, string description, int quantity, int type, int price)
        {

            Console.WriteLine("[NEW ORDER] (" + description + " | " + quantity + " | " + tableID + " | " + type + " | " + price + ")");

            Table tbl = Tables.ElementAt(tableID);
            if (tbl.Occupied == true)
            {
                Console.WriteLine("<NEW ORDER DENIED> Table occupied. \n");
                return true;
            }
            
            Orders.Add(new Order(description, quantity, tbl, type, price));
            tbl.Orders.Add(Orders.Last());
            tbl.Occupied = true;
            Console.WriteLine("<NEW ORDER ACCEPTED>\n");
            return true;
        }

        public void consultTable(int id)
        {

        }

        public void printTables()
        {
            Console.WriteLine("All Tables:");
            foreach (var t in Tables)
            {
                string status = "FREE";
                if (t.Occupied) status = "OCCUPIED";
                Console.WriteLine("[" + t.Id + "] <" + status + ">");
            }
        }

        public void printOrders()
        {
            Console.WriteLine("All Orders:");
            foreach (var o in Orders)
            {
                Console.WriteLine("[" + o.Id + "]");
                Console.WriteLine("  Description: " + o.Description);
                Console.WriteLine("  Quantity: " + o.Quantity);
                Console.WriteLine("  TableId: " + o.Table.Id);
                Console.WriteLine("  Type: " + o.Type);
                Console.WriteLine("  Price: " + o.Price);
                Console.WriteLine("  Status: " + o.Status);
                Console.WriteLine();
            }
        }
    }

    class Table
    {
        private static int LastId = 0;

        public int Id { get; }
        public bool Occupied { get; set; }
        public IList<Order> Orders { get; set; }

        public Table()
        {
            Id = LastId++;
            Occupied = false;
            Orders = new List<Order> {};
        }
    }

    class Order
    {
        public const int BAR = 0, KITCHEN = 1, PENDING = 0, PREPERATION = 1, READY = 2;

        private static int LastId = 0;

        public int Id { get; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public Table Table { get; set; }

        public int Type { get; set; } // 0 - Bar, 1 - Kitchen

        public int Price { get; set; }

        public int Status { get; set; } // 0 - Pending, 1 - Preparation, 2 - Ready

        public Order(string description, int quantity, Table table, int type, int price)
        {
            Id = LastId++;
            Description = description;
            Quantity = quantity;
            Table = table;
            Type = type;
            Price = price;
            Status = 0;
        }

    }
}
