using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        foreach (Order o in Orders) sum += o.Price;
        return sum;
    }
}

public class Order
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

public interface IOrdersList
{
    bool addOrder(int tableID, string description, int quantity, int type, int price);
    void consultTable(int id);
    void printTables();
    void printOrders();
}