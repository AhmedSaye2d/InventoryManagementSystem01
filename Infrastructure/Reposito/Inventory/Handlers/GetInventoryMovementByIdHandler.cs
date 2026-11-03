using Application.DTO.Response.Inventory;
using Application.Interfaces.Repositories;
using Application.Services.Inventory.Queries;
using MediatR;

namespace Infrastructure.Reposito.Inventory.Handlers
{
    public class GetInventoryMovementByIdHandler : IRequestHandler<GetInventoryMovementByIdQuery, InventoryMovementResponseDto?>
    {
        private readonly IInventoryMovementRepository _repository;

        public GetInventoryMovementByIdHandler(IInventoryMovementRepository repository)
        {
            _repository = repository;
        }

        public async Task<InventoryMovementResponseDto?> Handle(GetInventoryMovementByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMovementByIdResponseAsync(request.Id);
        }
    }
}
