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
                if (promotion.Type == PromotionType.Quantity)
                {
                    foreach (OrderDetail orderDetail in order.OrderDetails.Where(x => x.IsPromotionApplied == false))
                    {
                        if (promotion.StockKeepingUnitIds.Contains(orderDetail.StockKeepingUnit.Id) && orderDetail.Quantity >= promotion.Quantity)
                        {
                            int remainingQuantity = orderDetail.Quantity;

                            while (remainingQuantity >= promotion.Quantity)
                            {
                                total += promotion.Price;
                                remainingQuantity -= promotion.Quantity;
                            }

                            total += remainingQuantity * orderDetail.StockKeepingUnit.UnitPrice;
                            orderDetail.IsPromotionApplied = true;
                        }
                    }
                }
                else if (promotion.Type == PromotionType.Combination)
                {
                    List<OrderDetail> applicableOrderDetails = order.OrderDetails.Where(x => promotion.StockKeepingUnitIds.Contains(x.StockKeepingUnit.Id)).ToList();
                    int promotionApplicableTimes = applicableOrderDetails.Min(x => x.Quantity);
                    applicableOrderDetails.ForEach(x =>
                    {
                        //x.IsPromotionApplied = true;
                        x.Quantity -= promotionApplicableTimes;
                    });

                    total += promotionApplicableTimes * promotion.Price;
                }
            }

            total += order.OrderDetails.Where(x => !x.IsPromotionApplied).Sum(x => x.Quantity * x.StockKeepingUnit.UnitPrice);
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