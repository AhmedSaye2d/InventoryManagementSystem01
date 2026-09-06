using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Locations;
using Mapster;
using MediatR;
using LocationEntity = Domain.Entities.Product.Location;

namespace Infrastructure.Reposito.Product.Handlers.Location
{
    public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLocationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServicesResponse> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _unitOfWork.Locations.ExistsAsync(x => x.Name.ToLower() == request.locationmodel.Name.ToLower());
                if (exists)
                {
                    return GeneralDbResponses.ItemAlreadyExist(request.locationmodel.Name);
                }

                var data = request.locationmodel.Adapt(new LocationEntity());

                await _unitOfWork.Locations.AddAsync(data);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemCreated(request.locationmodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
