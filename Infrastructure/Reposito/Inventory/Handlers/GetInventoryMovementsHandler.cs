using Application.DTO.Response.Inventory;
using Application.Interfaces.Repositories;
using Application.Services.Inventory.Queries;
using MediatR;

namespace Infrastructure.Reposito.Inventory.Handlers
{
    public class GetInventoryMovementsHandler : IRequestHandler<GetInventoryMovementsQuery, PagedResponse<InventoryMovementResponseDto>>
    {
        private readonly IInventoryMovementRepository _repository;

        public GetInventoryMovementsHandler(IInventoryMovementRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<InventoryMovementResponseDto>> Handle(GetInventoryMovementsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetFilteredMovementsAsync(request.Filter);
        }
    }
}
