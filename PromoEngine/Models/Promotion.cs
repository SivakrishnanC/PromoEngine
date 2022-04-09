using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoEngine.Models;

public class Promotion
{
    public Promotion(int id, PromotionType type, List<string> stockKeepingUnitIds, int quantity, decimal price)
    {
        Id = id;
        Type = type;
        StockKeepingUnitIds = stockKeepingUnitIds;
        Quantity = quantity;
        Price = price;
    }

    public int Id { get; set; }

    public PromotionType Type { get; set; }

    public List<string> StockKeepingUnitIds { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}

public enum PromotionType
{
    Quantity,
    Combination
}