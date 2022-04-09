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
            Order order = new Order(1, new List<OrderDetail>() { new(quantity: 1, new StockKeepingUnit("A", 50)) });
            Promotion promotion = new Promotion(1, PromotionType.Quantity, new List<string> { "A" }, 3, 130);

            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, new List<Promotion> { promotion });
            Assert.Equal(50, invoiceAmount);
        }

        [Fact]
        public void OrderWithOrderDetailsAndOneApplicablePromotion_ReturnsPromotionAmount()
        {
            Order order = new Order(1, new List<OrderDetail>() { new(quantity: 3, new StockKeepingUnit("A", 50)) });
            Promotion promotion = new Promotion(1, PromotionType.Quantity, new List<string> { "A" }, 3, 130);

            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, new List<Promotion> { promotion });
            Assert.Equal(130, invoiceAmount);
        }

        [Fact]
        public void OrderWithOrderDetailsQuantityBasedPromotionAppliesMultipleTimes_ReturnsPromotionAmount()
        {
            Order order = new Order(1, new List<OrderDetail>()
            {
                new(quantity: 3, new StockKeepingUnit("A", 50)),
                new(quantity: 2, new StockKeepingUnit("B", 30))
            });
            Promotion promotion1 = new Promotion(1, PromotionType.Quantity, new List<string> { "A" }, 3, 130);
            Promotion promotion2 = new Promotion(1, PromotionType.Quantity, new List<string> { "B" }, 2, 45);

            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, new List<Promotion> { promotion1, promotion2 });
            Assert.Equal(175, invoiceAmount);
        }

        [Fact]
        public void OrderWithOrderDetails_QuantityBasedPromotionAppliesMultipleTimes_WithResidualQuantity_ReturnsPromotionAmount()
        {
            Order order = new Order(1, new List<OrderDetail>()
            {
                new(quantity: 5, new StockKeepingUnit("A", 50)),
                new(quantity: 5, new StockKeepingUnit("B", 30))
            });
            Promotion promotion1 = new Promotion(1, PromotionType.Quantity, new List<string> { "A" }, 3, 130);
            Promotion promotion2 = new Promotion(1, PromotionType.Quantity, new List<string> { "B" }, 2, 45);

            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, new List<Promotion> { promotion1, promotion2 });
            Assert.Equal(350, invoiceAmount);
        }

        [Fact]
        public void OrderWithOrderDetails_CombinationBasedPromotionApplied_ReturnsPromotionAmount()
        {
            Order order = new Order(1, new List<OrderDetail>()
            {
                new(quantity: 1, new StockKeepingUnit("C", 20)),
                new(quantity: 1, new StockKeepingUnit("D", 15))
            });
            Promotion promotion1 = new Promotion(1, PromotionType.Combination, new List<string> { "C", "D" }, 1, 30);
            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, new List<Promotion> { promotion1 });
            Assert.Equal(30, invoiceAmount);
        }

        [Fact]
        public void OrderWithOrderDetails_CombinationBasedPromotionAppliedMultipleTimes_ReturnsPromotionAmount()
        {
            Order order = new Order(1, new List<OrderDetail>()
            {
                new(quantity: 2, new StockKeepingUnit("C", 20)),
                new(quantity: 2, new StockKeepingUnit("D", 15))
            });
            Promotion promotion1 = new Promotion(1, PromotionType.Combination, new List<string> { "C", "D" }, 1, 30);
            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, new List<Promotion> { promotion1 });
            Assert.Equal(60, invoiceAmount);
        }

        [Fact]
        public void OrderWithOrderDetails_CombinationBasedPromotionAppliedMultipleTimes_WithResidualQuantity_ReturnsPromotionAmountPlusResidualAmount()
        {
            Order order = new Order(1, new List<OrderDetail>()
            {
                new(quantity: 3, new StockKeepingUnit("C", 20)),
                new(quantity: 2, new StockKeepingUnit("D", 15))
            });
            Promotion promotion1 = new Promotion(1, PromotionType.Combination, new List<string> { "C", "D" }, 1, 30);
            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, new List<Promotion> { promotion1 });
            Assert.Equal(80, invoiceAmount);

            order = new Order(1, new List<OrderDetail>()
            {
                new(quantity: 2, new StockKeepingUnit("C", 20)),
                new(quantity: 3, new StockKeepingUnit("D", 15))
            });
            promotion1 = new Promotion(1, PromotionType.Combination, new List<string> { "C", "D" }, 1, 30);
            invoiceAmount = _engine.CalculateInvoiceAmount(order, new List<Promotion> { promotion1 });
            Assert.Equal(75, invoiceAmount);
        }

        [Fact]
        public void OrderWithOrderDetails_MultiplePromotionTypeExists_WithoutOneCombinationItem()
        {
            Order order = new Order(1, new List<OrderDetail>()
            {

                new(quantity: 5, new StockKeepingUnit("A", 50)),
                new(quantity: 5, new StockKeepingUnit("B", 30)),
                new(quantity: 1, new StockKeepingUnit("C", 20)),
            });

            List<Promotion> promotions = new List<Promotion>
            {
                new Promotion(1, PromotionType.Quantity, new List<string> {"A"}, 3, 130),
                new Promotion(2, PromotionType.Quantity, new List<string> {"B"}, 2, 45),
                new Promotion(3, PromotionType.Combination, new List<string> {"C", "D"}, 1, 30)
            };

            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, promotions);
            Assert.Equal(370, invoiceAmount);
        }

        [Fact]
        public void OrderWithOrderDetails_MultiplePromotionTypeExists_AppliesAll()
        {
            Order order = new Order(1, new List<OrderDetail>()
            {

                new(quantity: 3, new StockKeepingUnit("A", 50)),
                new(quantity: 5, new StockKeepingUnit("B", 30)),
                new(quantity: 1, new StockKeepingUnit("C", 20)),
                new(quantity: 1, new StockKeepingUnit("D", 15))
            });

            List<Promotion> promotions = new List<Promotion>
            {
                new Promotion(1, PromotionType.Quantity, new List<string> {"A"}, 3, 130),
                new Promotion(2, PromotionType.Quantity, new List<string> {"B"}, 2, 45),
                new Promotion(3, PromotionType.Combination, new List<string> {"C", "D"}, 1, 30)
            };

            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, promotions);
            Assert.Equal(280, invoiceAmount);
        }

        [Fact]
        public void OrderWithOrderDetails_MultiplePromotionTypeExists_WithResidualQuantity()
        {
            Order order = new Order(1, new List<OrderDetail>()
            {

                new(quantity: 5, new StockKeepingUnit("A", 50)),
                new(quantity: 5, new StockKeepingUnit("B", 30)),
                new(quantity: 2, new StockKeepingUnit("C", 20)),
                new(quantity: 1, new StockKeepingUnit("D", 15))
            });

            List<Promotion> promotions = new List<Promotion>
            {
                new Promotion(1, PromotionType.Quantity, new List<string> {"A"}, 3, 130),
                new Promotion(2, PromotionType.Quantity, new List<string> {"B"}, 2, 45),
                new Promotion(3, PromotionType.Combination, new List<string> {"C", "D"}, 1, 30)
            };

            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order, promotions);
            Assert.Equal(400, invoiceAmount);
        }
    }
}