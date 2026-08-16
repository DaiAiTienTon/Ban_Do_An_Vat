using System;
using System.Threading.Tasks;
using Ban_Do_An_Vat.Controllers;
using Ban_Do_An_Vat.Data;
using Ban_Do_An_Vat.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ban_Do_An_Vat.Tests
{
    public class ImageControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageController _controller;

        public ImageControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new ImageController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task Snack_ShouldReturnFileResult_WhenImageDataExists()
        {
            byte[] dummyData = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var snack = new Snack
            {
                Id = 1,
                Name = "Bánh tráng phơi sương",
                Description = "Mô tả",
                Price = 20000,
                StockQuantity = 10,
                ImageData = dummyData,
                ImageContentType = "image/png"
            };
            _context.Snacks.Add(snack);
            await _context.SaveChangesAsync();

            var result = await _controller.Snack(1);

            result.Should().BeOfType<FileContentResult>();
            var fileResult = (FileContentResult)result;
            fileResult.FileContents.Should().BeEquivalentTo(dummyData);
            fileResult.ContentType.Should().Be("image/png");
        }

        [Fact]
        public async Task Snack_ShouldRedirectToUrl_WhenImageDataIsNullButImageUrlExists()
        {
            var snack = new Snack
            {
                Id = 2,
                Name = "Khoai tây chiên",
                Description = "Mô tả",
                Price = 15000,
                StockQuantity = 10,
                ImageData = null,
                ImageUrl = "https://example.com/snack.jpg"
            };
            _context.Snacks.Add(snack);
            await _context.SaveChangesAsync();

            var result = await _controller.Snack(2);

            result.Should().BeOfType<RedirectResult>();
            var redirectResult = (RedirectResult)result;
            redirectResult.Url.Should().Be("https://example.com/snack.jpg");
        }

        [Fact]
        public async Task Snack_ShouldReturnNotFound_WhenSnackDoesNotExist()
        {
            var result = await _controller.Snack(999);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Combo_ShouldReturnFileResult_WhenImageDataExists()
        {
            byte[] dummyData = new byte[] { 0xAA, 0xBB, 0xCC };
            var combo = new Combo
            {
                Id = 1,
                Name = "Combo Siêu Tiết Kiệm",
                Description = "Mô tả combo",
                OriginalPrice = 100000,
                SalePrice = 80000,
                ImageData = dummyData,
                ImageContentType = "image/jpeg"
            };
            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();

            var result = await _controller.Combo(1);

            result.Should().BeOfType<FileContentResult>();
            var fileResult = (FileContentResult)result;
            fileResult.FileContents.Should().BeEquivalentTo(dummyData);
            fileResult.ContentType.Should().Be("image/jpeg");
        }

        [Fact]
        public async Task Category_ShouldReturnFileResult_WhenImageDataExists()
        {
            byte[] dummyData = new byte[] { 0x10, 0x20 };
            var category = new Category
            {
                Id = 1,
                Name = "Đồ ăn vặt cay",
                Description = "Các loại món cay",
                ImageUrl = "/images/categories/cay.jpg",
                ImageData = dummyData,
                ImageContentType = "image/webp"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = await _controller.Category(1);

            result.Should().BeOfType<FileContentResult>();
            var fileResult = (FileContentResult)result;
            fileResult.FileContents.Should().BeEquivalentTo(dummyData);
            fileResult.ContentType.Should().Be("image/webp");
        }
    }
}
