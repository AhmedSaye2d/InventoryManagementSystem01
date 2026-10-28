using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Locations;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Location
{
    public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public DeleteLocationHandler(IUnitOfWork unitOfWork, Application.Interfaces.ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<ServicesResponse> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _unitOfWork.Locations.GetByIdAsync(request.Id);

                if (location == null)
                {
                    return GeneralDbResponses.ItemNotFound("location");
                }

                _unitOfWork.Locations.Delete(location);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync("locations:all");
                await _cacheService.RemoveAsync($"location:{request.Id}");

                return GeneralDbResponses.ItemDeleted(location.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
