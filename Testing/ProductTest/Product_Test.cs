using Domain.Entities.Product;
using Infrastructure.DataAccess;
using Infrastructure.UnitOfWork;
using Infrastructure.Reposito.Product.Handlers.Products;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Application.Services.Product.Commands.Products;
using Application.DTO.Request.Product;


namespace Testing.ProductTest
{
    
    [TestFixture]
    public class Product_Test
    {

        [Test]
        public async Task CreateProduct_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new AppDbContext(options);
            var unitOfWork = new UnitOfWork(context);
            var handler = new CreateProductHandler(unitOfWork);

            var command = new CreateProductCommand(new AddProductRequestDto
            {
                Name = "Laptop",
                Price = 30000,
                Quantity = 10,
                CategoryId = Guid.NewGuid(),
                LocationId = Guid.NewGuid(),
                Base64Image = "base64image",
                SerialNumber = "12345",
                Description = "A description"
            });
            // Act
            var result = await handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Console.WriteLine($"Result Message: {result.Message}");
            Assert.That(result.Flag, Is.True, result.Message);

            Assert.That(
                context.Products.Count(),
                Is.EqualTo(1));
        }


    }
}
