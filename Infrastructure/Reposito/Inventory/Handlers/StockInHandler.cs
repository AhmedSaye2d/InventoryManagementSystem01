using Application.DTO.Response.Inventory;
using Application.Interfaces.Repositories;
using Application.Services.Inventory.Commands;
using Domain.Entities.Inventory;
using Infrastructure.Reposito.Product.Handlers;
using MediatR;

namespace Infrastructure.Reposito.Inventory.Handlers
{
    public class StockInHandler : IRequestHandler<StockInCommand, StockOperationResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockInHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StockOperationResponseDto> Handle(StockInCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Request;
            var userId = request.UserId;

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null)
            {
                var notFound = GeneralDbResponses.ItemNotFound($"Product {dto.ProductId}");
                return new StockOperationResponseDto { Success = notFound.Flag, Message = notFound.Message };
            }

            // Resolve location: use provided or product's location
            var locationId = dto.LocationId ?? product.LocationId;
            var locationExists = await _unitOfWork.Locations.ExistsAsync(l => l.Id == locationId);
            if (!locationExists)
            {
                var notFound = GeneralDbResponses.ItemNotFound($"Location {locationId}");
                return new StockOperationResponseDto { Success = notFound.Flag, Message = notFound.Message };
            }

            if (dto.Quantity <= 0)
            {
                return new StockOperationResponseDto { Success = false, Message = "Quantity must be greater than zero" };
            }

            var oldQuantity = product.Quantity;
            var newQuantity = oldQuantity + dto.Quantity;

            var movement = new InventoryMovement
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                LocationId = locationId,
                Quantity = dto.Quantity, // positive
                MovementType = MovementType.StockIn,
                Reason = dto.Reason ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UserId = string.IsNullOrWhiteSpace(userId) ? "system" : userId,
                ReferenceId = dto.ReferenceId,
                QuantityAfter = newQuantity
            };

            product.Quantity = newQuantity;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.InventoryMovements.AddAsync(movement);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StockOperationResponseDto
            {
                Success = true,
                Message = $"Stock In +{dto.Quantity} succeeded",
                ProductId = product.Id,
                ProductName = product.Name,
                OldQuantity = oldQuantity,
                NewQuantity = newQuantity,
                MovementQuantity = dto.Quantity,
                MovementType = MovementType.StockIn,
                MovementId = movement.Id,
                LocationId = locationId
            };
        }
    }
}
