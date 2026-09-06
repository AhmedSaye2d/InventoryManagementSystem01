using Application.DTO.Request.Inventory;
using Application.DTO.Response.Inventory;
using Application.Interfaces.Repositories;
using Domain.Entities.Inventory;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InventoryMovementRepository : GenericRepository<InventoryMovement>, IInventoryMovementRepository
    {
        public InventoryMovementRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<InventoryMovement?> GetMovementByIdWithProductAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<InventoryMovementResponseDto?> GetMovementByIdResponseAsync(Guid id)
        {
            var movement = await _dbSet
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Location)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movement == null) return null;

            return new InventoryMovementResponseDto
            {
                Id = movement.Id,
                ProductId = movement.ProductId,
                ProductName = movement.Product.Name,
                LocationId = movement.LocationId,
                LocationName = movement.Location.Name,
                Quantity = movement.Quantity,
                MovementType = movement.MovementType,
                Reason = movement.Reason,
                CreatedAt = movement.CreatedAt,
                UserId = movement.UserId,
                ReferenceId = movement.ReferenceId,
                QuantityAfter = movement.QuantityAfter
            };
        }

        public async Task<PagedResponse<InventoryMovementResponseDto>> GetFilteredMovementsAsync(InventoryMovementFilterRequestDto filter)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Location)
                .AsQueryable();

            if (filter.ProductId.HasValue)
                query = query.Where(x => x.ProductId == filter.ProductId.Value);

            if (filter.LocationId.HasValue)
                query = query.Where(x => x.LocationId == filter.LocationId.Value);

            if (filter.MovementType.HasValue)
                query = query.Where(x => x.MovementType == filter.MovementType.Value);

            if (!string.IsNullOrWhiteSpace(filter.UserId))
                query = query.Where(x => x.UserId == filter.UserId);

            if (filter.DateFrom.HasValue)
                query = query.Where(x => x.CreatedAt >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(x => x.CreatedAt <= filter.DateTo.Value);

            query = (filter.SortBy?.ToLower()) switch
            {
                "quantity" => filter.SortDescending ? query.OrderByDescending(x => x.Quantity) : query.OrderBy(x => x.Quantity),
                "movementtype" => filter.SortDescending ? query.OrderByDescending(x => x.MovementType) : query.OrderBy(x => x.MovementType),
                _ => filter.SortDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 20 : (filter.PageSize > 100 ? 100 : filter.PageSize);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new InventoryMovementResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    LocationId = x.LocationId,
                    LocationName = x.Location.Name,
                    Quantity = x.Quantity,
                    MovementType = x.MovementType,
                    Reason = x.Reason,
                    CreatedAt = x.CreatedAt,
                    UserId = x.UserId,
                    ReferenceId = x.ReferenceId,
                    QuantityAfter = x.QuantityAfter
                })
                .ToListAsync();

            return new PagedResponse<InventoryMovementResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResponse<InventoryMovementResponseDto>> GetProductHistoryAsync(Guid productId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Location)
                .Where(x => x.ProductId == productId);

            if (dateFrom.HasValue)
                query = query.Where(x => x.CreatedAt >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.CreatedAt <= dateTo.Value);

            query = query.OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync();
            var validPageNumber = pageNumber < 1 ? 1 : pageNumber;
            var validPageSize = pageSize < 1 ? 20 : (pageSize > 100 ? 100 : pageSize);

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .Select(x => new InventoryMovementResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    LocationId = x.LocationId,
                    LocationName = x.Location.Name,
                    Quantity = x.Quantity,
                    MovementType = x.MovementType,
                    Reason = x.Reason,
                    CreatedAt = x.CreatedAt,
                    UserId = x.UserId,
                    ReferenceId = x.ReferenceId,
                    QuantityAfter = x.QuantityAfter
                })
                .ToListAsync();

            return new PagedResponse<InventoryMovementResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = validPageNumber,
                PageSize = validPageSize
            };
        }

        public async Task<PagedResponse<InventoryMovementResponseDto>> GetReportAsync(MovementType type, Guid? productId, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            var query = _dbSet.AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Location)
                .Where(x => x.MovementType == type);

            if (productId.HasValue) query = query.Where(x => x.ProductId == productId.Value);
            if (from.HasValue) query = query.Where(x => x.CreatedAt >= from.Value);
            if (to.HasValue) query = query.Where(x => x.CreatedAt <= to.Value);

            query = query.OrderByDescending(x => x.CreatedAt);

            var total = await query.CountAsync();
            var validPageNumber = pageNumber < 1 ? 1 : pageNumber;
            var validPageSize = pageSize < 1 ? 20 : (pageSize > 100 ? 100 : pageSize);

            var items = await query.Skip((validPageNumber - 1) * validPageSize).Take(validPageSize)
                .Select(x => new InventoryMovementResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    LocationId = x.LocationId,
                    LocationName = x.Location.Name,
                    Quantity = x.Quantity,
                    MovementType = x.MovementType,
                    Reason = x.Reason,
                    CreatedAt = x.CreatedAt,
                    UserId = x.UserId,
                    ReferenceId = x.ReferenceId,
                    QuantityAfter = x.QuantityAfter
                }).ToListAsync();

            return new PagedResponse<InventoryMovementResponseDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = validPageNumber,
                PageSize = validPageSize
            };
        }
    }
}
