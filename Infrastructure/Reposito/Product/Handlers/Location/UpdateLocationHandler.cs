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

        public UpdateLocationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

                return GeneralDbResponses.ItemUpdated(request.LocationModel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
