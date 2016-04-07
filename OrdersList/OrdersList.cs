using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class OrdersList : MarshalByRefObject, IOrdersList
{
    private List<Table> Tables;
    private List<Order> Orders;
    private List<Item> MenuItems; //Lista dos produtos do menu
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
        Console.WriteLine("Printer output:\n");
    }

    public void connect() { }

    public List<Item> getMenuItems() { return MenuItems; }

    public int addOrder(int tableID, int itemId, int quantity)
    {
        Table tbl = Tables.ElementAt(tableID);
        if (!Tables[tableID].AllowOrders) return 1;
        if (!tbl.Occupied) return 2;

        //Console.WriteLine("[NEW ORDER] (" + MenuItems[itemId].Name + " | " + quantity + " | " + tableID + " | " + MenuItems[itemId].Type + " | " + MenuItems[itemId].Price + ")");

        tbl.Orders.Add(new Order(MenuItems[itemId], quantity, tbl));
        Orders.Add(tbl.Orders.Last());

        NotifyClients(Operation.New, Orders.Last());
        //Console.WriteLine("<NEW ORDER ACCEPTED>\n");
        return 0;
    }

    public List<Order> getOrders() { return Orders; }

    public void changeOrderStatus(Order o, OrderStatus newOS)
    {
        int i = Orders.FindIndex(x => x.Id == o.Id);
        Orders[i].Status = newOS;

        NotifyClients(Operation.Change, Orders[i]);
    }

    public void deleteOrder(Order o)
    {
        o.Status = OrderStatus.Delivered;
        NotifyClients(Operation.Change, o);
        Tables[o.Table.Id].Orders.Remove(o);
        Orders.Remove(o);
    }

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

    public void assignTable(int id)
    {
        Tables[id].Occupied = true;
    }

    public bool requestBill(int id)
    {
        if (!Tables[id].Occupied) return false;
        Tables[id].AllowOrders = false;

        List<Order> lo = new List<Order>();

        foreach (Order o in Tables[id].Orders)
        {
            if (o.Status == OrderStatus.Pending)
            {
                lo.Add(o);
                o.Status = OrderStatus.Delivered;
                NotifyClients(Operation.Change, o);
            }
        }

        foreach (Order o in lo)
        {
            Tables[id].Orders.Remove(o);
            Orders.Remove(o);
        }

        consultTable(id);

        return true;
    }

    public bool payTable(int id)
    {
        if (!Tables[id].Occupied || Tables[id].AllowOrders) return false;
        Tables[id].Occupied = false;
        Tables[id].AllowOrders = true;

        for(int i=0; i< Orders.Count; i++)
        {
            if(Orders[i].Table.Id == id) Orders.RemoveAt(i);
        }
        Tables[id].Orders.Clear();

        return true;
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
