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
        public void OrderWithoutOrderDetailsReturnsZero()
        {
            Order order = new Order(1, new List<OrderDetail>());
            decimal invoiceAmount = _engine.CalculateInvoiceAmount(order);
            Assert.Equal(0, invoiceAmount);
        }
    }
}