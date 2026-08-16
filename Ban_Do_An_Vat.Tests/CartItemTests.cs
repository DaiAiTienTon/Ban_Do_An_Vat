using Ban_Do_An_Vat.Models;
using FluentAssertions;
using Xunit;

namespace Ban_Do_An_Vat.Tests
{
    public class CartItemTests
    {
        [Theory]
        [InlineData(10000, 2, 20000)]
        [InlineData(50000, 1, 50000)]
        [InlineData(15000, 3, 45000)]
        public void TotalPrice_ShouldCalculateCorrectly_WhenQuantityAndPriceArePositive(decimal price, int quantity, decimal expectedTotal)
        {
            var item = new CartItem
            {
                Price = price,
                Quantity = quantity
            };

            item.TotalPrice.Should().Be(expectedTotal);
        }

        [Fact]
        public void TotalPrice_ShouldReturnZero_WhenQuantityIsZero()
        {
            var item = new CartItem
            {
                Price = 25000,
                Quantity = 0
            };

            item.TotalPrice.Should().Be(0);
        }

        [Fact]
        public void TotalPrice_ShouldReturnZero_WhenPriceIsZero()
        {
            var item = new CartItem
            {
                Price = 0,
                Quantity = 5
            };

            item.TotalPrice.Should().Be(0);
        }
    }
}
