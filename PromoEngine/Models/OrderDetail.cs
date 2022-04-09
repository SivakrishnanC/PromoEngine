namespace PromoEngine.Models;

public class OrderDetail
{
    public OrderDetail(int quantity, StockKeepingUnit stockKeepingUnit)
    {
        Quantity = quantity;
        StockKeepingUnit = stockKeepingUnit;
    }

    public int Id { get; set; }
    public StockKeepingUnit StockKeepingUnit { get; set; }
    public int Quantity { get; set; }
}