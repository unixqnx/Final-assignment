using System.Collections.Generic;

namespace OrderItemsReserver.Dto;

public class Order
{
    public string id { get; set; }

    public int Id { get; set; }

    public Address ShipToAddress { get; set; }

    public List<OrderItem> OrderItems { get; set; }

    public decimal Total
    {
        get {
            var total = 0m;
            foreach (var item in OrderItems)
            {
                total += item.UnitPrice * item.Units;
            }
            return total;
        }
    }
}
