using System;
using System.Collections.Generic;

public enum OrderType { Bar, Kitchen };
public enum OrderStatus { Pending, Prepararion, Ready };
public enum Operation { New, Change };

[Serializable]
public class Table
{
    private static int LastId = 0;

    public int Id { get; }
    public bool Occupied { get; set; }
    public IList<Order> Orders { get; set; }

    public Table()
    {
        Id = LastId++;
        Occupied = false;
        Orders = new List<Order> { };
    }

    public int getTotalPrice()
    {
        int sum = 0;
        foreach (Order o in Orders) sum += Convert.ToInt32(o.Item.Price);
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
    public OrderStatus NextStatus { get { if (Status < OrderStatus.Ready) return Status + 1; else return OrderStatus.Ready; } }

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

public interface IOrdersList
{
    event AlterDelegate alterEvent;

    List<Item> getMenuItems();
    bool addOrder(int tableID, int itemId, int quantity);
    List<Order> getOrders();
    void changeOrderStatus(Order o, OrderStatus newOS);
    void consultTable(int id);
    void printTables();
    void printOrders();
}

/* Classe para subscrição dos eventos */
public class AlterEventRepeater : MarshalByRefObject
{
    public event AlterDelegate alterEvent;

    public override object InitializeLifetimeService() { return null; }

    public void Repeater(Operation op, Order order) { if (alterEvent != null) alterEvent(op, order); }
}
