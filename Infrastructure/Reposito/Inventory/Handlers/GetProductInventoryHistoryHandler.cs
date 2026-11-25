using Application.DTO.Response.Inventory;
using Application.Interfaces.Repositories;
using Application.Services.Inventory.Queries;
using MediatR;

namespace Infrastructure.Reposito.Inventory.Handlers
{
    public class GetProductInventoryHistoryHandler : IRequestHandler<GetProductInventoryHistoryQuery, PagedResponse<InventoryMovementResponseDto>>
    {
        private readonly IInventoryMovementRepository _repository;

        public GetProductInventoryHistoryHandler(IInventoryMovementRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<InventoryMovementResponseDto>> Handle(GetProductInventoryHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetProductHistoryAsync(
                request.ProductId,
                request.DateFrom,
                request.DateTo,
                request.PageNumber,
                request.PageSize);
        }
    }
}
