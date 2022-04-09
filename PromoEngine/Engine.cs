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
                switch (promotion.Type)
                {
                    case PromotionType.Quantity:
                        total += CalculateQuantityBasedPromotion(order, promotion);
                        break;
                    case PromotionType.Combination:
                        total += CalculateCombinationBasedPromotion(order, promotion);
                        break;
                }
            }
        }

        total += order.OrderDetails.Where(x => !x.IsPromotionApplied).Sum(x => x.Quantity * x.StockKeepingUnit.UnitPrice);

        return total;
    }

    private decimal CalculateQuantityBasedPromotion(Order order, Promotion promotion)
    {
        decimal total = 0;

        foreach (OrderDetail orderDetail in order.OrderDetails.Where(x => x.IsPromotionApplied == false))
        {
            if (promotion.StockKeepingUnitIds.Contains(orderDetail.StockKeepingUnit.Id) &&
                orderDetail.Quantity >= promotion.Quantity)
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

        return total;
    }

    private decimal CalculateCombinationBasedPromotion(Order order, Promotion promotion)
    {
        decimal total = 0;

        if (IsPromotionCanBeApplied(order, promotion))
        {
            List<OrderDetail> applicableOrderDetails = order.OrderDetails
                .Where(x => promotion.StockKeepingUnitIds.Contains(x.StockKeepingUnit.Id)).ToList();

            //calculate the number of times combination promotion can be applied
            int promotionApplicableTimes = applicableOrderDetails.Min(x => x.Quantity);

            //apply the combination promotion
            applicableOrderDetails.ForEach(x => { x.IsPromotionApplied = true; });

            //calculate the price for the combination applicable times
            total += promotionApplicableTimes * promotion.Price;

            //add the actual amount remaining items
            total += applicableOrderDetails
                .Where(x => x.Quantity - promotionApplicableTimes > 0)
                .Sum(x => (x.Quantity - promotionApplicableTimes) * x.StockKeepingUnit.UnitPrice);
        }

        return total;
    }

    private static bool IsPromotionCanBeApplied(Order order, Promotion promotion)
    {
        return promotion.StockKeepingUnitIds.All(pSkuId =>
            order.OrderDetails.Select(oi => oi.StockKeepingUnit.Id).Contains(pSkuId));
    }
}