using System.Collections.Generic;
using Xunit;
using PromoEngine;
using PromoEngine.Models;

namespace PromoEngineTests
{
    public class PromoEngineTests
    {
        private Engine _engine;
        public PromoEngineTests()
        {
            _engine = new Engine();
        }

        [Fact]
        public void OrderWithoutOrderDetails_ReturnsZero()
        {
            Order order = new Order(1, new List<OrderDetail>());
            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, null);
            Assert.Equal(0, invoiceAmount);
        }

        [Fact]
        public void OrderWithoutPromotion_ReturnsActualOrderAmount()
        {
            Order order = new Order(1, new List<OrderDetail>() { new(1, new StockKeepingUnit("A", 50)) });

            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, null);
            Assert.Equal(50, invoiceAmount);
        }

        [Fact]
        public void OrderWithPromotionNoPromotionApplied_ReturnsActualOrderAmount()
        {
            Order order = new Order(1, new List<OrderDetail>() { new(1, new StockKeepingUnit("A", 50)) });
            Promotion promotion = new Promotion(1, PromotionType.Quantity, new List<string> { "A" }, 3, 130);

            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, new List<Promotion> { promotion });
            Assert.Equal(50, invoiceAmount);
        }
    }
}