namespace PromoEngine.Models;

public class StockKeepingUnit
{
    public StockKeepingUnit(string id, decimal unitPrice)
    {
        Id = id;
        UnitPrice = unitPrice;
    }

    public string Id { get; set; }
    public decimal UnitPrice { get; set; }
}