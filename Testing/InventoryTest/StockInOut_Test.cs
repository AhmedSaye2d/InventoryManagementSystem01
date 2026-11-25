using Application.DTO.Request.Inventory;
using Application.Services.Inventory.Commands;
using Domain.Entities.Product;
using Infrastructure.DataAccess;
using Infrastructure.Reposito.Inventory.Handlers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Testing.InventoryTest
{
    [TestFixture]
    public class StockInOut_Test
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new AppDbContext(options);
        }

        private async Task<Guid> Seed(AppDbContext ctx, int qty = 100)
        {
            var loc = new Location { Id = Guid.NewGuid(), Name = "A-01" };
            var cat = new Category { Id = Guid.NewGuid(), Name = "Cat" };
            var p = new Product
            {
                Id = Guid.NewGuid(), Name = "HP Laptop", CategoryId = cat.Id, LocationId = loc.Id,
                Price = 1000, Quantity = qty, SerialNumber = Guid.NewGuid().ToString(),
                Description = "desc", Base64Image = "img"
            };
            ctx.Locations.Add(loc); ctx.Categories.Add(cat); ctx.Products.Add(p);
            await ctx.SaveChangesAsync();
            return p.Id;
        }

        [Test]
        public async Task StockIn_IncreasesQuantity_AndCreatesMovement()
        {
            using var ctx = CreateContext();
            var pid = await Seed(ctx, 100);
            var handler = new StockInHandler(ctx);
            var req = new StockInRequestDto { ProductId = pid, Quantity = 20, Reason = "Purchase" };
            var res = await handler.Handle(new StockInCommand(req, "user1"), CancellationToken.None);

            Assert.That(res.Success, Is.True);
            Assert.That(res.OldQuantity, Is.EqualTo(100));
            Assert.That(res.NewQuantity, Is.EqualTo(120));
            Assert.That(res.MovementQuantity, Is.EqualTo(20));
            var prod = await ctx.Products.FindAsync(pid);
            Assert.That(prod!.Quantity, Is.EqualTo(120));
            Assert.That(ctx.InventoryMovements.Count(), Is.EqualTo(1));
            var mov = await ctx.InventoryMovements.FirstAsync();
            Assert.That(mov.MovementType, Is.EqualTo(Domain.Entities.Inventory.MovementType.StockIn));
            Assert.That(mov.Quantity, Is.EqualTo(20));
            Assert.That(mov.QuantityAfter, Is.EqualTo(120));
        }

        [Test]
        public async Task StockOut_DecreasesQuantity_AndCreatesNegativeMovement()
        {
            using var ctx = CreateContext();
            var pid = await Seed(ctx, 120);
            var handler = new StockOutHandler(ctx);
            var req = new StockOutRequestDto { ProductId = pid, Quantity = 5, Reason = "Sale" };
            var res = await handler.Handle(new StockOutCommand(req, "user1"), CancellationToken.None);

            Assert.That(res.Success, Is.True);
            Assert.That(res.NewQuantity, Is.EqualTo(115));
            var mov = await ctx.InventoryMovements.FirstAsync();
            Assert.That(mov.Quantity, Is.EqualTo(-5));
            Assert.That(mov.MovementType, Is.EqualTo(Domain.Entities.Inventory.MovementType.StockOut));
        }

        [Test]
        public async Task StockOut_InsufficientStock_Fails_AndNoChange()
        {
            using var ctx = CreateContext();
            var pid = await Seed(ctx, 10);
            var handler = new StockOutHandler(ctx);
            var req = new StockOutRequestDto { ProductId = pid, Quantity = 20, Reason = "Sale" };
            var res = await handler.Handle(new StockOutCommand(req, "user1"), CancellationToken.None);

            Assert.That(res.Success, Is.False);
            Assert.That(res.Message, Does.Contain("Insufficient"));
            var prod = await ctx.Products.FindAsync(pid);
            Assert.That(prod!.Quantity, Is.EqualTo(10));
            Assert.That(ctx.InventoryMovements.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task StockIn_ProductNotFound_Fails()
        {
            using var ctx = CreateContext();
            var handler = new StockInHandler(ctx);
            var req = new StockInRequestDto { ProductId = Guid.NewGuid(), Quantity = 10 };
            var res = await handler.Handle(new StockInCommand(req, "user1"), CancellationToken.None);
            Assert.That(res.Success, Is.False);
            Assert.That(res.Message, Does.Contain("not found"));
        }

        [Test]
        public async Task StockIn_ZeroQuantity_Fails()
        {
            using var ctx = CreateContext();
            var pid = await Seed(ctx, 10);
            var handler = new StockInHandler(ctx);
            var req = new StockInRequestDto { ProductId = pid, Quantity = 0 };
            var res = await handler.Handle(new StockInCommand(req, "user1"), CancellationToken.None);
            Assert.That(res.Success, Is.False);
        }
    }
}
