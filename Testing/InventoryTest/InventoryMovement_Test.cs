using Application.DTO.Request.Inventory;
using Application.Services.Inventory.Queries;
using Domain.Entities.Inventory;
using Domain.Entities.Product;
using Infrastructure.DataAccess;
using Infrastructure.UnitOfWork;
using Infrastructure.Reposito.Inventory.Handlers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Testing.InventoryTest
{
    [TestFixture]
    public class InventoryMovement_Test
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private async Task<(Guid productId, Guid locationId)> SeedProductAndLocation(AppDbContext context)
        {
            var location = new Location { Id = Guid.NewGuid(), Name = "A-01" };
            var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "HP Laptop",
                CategoryId = category.Id,
                LocationId = location.Id,
                Price = 1000,
                Quantity = 100,
                SerialNumber = Guid.NewGuid().ToString(),
                Description = "Test product",
                Base64Image = "base64"
            };
            context.Locations.Add(location);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return (product.Id, location.Id);
        }

        [Test]
        public async Task GetInventoryMovements_FilterByProduct_ReturnsCorrectItems()
        {
            using var context = CreateContext();
            var (productId, locationId) = await SeedProductAndLocation(context);

            context.InventoryMovements.AddRange(
                new InventoryMovement { ProductId = productId, LocationId = locationId, Quantity = 50, MovementType = MovementType.StockIn, Reason = "Initial stock", CreatedAt = DateTime.UtcNow.AddDays(-2), UserId = "user1" },
                new InventoryMovement { ProductId = productId, LocationId = locationId, Quantity = -10, MovementType = MovementType.StockOut, Reason = "Sale", CreatedAt = DateTime.UtcNow.AddDays(-1), UserId = "user1" },
                new InventoryMovement { ProductId = Guid.NewGuid(), LocationId = locationId, Quantity = 20, MovementType = MovementType.StockIn, Reason = "Other product", CreatedAt = DateTime.UtcNow, UserId = "user2" }
            );
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var handler = new GetInventoryMovementsHandler(unitOfWork.InventoryMovements);
            var filter = new InventoryMovementFilterRequestDto { ProductId = productId, PageNumber = 1, PageSize = 10 };
            var result = await handler.Handle(new GetInventoryMovementsQuery(filter), CancellationToken.None);

            Assert.That(result.TotalCount, Is.EqualTo(2));
            Assert.That(result.Items.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetProductInventoryHistory_Pagination_Works()
        {
            using var context = CreateContext();
            var (productId, locationId) = await SeedProductAndLocation(context);

            for (int i = 0; i < 25; i++)
            {
                context.InventoryMovements.Add(new InventoryMovement
                {
                    ProductId = productId,
                    LocationId = locationId,
                    Quantity = 1,
                    MovementType = MovementType.StockIn,
                    Reason = $"Entry {i}",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                    UserId = "user1"
                });
            }
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var handler = new GetProductInventoryHistoryHandler(unitOfWork.InventoryMovements);
            var result = await handler.Handle(new GetProductInventoryHistoryQuery(productId, PageNumber: 2, PageSize: 10), CancellationToken.None);

            Assert.That(result.TotalCount, Is.EqualTo(25));
            Assert.That(result.Items.Count(), Is.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.TotalPages, Is.EqualTo(3));
        }

        [Test]
        public async Task GetInventoryMovementById_Existing_ReturnsDto()
        {
            using var context = CreateContext();
            var (productId, locationId) = await SeedProductAndLocation(context);

            var movement = new InventoryMovement
            {
                ProductId = productId,
                LocationId = locationId,
                Quantity = -5,
                MovementType = MovementType.Adjustment,
                Reason = "Damaged",
                CreatedAt = DateTime.UtcNow,
                UserId = "user1"
            };
            context.InventoryMovements.Add(movement);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var handler = new GetInventoryMovementByIdHandler(unitOfWork.InventoryMovements);
            var result = await handler.Handle(new GetInventoryMovementByIdQuery(movement.Id), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Quantity, Is.EqualTo(-5));
            Assert.That(result.MovementType, Is.EqualTo(MovementType.Adjustment));
        }

        [Test]
        public async Task GetStockInReport_OnlyReturnsStockIn()
        {
            using var context = CreateContext();
            var (productId, locationId) = await SeedProductAndLocation(context);

            context.InventoryMovements.AddRange(
                new InventoryMovement { ProductId = productId, LocationId = locationId, Quantity = 10, MovementType = MovementType.StockIn, Reason = "in", CreatedAt = DateTime.UtcNow, UserId = "u1" },
                new InventoryMovement { ProductId = productId, LocationId = locationId, Quantity = -5, MovementType = MovementType.StockOut, Reason = "out", CreatedAt = DateTime.UtcNow, UserId = "u1" },
                new InventoryMovement { ProductId = productId, LocationId = locationId, Quantity = -2, MovementType = MovementType.Adjustment, Reason = "adj", CreatedAt = DateTime.UtcNow, UserId = "u1" }
            );
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var handler = new GetStockInReportHandler(unitOfWork.InventoryMovements);
            var result = await handler.Handle(new GetStockInReportQuery(1, 10), CancellationToken.None);

            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.Items.First().MovementType, Is.EqualTo(MovementType.StockIn));
        }
    }
}
