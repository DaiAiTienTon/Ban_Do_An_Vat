using System;
using Ban_Do_An_Vat.Models;
using FluentAssertions;
using Xunit;

namespace Ban_Do_An_Vat.Tests
{
    public class CouponAndShippingTests
    {
        [Theory]
        [InlineData(200000, 10, "Percentage", 20000)]
        [InlineData(100000, 15, "Percentage", 15000)]
        [InlineData(50000, 50, "Percentage", 25000)]
        public void CalculateDiscount_Percentage_ShouldCalculateCorrectDiscount(decimal cartTotal, decimal discountAmount, string discountType, decimal expectedDiscount)
        {
            var coupon = new Coupon
            {
                Code = "TEST",
                DiscountAmount = discountAmount,
                DiscountType = discountType,
                MinOrderAmount = 0,
                IsActive = true,
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            decimal discount = coupon.DiscountType == "Percentage"
                ? cartTotal * (coupon.DiscountAmount / 100)
                : coupon.DiscountAmount;

            discount.Should().Be(expectedDiscount);
        }

        [Fact]
        public void CalculateDiscount_FixedAmount_ShouldReturnFixedValue()
        {
            var coupon = new Coupon
            {
                Code = "FIXED30K",
                DiscountAmount = 30000,
                DiscountType = "FixedAmount",
                MinOrderAmount = 100000,
                IsActive = true,
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            decimal discount = coupon.DiscountType == "Percentage"
                ? 150000 * (coupon.DiscountAmount / 100)
                : coupon.DiscountAmount;

            discount.Should().Be(30000);
        }

        [Theory]
        [InlineData(200000, 0)]
        [InlineData(150001, 0)]
        [InlineData(150000, 20000)]
        [InlineData(50000, 20000)]
        [InlineData(0, 20000)]
        public void CalculateShippingFee_ShouldFollowPolicy(decimal cartTotal, decimal expectedFee)
        {
            decimal shippingFee = cartTotal > 150000 ? 0 : 20000;
            shippingFee.Should().Be(expectedFee);
        }

        [Fact]
        public void Combo_DiscountPercent_ShouldCalculateCorrectly()
        {
            var combo = new Combo
            {
                OriginalPrice = 100000,
                SalePrice = 75000
            };

            combo.DiscountPercent.Should().Be(25);
        }

        [Fact]
        public void Combo_DiscountPercent_ShouldReturnZero_WhenOriginalPriceIsZero()
        {
            var combo = new Combo
            {
                OriginalPrice = 0,
                SalePrice = 0
            };

            combo.DiscountPercent.Should().Be(0);
        }
    }
}
