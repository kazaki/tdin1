using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;

public enum OrderType { Bar, Kitchen };
public enum OrderStatus { Pending, Prepararion, Ready, Delivered };
public enum Operation { New, Change };

[Serializable]
public class Table
{
    private static int LastId = 0;

    public int Id { get; }
    public bool Occupied { get; set; }
    public IList<Order> Orders { get; set; }
    public bool AllowOrders { get; set; }

    public Table()
    {
        Id = LastId++;
        Occupied = false;
        AllowOrders = true;
        Orders = new List<Order> { };
    }

    public Decimal getTotalPrice()
    {
        Decimal sum = 0;
        foreach (Order o in Orders) sum += o.Item.Price * o.Quantity;
        return sum;
    }
}

[Serializable]
public class Order
{
    private static int LastId = 0;
    public int Id { get; set; }
    public Item Item { get; set; }
    public int Quantity { get; set; }
    public Table Table { get; set; }
    public OrderStatus Status { get; set; } // 0 - Pending, 1 - Preparation, 2 - Ready

    public Order(Item item, int quantity, Table table)
    {
        Id = LastId++;
        Item = item;
        Quantity = quantity;
        Table = table;
        Status = OrderStatus.Pending;
    }

    public override bool Equals(object other)
    {
        if (other == null) return false;
        return ((Order)other).Id == Id;
    }

    /* Estes 2 métodos foram necessários, por a classe Order ser um "objeto complexo", para mostrar os dados fazendo binding da classe Order nos dataGridView */
    public string StringItem { get { return Item.Name; } }
    public string StringTable { get { return (Table.Id + 1).ToString(); } }
    public string StringType { get { return Item.Type.ToString(); } }
    public OrderStatus NextStatus { get { if (Status < OrderStatus.Ready) return Status + 1; else return OrderStatus.Ready; } }
    public String NextStatusBt { get { if (Status < OrderStatus.Ready) return "> " + (Status + 1); else return OrderStatus.Ready.ToString(); } }

}

[Serializable]
public class Item
{
    public int Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public OrderType Type { get; }

    public Item(int id, string name, decimal price, OrderType type)
    {
        this.Id = id;
        this.Name = name;
        this.Price = price;
        this.Type = type;
    }
}

public delegate void AlterDelegate(Operation op, Order order);
public delegate void AlterDelegateBar(Operation op, Order order);
public delegate void AlterDelegateKitchen(Operation op, Order order);

public interface IOrdersList
{
    event AlterDelegate alterEvent;
    event AlterDelegateBar alterEventBar;
    event AlterDelegateKitchen alterEventKitchen;

    void connect();
    List<Item> getMenuItems();
    int addOrder(int tableID, int itemId, int quantity);
    List<Order> getOrders();
    List<Order> getOrdersBar();
    List<Order> getOrdersKitchen();
    void changeOrderStatus(Order o, OrderStatus newOS);
    void deleteOrder(Order o);
    void consultTable(int id);
    void printTables();
    void printOrders();
    void assignTable(int id);
    bool payTable(int id);
    bool requestBill(int id);
}

/* Classe para subscrição dos eventos */
public class AlterEventRepeater : MarshalByRefObject
{
    public event AlterDelegate alterEvent;
    public event AlterDelegateBar alterEventBar;
    public event AlterDelegateKitchen alterEventKitchen;

    public override object InitializeLifetimeService() { return null; }

    public void Repeater(Operation op, Order order) { if (alterEvent != null) alterEvent(op, order); }
    public void RepeaterBar(Operation op, Order order) { if (alterEventBar != null) alterEventBar(op, order); }
    public void RepeaterKitchen(Operation op, Order order) { if (alterEventKitchen != null) alterEventKitchen(op, order); }
}

/* Mechanism for instanciating a remote object through its interface, using the config file */
public class RemoteNew
{
    private static Hashtable types = null;

    private static void InitTypeTable()
    {
        types = new Hashtable();
        foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
            types.Add(entry.ObjectType, entry);
    }

    public static object New(Type type)
    {
        if (types == null)
            InitTypeTable();
        WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)types[type];
        if (entry == null)
            throw new RemotingException("Type not found!");
        return RemotingServices.Connect(type, entry.ObjectUrl);
    }
}