namespace PromoEngine.Models;

public class Order
{
    public Order(int id, List<OrderDetail> orderDetails)
    {
        Id = id;
        OrderDetails = orderDetails;
    }

    public int Id { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
}