using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PromoEngine.Models;

namespace PromoEngine;

public class Engine
{
    public decimal CalculateInvoiceAmount(Order order, List<Promotion>? promotions)
    {
        decimal total = 0;

        if (promotions != null && promotions.Any())
        {
            foreach (Promotion promotion in promotions)
            {
                if (order.OrderDetails.Any())
                {
                    total += order.OrderDetails.Sum(x => x.Quantity * x.StockKeepingUnit.UnitPrice);
                }
            }
        }
        else
        {
            if (order.OrderDetails.Any())
            {
                total += order.OrderDetails.Sum(x => x.Quantity * x.StockKeepingUnit.UnitPrice);
            }
        }

        return total;
    }
}