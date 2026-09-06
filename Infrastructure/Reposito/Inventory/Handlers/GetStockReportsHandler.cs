using Application.DTO.Response.Inventory;
using Application.Interfaces.Repositories;
using Application.Services.Inventory.Queries;
using Domain.Entities.Inventory;
using MediatR;

namespace Infrastructure.Reposito.Inventory.Handlers
{
    public abstract class BaseStockReportHandler
    {
        protected readonly IInventoryMovementRepository _repository;

        protected BaseStockReportHandler(IInventoryMovementRepository repository)
        {
            _repository = repository;
        }

        protected async Task<PagedResponse<InventoryMovementResponseDto>> GetReportAsync(
            MovementType type,
            Guid? productId,
            DateTime? from,
            DateTime? to,
            int pageNumber,
            int pageSize,
            CancellationToken ct)
        {
            return await _repository.GetReportAsync(type, productId, from, to, pageNumber, pageSize);
        }
    }

    public class GetStockInReportHandler : BaseStockReportHandler, IRequestHandler<GetStockInReportQuery, PagedResponse<InventoryMovementResponseDto>>
    {
        public GetStockInReportHandler(IInventoryMovementRepository repository) : base(repository) { }

        public async Task<PagedResponse<InventoryMovementResponseDto>> Handle(GetStockInReportQuery request, CancellationToken cancellationToken)
        {
            return await GetReportAsync(MovementType.StockIn, request.ProductId, request.DateFrom, request.DateTo, request.PageNumber, request.PageSize, cancellationToken);
        }
    }

    public class GetStockOutReportHandler : BaseStockReportHandler, IRequestHandler<GetStockOutReportQuery, PagedResponse<InventoryMovementResponseDto>>
    {
        public GetStockOutReportHandler(IInventoryMovementRepository repository) : base(repository) { }

        public async Task<PagedResponse<InventoryMovementResponseDto>> Handle(GetStockOutReportQuery request, CancellationToken cancellationToken)
        {
            return await GetReportAsync(MovementType.StockOut, request.ProductId, request.DateFrom, request.DateTo, request.PageNumber, request.PageSize, cancellationToken);
        }
    }

    public class GetAdjustmentReportHandler : BaseStockReportHandler, IRequestHandler<GetAdjustmentReportQuery, PagedResponse<InventoryMovementResponseDto>>
    {
        public GetAdjustmentReportHandler(IInventoryMovementRepository repository) : base(repository) { }

        public async Task<PagedResponse<InventoryMovementResponseDto>> Handle(GetAdjustmentReportQuery request, CancellationToken cancellationToken)
        {
            return await GetReportAsync(MovementType.Adjustment, request.ProductId, request.DateFrom, request.DateTo, request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
