using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class OrdersList : MarshalByRefObject, IOrdersList
{
    private IList<Table> Tables;
    private IList<Order> Orders;
    private IList<Item> MenuItems; //Lista dos produtos do menu
    public event AlterDelegate alterEvent;

    public OrdersList()
    {
        Tables = new List<Table> { new Table(), new Table(), new Table(), new Table(), new Table(), new Table() };
        Orders = new List<Order> { };
        MenuItems = new List<Item> {
            new Item(0, "Sandes de Fiambre", 2.5m , OrderType.Bar),
            new Item(1, "Sumo de Laranja", 2.5m , OrderType.Bar),
            new Item(2, "Tosta Mista", 2.5m , OrderType.Bar),
            new Item(3, "Omelete Mista", 2.5m , OrderType.Kitchen),
            new Item(4, "Prego no Prato", 2.5m , OrderType.Kitchen),
            new Item(5, "Filetes de Pescada", 2.5m , OrderType.Kitchen),
        };
    }

    public IList<Item> getMenuItems() { return MenuItems; }

    public bool addOrder(int tableID, int itemId, int quantity)
    {
        Console.WriteLine("[NEW ORDER] (" + MenuItems[itemId].Name + " | " + quantity + " | " + tableID + " | " + MenuItems[itemId].Type + " | " + MenuItems[itemId].Price + ")");

        Table tbl = Tables.ElementAt(tableID);

        Orders.Add(new Order(MenuItems[itemId], quantity, tbl));
        tbl.Orders.Add(Orders.Last());
        tbl.Occupied = true;

        NotifyClients(Operation.New, Orders.Last());
        Console.WriteLine("<NEW ORDER ACCEPTED>\n");
        return true;
    }

    public IList<Order> getOrders() { return Orders; }

    public void consultTable(int id)
    {
        Table tbl = Tables.ElementAt(id);
        Console.WriteLine("[Table " + id + "'s orders] " + tbl.getTotalPrice() + " euro(s)");
        foreach (Order o in tbl.Orders)
        {
            Console.WriteLine("[" + o.Id + "] (" + o.Item.Name + " | " + o.Quantity + " | " + o.Item.Type + ") " + o.Item.Price + " euro(s)");
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
        Console.WriteLine("All Orders:" + Orders.Count);
        foreach (var o in Orders)
        {
            Console.WriteLine("[" + o.Id + "]");
            Console.WriteLine("  Description: " + o.Item.Name);
            Console.WriteLine("  Quantity: " + o.Quantity);
            Console.WriteLine("  TableId: " + o.Table.Id);
            Console.WriteLine("  Type: " + o.Item.Type);
            Console.WriteLine("  Price: " + o.Item.Price);
            Console.WriteLine("  Status: " + o.Status);
            Console.WriteLine();
        }
    }

    /* Notifica os clientes que subscreveram o evento das novas orders */
    void NotifyClients(Operation op, Order order)
    {
        if (alterEvent != null)
        {
            Delegate[] invkList = alterEvent.GetInvocationList();

            foreach (AlterDelegate handler in invkList)
            {
                new Thread(() =>
                {
                    try
                    {
                        handler(op, order); //Invoking event handler
                    }
                    catch (Exception)
                    {
                        alterEvent -= handler; //Exception: Removed an event handler
                    }
                }).Start();
            }
        }
    }
}
