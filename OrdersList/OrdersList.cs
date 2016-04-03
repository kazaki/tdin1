using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class OrdersList : MarshalByRefObject, IOrdersList
{
    private IList<Table> Tables;
    private IList<Order> Orders;

    public OrdersList()
    {
        Tables = new List<Table> { new Table(), new Table(), new Table(), new Table(), new Table(), new Table() };
        Orders = new List<Order> { };
    }

    public bool addOrder(int tableID, string description, int quantity, int type, int price)
    {

        Console.WriteLine("[NEW ORDER] (" + description + " | " + quantity + " | " + tableID + " | " + type + " | " + price + ")");

        Table tbl = Tables.ElementAt(tableID);

        Orders.Add(new Order(description, quantity, tbl, type, price));
        tbl.Orders.Add(Orders.Last());
        tbl.Occupied = true;
        Console.WriteLine("<NEW ORDER ACCEPTED>\n");
        return true;
    }

    public void consultTable(int id)
    {
        Table tbl = Tables.ElementAt(id);
        Console.WriteLine("[Table " + id + "'s orders] " + tbl.getTotalPrice() + " euro(s)");
        foreach (Order o in tbl.Orders)
        {
            Console.WriteLine("[" + o.Id + "] (" + o.Description + " | " + o.Quantity + " | " + o.Type + ") " + o.Price + " euro(s)");
        }
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
