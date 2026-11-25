using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Locations;
using Mapster;
using MediatR;
using LocationEntity = Domain.Entities.Product.Location;

namespace Infrastructure.Reposito.Product.Handlers.Location
{
    public class UpdateLocationHandler : IRequestHandler<UpdateLocationCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public UpdateLocationHandler(IUnitOfWork unitOfWork, Application.Interfaces.ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<ServicesResponse> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _unitOfWork.Locations.GetByIdAsync(request.LocationModel.Id);
                if (location == null)
                {
                    return GeneralDbResponses.ItemNotFound(request.LocationModel.Name);
                }

                var data = request.LocationModel.Adapt(new LocationEntity());

                _unitOfWork.Locations.Update(data);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync("locations:all");
                await _cacheService.RemoveAsync($"location:{request.LocationModel.Id}");

                return GeneralDbResponses.ItemUpdated(request.LocationModel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
